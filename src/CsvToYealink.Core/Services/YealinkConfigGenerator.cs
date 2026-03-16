namespace CsvToYealink.Core.Services;

using System.Collections.Generic;

using CsvToYealink.Core.Entities;

public class YealinkConfigGenerator
{
    const int moduleId = 1;

    public void Generate(string outputPath, IEnumerable<Terminal> terminals)
    {
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine("# Yealink Directory Configuration");
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
                }
            }

            Console.WriteLine($"Successfully generated {terminals.Count()} contacts in: {outputPath}");
        }
    }
}