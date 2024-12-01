using Clero.UnitTests.Services;

namespace Clero.UnitTests;

public class RobotActionsTests
{
    private static Robot CreateTurnTestRobot(
        Direction initialDirection = Direction.North,
        int initialBatteryLevel = 1)
        => new(new[,] { { CellKind.Dirty } }, new ThrowingBackOffStrategy(), new(0, 0),
            initialDirection, initialBatteryLevel);
    
    [Theory]
    [InlineData(Direction.North, Direction.West)]
    [InlineData(Direction.West, Direction.South)]
    [InlineData(Direction.South, Direction.East)]
    [InlineData(Direction.East, Direction.North)]
    public void TurnsLeft(Direction initialDirection, Direction expectedDirection)
    {
        var robot = CreateTurnTestRobot(initialDirection);
        robot.TurnLeft().ShouldBe(ActionResult.Success);
        robot.Direction.ShouldBe(expectedDirection);
    }
    
    [Theory]
    [InlineData(Direction.North, Direction.East)]
    [InlineData(Direction.East, Direction.South)]
    [InlineData(Direction.South, Direction.West)]
    [InlineData(Direction.West, Direction.North)]
    public void TurnsRight(Direction initialDirection, Direction expectedDirection)
    {
        var robot = CreateTurnTestRobot(initialDirection);
        robot.TurnRight().ShouldBe(ActionResult.Success);
        robot.Direction.ShouldBe(expectedDirection);
    }
    
    [Fact]
    public void TurnsLeftOutOfBattery()
    {
        var robot = CreateTurnTestRobot(initialBatteryLevel: 0);
        robot.TurnLeft().ShouldBe(ActionResult.OutOfBattery);
    }
    
    [Fact]
    public void TurnsRightOutOfBattery()
    {
        var robot = CreateTurnTestRobot(initialBatteryLevel: 0);
        robot.TurnRight().ShouldBe(ActionResult.OutOfBattery);
    }

    private static Robot CreateMoveTestRobot(
        CellKind[,] room,
        Position initialPosition,
        Direction initialDirection,
        int initialBatteryLevel)
        => new(room, new ThrowingBackOffStrategy(), initialPosition, initialDirection, initialBatteryLevel);
    
    [Theory]
    [InlineData(Direction.North)]
    [InlineData(Direction.East)]
    [InlineData(Direction.South)]
    [InlineData(Direction.West)]
    public void Advances(Direction initialDirection)
    {
        var room = initialDirection switch
        {
            Direction.North or Direction.South => new[,] { { CellKind.Dirty }, { CellKind.Dirty } },
            Direction.East or Direction.West => new[,] { { CellKind.Dirty, CellKind.Dirty } },
            _ => throw new ArgumentOutOfRangeException()
        };

        var initialPosition = initialDirection switch
        {
            Direction.South or Direction.East => new Position(0, 0),
            Direction.North => new Position(0, 1),
            Direction.West => new Position(1, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var expectedPosition = initialDirection switch
        {
            Direction.North => new Position(0, 0),
            Direction.East => new Position(1, 0),
            Direction.South => new Position(0, 1),
            Direction.West => new Position(0, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var robot = CreateMoveTestRobot(room, initialPosition, initialDirection, 2);
        robot.Advance().ShouldBe(ActionResult.Success);
        robot.Position.ShouldBe(expectedPosition);
    }
}