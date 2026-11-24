namespace CsvToYealink.Core.Services;

using System.Text.RegularExpressions;

using ClosedXML.Excel;

using CsvToYealink.Core.Entities;

/// <summary>
/// Reads expansion module keys from the "Schema_Estensione" Excel layout.
/// Each row holds two keys side by side:
///   A = key | B = label | C = type | D = value      (left key)
///   F = label | G = type | H = value | I = key      (right key)
/// Rows whose key cell is not a number (e.g. "PAGINA 1" separators) are skipped.
/// Keys with an empty number are returned as reset keys (type None), so the
/// phone shows them as free slots.
/// </summary>
public partial class ExcelService
{
    private sealed record Side(int KeyCol, int LabelCol, int TypeCol, int ValueCol);

    private static readonly Side[] Sides =
    [
        new Side(KeyCol: 1, LabelCol: 2, TypeCol: 3, ValueCol: 4), // A, B, C, D
        new Side(KeyCol: 9, LabelCol: 6, TypeCol: 7, ValueCol: 8), // I, F, G, H
    ];

    // Separators people type in phone numbers ("0765 123456", "800-123456")
    [GeneratedRegex(@"[\s\-\./()]")]
    private static partial Regex NumberSeparators();

    private readonly List<string> warnings = [];
    private readonly string dialPrefix;
    private readonly int internalMaxLength;

    /// <param name="dialPrefix">Outside line prefix added to Speed Dial (13) and Forward (2) numbers, e.g. "0" for the PBX.</param>
    /// <param name="internalMaxLength">Numbers up to this many digits are internal (extensions, queues) and get no prefix.</param>
    public ExcelService(string dialPrefix = "0", int internalMaxLength = 4)
    {
        this.dialPrefix = dialPrefix ?? string.Empty;
        this.internalMaxLength = internalMaxLength;
    }

    /// <summary>Non-blocking issues found during the last read (e.g. label without number).</summary>
    public IReadOnlyList<string> Warnings => this.warnings;

    public IEnumerable<Terminal> ReadTerminals(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Excel file not found.", filePath);
        }

        using var workbook = new XLWorkbook(filePath);
        var sheet = workbook.Worksheet(1);
        var terminals = new List<Terminal>();
        this.warnings.Clear();

        foreach (var row in sheet.RowsUsed())
        {
            foreach (var side in Sides)
            {
                var terminal = this.ReadKey(row, side);
                if (terminal is not null)
                {
                    terminals.Add(terminal);
                }
            }
        }

        return terminals.OrderBy(t => t.Key).ToList();
    }

    private Terminal? ReadKey(IXLRow row, Side side)
    {
        if (!int.TryParse(Text(row, side.KeyCol), out int key))
        {
            return null; // page separator or empty row
        }

        string label = Text(row, side.LabelCol);
        var valueCell = row.Cell(side.ValueCol);
        string value = NumberSeparators().Replace(Text(row, side.ValueCol), string.Empty);

        if (value.Length == 0)
        {
            if (label.Length > 0)
            {
                this.warnings.Add(
                    $"Cell {valueCell.Address}: key {key} ({label}) has no number and will be reset.");
            }

            // Free slot: reset the key on the phone
            return new Terminal { Key = key, ExtensionType = ExtensionType.None };
        }

        var typeCell = row.Cell(side.TypeCol);
        if (!int.TryParse(Text(row, side.TypeCol), out int typeValue)
            || !Enum.IsDefined(typeof(ExtensionType), typeValue)
            || typeValue == (int)ExtensionType.None)
        {
            throw new InvalidDataException(
                $"Cell {typeCell.Address}: invalid type '{typeCell.GetFormattedString()}' for key {key} ({label}). Use 13 (Speed Dial), 16 (BLF) or 2 (Forward).");
        }

        var type = (ExtensionType)typeValue;

        return new Terminal
        {
            Key = key,
            Label = label,
            ExtensionType = type,
            // BLF and Speed Dial keys use the phone's first account
            Line = type is ExtensionType.BLF or ExtensionType.SpeedDial ? 1 : 0,
            // External numbers go through the PBX: prepend the outside line prefix
            ExtensionValue = this.NeedsPrefix(type, value) ? this.dialPrefix + value : value,
        };
    }

    // Only external numbers need the outside line prefix: PBX feature codes ("mete", e.g. *5234)
    // and short internal numbers (extensions, queues such as 400) are dialed as they are
    private bool NeedsPrefix(ExtensionType type, string value) =>
        type is ExtensionType.SpeedDial or ExtensionType.Forward
        && !value.StartsWith('*')
        && !value.StartsWith('#')
        && value.Length > this.internalMaxLength;

    private static string Text(IXLRow row, int column) =>
        row.Cell(column).GetFormattedString().Trim();
}
