namespace CsvToYealink.Core.Services.Interfaces;

using CsvToYealink.Core.Entities;

internal interface ICsvParser
{
    IEnumerable<Terminal> Parse(string filePath);
}
