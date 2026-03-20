namespace CsvToYealink.Core.Entities;

public class Terminal
{
    public int Key { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Line { get; set; }
    public ExtensionType ExtensionType { get; set; }
    public string ExtensionValue { get; set; } = string.Empty;
}
