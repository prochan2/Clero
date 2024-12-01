using Clero.Actions;
using Clero.UnitTests.Services;
using Xunit.Abstractions;

namespace Clero.UnitTests.RobotTests;

public class RobotMoveTests : TestBase
{
    public RobotMoveTests(ITestOutputHelper output) : base(output) { }

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
    
    private static void TestMove(
        CellKind[,] room,
        Position initialPosition,
        Direction initialDirection,
        Position expectedPosition,
        RobotAction action)
    {
        var robot = CreateTestRobot(room, true, initialPosition, initialDirection, 2);
        action(robot).ShouldBe(ActionResult.Success);
        robot.Position.ShouldBe(expectedPosition);
        robot.BatteryLevel.ShouldBe(0);
    }
    
    [Theory]
    [InlineData(Direction.North)]
    [InlineData(Direction.East)]
    [InlineData(Direction.South)]
    [InlineData(Direction.West)]
    public void Advances(Direction initialDirection)
    {
        var room = initialDirection switch
        {
            Direction.North or Direction.South => new[,] { { CellKind.DirtyUnvisited }, { CellKind.DirtyUnvisited } },
            Direction.East or Direction.West => new[,] { { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited } },
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
        
        TestMove(room, initialPosition, initialDirection, expectedPosition, RobotActions.Advance);
    }
    
    private static void TestMoveToObstacle(
        CellKind[,] room,
        Position initialPosition,
        Direction initialDirection,
        RobotAction action)
    {
        var robot = CreateTestRobot(room, false, initialPosition, initialDirection, 2);
        action(robot).ShouldBe(ActionResult.Obstacle);
        robot.Position.ShouldBe(initialPosition);
        robot.BatteryLevel.ShouldBe(0);
    }

    [Theory]
    [InlineData(Direction.North, false)]
    [InlineData(Direction.East, false)]
    [InlineData(Direction.South, false)]
    [InlineData(Direction.West, false)]
    [InlineData(Direction.North, true)]
    [InlineData(Direction.East, true)]
    [InlineData(Direction.South, true)]
    [InlineData(Direction.West, true)]
    public void AdvancesToObstacle(Direction initialDirection, bool isWall)
    {
        var room = isWall
            ? new[,] { { CellKind.DirtyUnvisited } }
            : initialDirection switch
            {
                Direction.North => new[,] { { CellKind.Obstacle }, { CellKind.DirtyUnvisited } },
                Direction.South => new[,] { { CellKind.DirtyUnvisited }, { CellKind.Obstacle } },
                Direction.East => new[,] { { CellKind.DirtyUnvisited, CellKind.Obstacle } },
                Direction.West => new[,] { { CellKind.Obstacle, CellKind.DirtyUnvisited } },
                _ => throw new ArgumentOutOfRangeException()
            };

        var initialPosition = isWall
            ? new Position(0, 0)
            : initialDirection switch
            {
                Direction.South or Direction.East => new Position(0, 0),
                Direction.North => new Position(0, 1),
                Direction.West => new Position(1, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        
        TestMoveToObstacle(room, initialPosition, initialDirection, RobotActions.Advance);
    }
}