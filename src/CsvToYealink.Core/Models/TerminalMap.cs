namespace CsvToYealink.Core.Entities;

using System.Globalization;

using CsvHelper.Configuration;

public sealed class TerminalMap : ClassMap<Terminal>
{
    public TerminalMap()
    {
        this.AutoMap(CultureInfo.InvariantCulture);

        this.Map(m => m.Key).Name("key");
        this.Map(m => m.Label).Name("label");
        this.Map(m => m.Line).Name("line").TypeConverter<SafeIntConverter>();
        this.Map(m => m.ExtensionType).Name("type");
        this.Map(m => m.ExtensionValue).Name("value");
    }
}
