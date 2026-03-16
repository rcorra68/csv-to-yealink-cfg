namespace CsvToYealink.Core.Entities;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public class SafeIntConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0; // Trasforma il vuoto in zero
        }

        return int.Parse(text);
    }
}
