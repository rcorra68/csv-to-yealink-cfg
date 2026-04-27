namespace CsvToYealink.Core.Entities;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public class SafeIntConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0; // Turns the empty field into zero
        }

        return int.Parse(text);
    }
}
