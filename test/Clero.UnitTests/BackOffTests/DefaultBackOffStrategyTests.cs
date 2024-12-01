using Xunit.Abstractions;

namespace Clero.UnitTests.BackOffTests;

public class DefaultBackOffStrategyTests : TestBase
{
    public DefaultBackOffStrategyTests(ITestOutputHelper output) : base(output) { }

    private static void Test(
        CellKind[,] room,
        Position initialPosition,
        Direction initialDirection,
        Position expectedPosition,
        Direction expectedDirection,
        int expectedBatteryLevel)
    {
        var robot = new Robot(room, new DefaultBackOffStrategy(), initialPosition, initialDirection, 100);
        robot.Advance().ShouldBe(ActionResult.Success);
        robot.Position.ShouldBe(expectedPosition);
        robot.Direction.ShouldBe(expectedDirection);
        robot.BatteryLevel.ShouldBe(expectedBatteryLevel);
    }
    
    [Fact]
    public void Sequence1SavesTheDay()
    {
        var room = new[,] { { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited } };
        var initialPosition = new Position(0, 0);
        var initialDirection = Direction.North;
        var expectedPosition = new Position(1, 0);
        var expectedDirection = Direction.North;
        var expectedBatteryLevel = 94;
        Test(room, initialPosition, initialDirection, expectedPosition, expectedDirection, expectedBatteryLevel);
    }
}