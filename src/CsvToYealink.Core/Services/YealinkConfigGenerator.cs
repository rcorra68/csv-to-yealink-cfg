namespace CsvToYealink.Core.Services;

using System.Collections.Generic;

using CsvToYealink.Core.Entities;

public class YealinkConfigGenerator
{
    const int moduleId = 1;

    public void Generate(string outputPath, IEnumerable<Terminal> terminals)
    {
        ArgumentNullException.ThrowIfNull(terminals);

        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            // Mandatory header: Yealink phones ignore .cfg files without it on the first line
            writer.WriteLine("#!version:1.0.0.1");
            writer.WriteLine("# Yealink expansion module configuration");
            writer.WriteLine("#");

            foreach (Terminal terminal in terminals)
            {

                writer.WriteLine($"# --- expansion_module.{moduleId}.key.{terminal.Key} ---");
                writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.label = {terminal.Label}");
                writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.type = {(int)terminal.ExtensionType}");

                switch (terminal.ExtensionType)
                {
                    case ExtensionType.BLF:
                        writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.extension = **");
                        writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.line = {terminal.Line}");
                        writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.value = {terminal.ExtensionValue}");
                        break;
                    case ExtensionType.Forward:
                        writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.value = {terminal.ExtensionValue}");
                        break;
                    case ExtensionType.None:
                        // Reset: clear leftovers from a previous configuration
                        writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.value = ");
                        writer.WriteLine($"expansion_module.{moduleId}.key.{terminal.Key}.extension = ");
                        break;
                }
            }

            Console.WriteLine($"Successfully generated {terminals.Count()} key in: {outputPath}");
        }
    }
}