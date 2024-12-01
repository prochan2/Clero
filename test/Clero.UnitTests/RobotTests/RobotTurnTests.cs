using Clero.UnitTests.Services;

namespace Clero.UnitTests.RobotTests;

public class RobotTurnTests
{
    private static Robot CreateTestRobot(
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
        var robot = CreateTestRobot(initialDirection);
        robot.TurnLeft().ShouldBe(ActionResult.Success);
        robot.Direction.ShouldBe(expectedDirection);
        robot.Position.ShouldBe(new(0, 0));
        robot.BatteryLevel.ShouldBe(0);
    }
    
    [Theory]
    [InlineData(Direction.North, Direction.East)]
    [InlineData(Direction.East, Direction.South)]
    [InlineData(Direction.South, Direction.West)]
    [InlineData(Direction.West, Direction.North)]
    public void TurnsRight(Direction initialDirection, Direction expectedDirection)
    {
        var robot = CreateTestRobot(initialDirection);
        robot.TurnRight().ShouldBe(ActionResult.Success);
        robot.Direction.ShouldBe(expectedDirection);
        robot.Position.ShouldBe(new(0, 0));
        robot.BatteryLevel.ShouldBe(0);
    }
    
    [Fact]
    public void TurnsLeftOutOfBattery()
    {
        var robot = CreateTestRobot(initialBatteryLevel: 0);
        robot.TurnLeft().ShouldBe(ActionResult.OutOfBattery);
        robot.Position.ShouldBe(new(0, 0));
        robot.BatteryLevel.ShouldBe(0);
    }
    
    [Fact]
    public void TurnsRightOutOfBattery()
    {
        var robot = CreateTestRobot(initialBatteryLevel: 0);
        robot.TurnRight().ShouldBe(ActionResult.OutOfBattery);
        robot.Position.ShouldBe(new(0, 0));
        robot.BatteryLevel.ShouldBe(0);
    }
}