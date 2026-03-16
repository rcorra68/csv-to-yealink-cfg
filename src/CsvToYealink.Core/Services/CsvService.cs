namespace CsvToYealink.Core.Services;

using System.Collections.Generic;
using System.Globalization;

using CsvHelper;

using CsvToYealink.Core.Entities;
using CsvToYealink.Core.Services.Interfaces;

public class CsvService : ICsvService
{
    public IEnumerable<T> ReadRecords<T>(string filePath) where T : class
    {
        var terminals = new List<Terminal>();

        try
        {

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<TerminalMap>();
                return csv.GetRecords<T>().ToList();
            }
        }
        catch (FileNotFoundException)
        {
            // Re-throw with more context if needed, or let it bubble up
            throw;
        }
        catch (CsvHelperException ex)
        {
            if (ex.Context?.Parser != null)
            {
                throw new Exception($"Error parsing CSV at row {ex.Context.Parser.Row}: {ex.Message}");
            }
            throw new Exception($"Error parsing CSV: {ex.Message}");
        }
    }
}
