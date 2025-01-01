using CommandLine;

namespace CSProjectCLI;

public class Options
{
    public static class Args
    {
        public const string Action = "action";
        public const string Name = "name";
        public const string Path = "path";
        public const string Value = "value";
    }

    public static class Help
    {
        public const string Action = "The action to perform.";
        public const string Name = "The name of the XML item.";
        public const string Path = "The path (file or directory) of the .csproj file.";
        public const string Value = "The value of the XML item.";
    }

    [Value(0, MetaName = Args.Path, HelpText = Help.Path, Required = true)]
    public string Path { get; set; } = string.Empty;

    [Value(1, MetaName = Args.Action, HelpText = Help.Action, Required = true)]
    public string Action { get; set; } = string.Empty;

    [Option(Args.Name, HelpText = Help.Name)]
    public string Name { get; set; } = string.Empty;

    [Option(Args.Value, HelpText = Help.Value)]
    public string Value { get; set; } = string.Empty;
}
