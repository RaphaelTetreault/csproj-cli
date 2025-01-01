using CommandLine;
using Microsoft.Build.Construction;

namespace CSProjectCLI;

internal class Program
{
    static void Main(string[] args)
    {
        // Run program with options
        var parseResult = Parser.Default.ParseArguments<Options>(args)
            .WithParsed(ExecuteAction);
    }

    private static void ExecuteAction(Options options)
    {
        // Get single or multiple .csproj files
        string[] paths = GetCsprojFilePaths(options);

        // Iterate over path or paths
        foreach (string path in paths)
        {
            // Force path to that which has been gathered
            options.Path = path;

            // Load CSProj
            var csproj = ProjectRootElement.Open(path);

            // Do action
            switch (options.Action.ToLower())
            {
                case "modify-property": ModifyProperty(csproj, options); break;
                case "bump-version-major": BumpVersionMajor(csproj, options); break;
                case "bump-version-minor": BumpVersionMinor(csproj, options); break;
                case "bump-version-patch": BumpVersionPatch(csproj, options); break;
                default: throw new Exception();
            };
        }
    }

    private static void ModifyProperty(ProjectRootElement csproj, Options options)
    {
        bool matchFound = PropertyExists(csproj, options, out string existingValue);
        if (matchFound)
        {
            Console.WriteLine($"{options.Name}: \"{existingValue}\" -> \"{options.Value}\"");
            csproj.AddProperty(options.Name, options.Value);
            csproj.Save();
            Console.WriteLine($"{options.Path} updated and saved.");
        }
        else
        {
            Console.WriteLine($"Property {options.Name} not found.");
            for (int i = 0; i < csproj.Properties.Count; i++)
            {
                ProjectPropertyElement element = csproj.Properties.ElementAt(i);
                Console.WriteLine($"Property {i + 1}:\t\"{element.Name}\" = \"{element.Value}\"");

            }
        }
    }

    private static void BumpVersion(ProjectRootElement csproj, Options options, int index)
    {
        string[] kVersioningParts = ["MAJOR", "MINOR", "PATCH"];
        options.Name = "Version"; // property name is "Version"

        bool matchFound = PropertyExists(csproj, options, out string existingValue);
        if (matchFound)
        {
            // Ensure we are modifying value in range
            if (index >= kVersioningParts.Length)
            {
                Console.WriteLine($"Index value {index} is larger than max {kVersioningParts.Length}");
                return;
            }

            // Get version string in pieces (separate periods)
            string[] versionElements = existingValue.Split(".");

            // Validate length
            bool isCorrectFormat = versionElements.Length == kVersioningParts.Length;
            if (!isCorrectFormat)
            {
                Console.WriteLine($"Version not formatted as " +
                    $"{versionElements[0]}." +
                    $"{versionElements[1]}." +
                    $"{versionElements[2]}");
                return;
            }

            // Validate values
            for (int i = 0; i < versionElements.Length; i++)
            {
                string versionElement = versionElements[i];
                bool isNumber = int.TryParse(versionElement, out _);
                if (!isNumber)
                {
                    Console.WriteLine($"Version {kVersioningParts[i]} is not a number: \"{versionElement}\"");
                    return;
                }
            }

            // Parse, Increment, Reconstruct
            int versionNumber = int.Parse(versionElements[index]);
            versionElements[index] = (versionNumber + 1).ToString();
            string versionValue =
                $"{versionElements[0]}." +
                $"{versionElements[1]}." +
                $"{versionElements[2]}";

            // Assign value and print report
            Console.WriteLine($"{options.Name}: \"{existingValue}\" -> \"{versionValue}\"");
            csproj.AddProperty(options.Name, versionValue);
            csproj.Save();
            Console.WriteLine($"{options.Path} updated and saved.");
        }
        else
        {
            Console.WriteLine($"Did not find property \"{options.Value}\".");
        }
    }

    private static void BumpVersionMajor(ProjectRootElement csproj, Options options)
        => BumpVersion(csproj, options, 0);

    private static void BumpVersionMinor(ProjectRootElement csproj, Options options)
        => BumpVersion(csproj, options, 1);

    private static void BumpVersionPatch(ProjectRootElement csproj, Options options)
        => BumpVersion(csproj, options, 2);

    private static bool PropertyExists(ProjectRootElement csproj, Options options, out string existingValue)
    {
        for (int i = 0; i < csproj.Properties.Count; i++)
        {
            ProjectPropertyElement element = csproj.Properties.ElementAt(i);
            if (element.Name == options.Name)
            {
                existingValue = element.Value;
                return true;
            }
        }
        existingValue = string.Empty;
        return false;
    }

    private static string[] GetCsprojFilePaths(Options options)
    {
        string[] path = [];
        bool isPathFile = File.Exists(options.Path);
        bool isPathDir = Directory.Exists(options.Path);
        if (isPathFile)
            path = [options.Path];
        else if (isPathDir)
            path = Directory.GetFiles(options.Path, "*.csproj", SearchOption.AllDirectories);
        return path;
    }
}
