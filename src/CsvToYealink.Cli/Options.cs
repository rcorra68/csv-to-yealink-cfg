namespace CsvToYealink.Cli;

using CommandLine;

public class Options
{
    [Option('s', "source", Required = true, HelpText = "Source file path: .csv or .xlsx (Schema_Estensione layout).")]
    public string SourcePath { get; set; } = string.Empty;

    [Option('o', "output", Required = false, Default = "directory.cfg", HelpText = "Output .cfg file path.")]
    public string OutputPath { get; set; } = "directory.cfg";

    [Option('p', "dial-prefix", Default = "0", HelpText = "Outside line prefix added to type 2 numbers (Excel sources only). Use -p \"\" to disable.")]
    public string DialPrefix { get; set; } = "0";

    [Option('t', "template", Required = false, HelpText = "Path to a custom template file.")]
    public string? TemplatePath { get; set; }

    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('e', "env", Default = "Development", HelpText = "Target environment (Development, Staging, Production).")]
    public string Environment { get; set; } = "Development";
}
