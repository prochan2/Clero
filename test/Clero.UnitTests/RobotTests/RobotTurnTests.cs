using Clero.Actions;
using Clero.UnitTests.Services;
using Xunit.Abstractions;

namespace Clero.UnitTests.RobotTests;

public class RobotTurnTests : TestBase
{
    public RobotTurnTests(ITestOutputHelper output) : base(output) { }

    private static Robot CreateTestRobot(
        Direction initialDirection = Direction.North,
        int initialBatteryLevel = 1)
        => new(new[,] { { CellKind.DirtyUnvisited } }, new ThrowingBackOffStrategy(), new(0, 0),
            initialDirection, initialBatteryLevel);
    
    private static void TestTurn(Direction initialDirection, Direction expectedDirection, RobotAction action)
    {
        var robot = CreateTestRobot(initialDirection);
        action(robot).ShouldBe(ActionResult.Success);
        robot.Direction.ShouldBe(expectedDirection);
        robot.Position.ShouldBe(new(0, 0));
        robot.BatteryLevel.ShouldBe(0);
    }

    [Theory]
    [InlineData(Direction.North, Direction.West)]
    [InlineData(Direction.West, Direction.South)]
    [InlineData(Direction.South, Direction.East)]
    [InlineData(Direction.East, Direction.North)]
    public void TurnsLeft(Direction initialDirection, Direction expectedDirection) =>
        TestTurn(initialDirection, expectedDirection, RobotActions.TurnLeft);

    [Theory]
    [InlineData(Direction.North, Direction.East)]
    [InlineData(Direction.East, Direction.South)]
    [InlineData(Direction.South, Direction.West)]
    [InlineData(Direction.West, Direction.North)]
    public void TurnsRight(Direction initialDirection, Direction expectedDirection) =>
        TestTurn(initialDirection, expectedDirection, RobotActions.TurnRight);
}