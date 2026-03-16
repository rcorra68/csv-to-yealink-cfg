namespace CsvToYealink.Core.Services;

using System.Collections.Generic;
using System.Globalization;

using CsvHelper;

using CsvToYealink.Core.Entities;
using CsvToYealink.Core.Services.Interfaces;

public class CsvParser : ICsvParser
{
    public IEnumerable<Terminal> Parse(string filePath)
    {
        var terminals = new List<Terminal>();

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<TerminalMap>();
            terminals = csv.GetRecords<Terminal>().ToList();
        }

        return terminals;
    }
}
