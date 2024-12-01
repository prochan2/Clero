using Clero.UnitTests.Services;

namespace Clero.UnitTests.RobotTests;

public class RobotMoveTests
{
    private static Robot CreateTestRobot(
        CellKind[,] room,
        bool throwsOnBackOff,
        Position initialPosition,
        Direction initialDirection,
        int initialBatteryLevel)
        => new(
            room,
            throwsOnBackOff ? new ThrowingBackOffStrategy() : new NoOpBackOffStrategy(),
            initialPosition,
            initialDirection,
            initialBatteryLevel);
    
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
        
        var robot = CreateTestRobot(room, true, initialPosition, initialDirection, 2);
        robot.Advance().ShouldBe(ActionResult.Success);
        robot.Position.ShouldBe(expectedPosition);
        robot.BatteryLevel.ShouldBe(0);
    }
    
    [Theory]
    [InlineData(Direction.North)]
    [InlineData(Direction.East)]
    [InlineData(Direction.South)]
    [InlineData(Direction.West)]
    public void AdvancesToObstacle(Direction initialDirection)
    {
        var room = initialDirection switch
        {
            Direction.North => new[,] { { CellKind.Obstacle }, { CellKind.Dirty } },
            Direction.South => new[,] { { CellKind.Dirty }, { CellKind.Obstacle } },
            Direction.East => new[,] { { CellKind.Dirty, CellKind.Obstacle } },
            Direction.West => new[,] { { CellKind.Obstacle, CellKind.Dirty } },
            _ => throw new ArgumentOutOfRangeException()
        };

        var initialPosition = initialDirection switch
        {
            Direction.South or Direction.East => new Position(0, 0),
            Direction.North => new Position(0, 1),
            Direction.West => new Position(1, 0),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var robot = CreateTestRobot(room, false, initialPosition, initialDirection, 2);
        robot.Advance().ShouldBe(ActionResult.Obstacle);
        robot.Position.ShouldBe(initialPosition);
        robot.BatteryLevel.ShouldBe(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void AdvancesOutOfBattery(int initialBatteryLevel)
    {
        // Not running out of battery would cause hitting an obstacle.
        var initialPosition = new Position(0, 0);
        
        var robot = CreateTestRobot(
            new[,] { { CellKind.Dirty } },
            true,
            initialPosition,
            Direction.North,
            initialBatteryLevel);
        
        robot.Advance().ShouldBe(ActionResult.OutOfBattery);
        robot.Position.ShouldBe(initialPosition);
        robot.BatteryLevel.ShouldBe(initialBatteryLevel);
    }
}