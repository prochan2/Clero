namespace Clero.Cli.Commands;

internal partial class CleanCommand
{
    internal sealed class CleanCommandSettings : CommandSettings
    {
        [CommandArgument(0, "<source>")]
        public string SourceFilePath { get; set; } = null!;
        
        [CommandArgument(1, "<result>")]
        public string ResultFilePath { get; set; } = null!;
    }
}