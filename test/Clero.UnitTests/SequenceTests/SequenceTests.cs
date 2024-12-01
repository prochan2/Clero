using Clero.Actions;

namespace Clero.UnitTests.SequenceTests;

public class SequenceTests
{
    [Fact]
    public void Test1SequencePasses()
    {
        var room = new[,]
        {
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited }
        };
        
        var initialPosition = new Position(3, 0);
        var initialDirection = Direction.North;
        var initialBatteryLevel = 80;

        RobotAction[] commands =
        [
            RobotActions.TurnLeft,
            RobotActions.Advance,
            RobotActions.Clean,
            RobotActions.Advance,
            RobotActions.Clean,
            RobotActions.TurnRight,
            RobotActions.Advance,
            RobotActions.Clean
        ];
        
        var robot = new Robot(room, new DefaultBackOffStrategy(), initialPosition, initialDirection, initialBatteryLevel);
        
        robot.Execute(commands).ShouldBe(ActionResult.Success);
        
        room.ShouldBe(new[,]
        {
            { CellKind.DirtyUnvisited, CellKind.Clean, CellKind.Clean, CellKind.DirtyVisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited }
        });

        robot.Position.ShouldBe(new(2, 0));
        robot.Direction.ShouldBe(Direction.North);
        robot.BatteryLevel.ShouldBe(53);
    }
}