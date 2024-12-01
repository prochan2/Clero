using Clero.Actions;
using Clero.UnitTests.Services;
using Xunit.Abstractions;

namespace Clero.UnitTests.RobotTests;

public class RobotOutOfBatteryTests : TestBase
{
    public RobotOutOfBatteryTests(ITestOutputHelper output) : base(output) { }

    private static void TestActionOutOfBattery(int initialBatteryLevel, RobotAction action)
    {
        // Not running out of battery would cause hitting an obstacle when moving.
        var room = new[,] { { CellKind.DirtyUnvisited } };
        var initialPosition = new Position(0, 0);
        
        var robot = new Robot(
            room,
            new ThrowingBackOffStrategy(),
            initialPosition,
            Direction.North,
            initialBatteryLevel);

        action(robot).ShouldBe(ActionResult.OutOfBattery);
        room[0, 0].ShouldBe(CellKind.DirtyVisited);
        robot.Position.ShouldBe(initialPosition);
        robot.BatteryLevel.ShouldBe(initialBatteryLevel);
    }
    
    [Fact]
    public void FailsTurningLeftOutOfBattery() =>
        TestActionOutOfBattery(0, RobotActions.TurnLeft);
    
    [Fact]
    public void FailsTurningRightOutOfBattery() =>
        TestActionOutOfBattery(0, RobotActions.TurnRight);
    
    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void FailsAdvancingOutOfBattery(int initialBatteryLevel) =>
        TestActionOutOfBattery(initialBatteryLevel, RobotActions.Advance);
    
    [Theory]
    [InlineData(2)]
    [InlineData(1)]
    [InlineData(0)]
    public void FailsBackingOutOfBattery(int initialBatteryLevel) =>
        TestActionOutOfBattery(initialBatteryLevel, RobotActions.Back);
    
    [Theory]
    [InlineData(4)]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    [InlineData(0)]
    public void FailsCleaningOutOfBattery(int initialBatteryLevel) =>
        TestActionOutOfBattery(initialBatteryLevel, RobotActions.Clean);
}