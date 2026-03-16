namespace CsvToYealink.Core.Services.Interfaces;

internal interface ICsvService
{
    IEnumerable<T> ReadRecords<T>(string filePath) where T : class;
}
