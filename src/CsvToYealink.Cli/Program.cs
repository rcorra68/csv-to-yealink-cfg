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
            var parser = new CsvService();
            var generator = new YealinkConfigGenerator();

            // 1. Read
            var terminals = parser.ReadRecords<Terminal>(opts.SourcePath);

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