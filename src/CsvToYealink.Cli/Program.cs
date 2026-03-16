namespace CsvToYealink.Cli;

using CommandLine;

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
            var parser = new CsvParser();
            var generator = new YealinkConfigGenerator();

            // 1. Read
            var terminals = parser.Parse(opts.SourcePath);

            // 2. Generate
            generator.Generate(opts.OutputPath, terminals);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
    private static void HandleParseError(IEnumerable<Error> errs)
    {
        // Handle errors (e.g., missing required arguments)
        Environment.Exit(1);
    }
}