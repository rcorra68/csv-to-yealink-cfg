namespace CsvToYealink.Core.Entities;

public class Terminal
{
    public int Key { get; set; }
    public string Label { get; set; }
    public int Line { get; set; }
    public ExtensionType ExtensionType { get; set; }
    public string ExtensionValue { get; set; }
}
