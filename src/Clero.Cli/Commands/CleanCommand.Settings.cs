using System.ComponentModel;

namespace Clero.Cli.Commands;

internal partial class CleanCommand
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class CleanCommandSettings : CommandSettings
    {
        [Description("Path to the source file")]
        [CommandArgument(0, "<source>")]
        public string SourceFilePath { get; set; } = null!;

        [Description("Path to the result file")]
        [CommandArgument(1, "<result>")]
        public string ResultFilePath { get; set; } = null!;

        [Description("Enable verbose output")]
        [CommandOption("-v|--verbose")]
        public bool Verbose { get; set; }
    }
}