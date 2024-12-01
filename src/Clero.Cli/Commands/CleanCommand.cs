using Clero.Actions;
using Clero.Serialization;
using Serilog;

namespace Clero.Cli.Commands;

[UsedImplicitly]
internal partial class CleanCommand : Command<CleanCommand.CleanCommandSettings>
{
    private void EnableLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();
    }

    public override int Execute(CommandContext context, CleanCommandSettings settings)
    {
        if (settings.Verbose)
        {
            EnableLogging();
        }
        
        (CellKind[,] Room, Position Position, Direction Direction, RobotAction[] Commands, int BatteryLevel) input;
        
        using (var inputFile = File.OpenRead(settings.SourceFilePath))
        {
            input = new InputDeserializer().Deserialize(inputFile);
        }
        
        var robot = new Robot(input.Room, new DefaultBackOffStrategy(), input.Position, input.Direction, input.BatteryLevel);
        var result = robot.Execute(input.Commands);
        
        Log.Verbose("Result: {Result}", result);
        
        using (var outputFile = File.Create(settings.ResultFilePath))
        {
            new OutputSerializer().Serialize(
                outputFile,
                input.Room,
                robot.Position,
                robot.Direction,
                robot.BatteryLevel);
        }

        return 0;
    }
}