namespace CsvToYealink.Cli;

using CommandLine;

using CsvToYealink.Core.Entities;
using CsvToYealink.Core.Services;

public class Program
{
    static void Main(string[] args)
    {
        try
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void RunOptions(Options opts)
    {
        try
        {
            var generator = new YealinkConfigGenerator();

            // 1. Read (Excel layout or plain CSV, chosen by file extension)
            bool isExcel = Path.GetExtension(opts.SourcePath)
                .Equals(".xlsx", StringComparison.OrdinalIgnoreCase);

            IEnumerable<Terminal> terminals;
            if (isExcel)
            {
                var excel = new ExcelService(opts.DialPrefix);
                terminals = excel.ReadTerminals(opts.SourcePath);

                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (string warning in excel.Warnings)
                {
                    Console.WriteLine($"Warning: {warning}");
                }
                Console.ResetColor();
            }
            else
            {
                terminals = new CsvService().ReadRecords<Terminal>(opts.SourcePath);
            }

            // 2. Generate
            generator.Generate(opts.OutputPath, terminals);
        }
        catch (FileNotFoundException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: The file '{opts.SourcePath}' was not found.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.ResetColor();
        }
    }
    private static void HandleParseError(IEnumerable<Error> errs)
    {
        // Handle errors (e.g., missing required arguments)
        Environment.Exit(1);
    }
}