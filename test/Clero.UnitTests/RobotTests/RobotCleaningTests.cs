using Clero.UnitTests.Services;

namespace Clero.UnitTests.RobotTests;

public class RobotCleaningTests
{
    [Fact]
    public void RobotCleansCell()
    {
        var room = new CellKind[,] { { CellKind.DirtyUnvisited } };
        var robot = new Robot(room, new ThrowingBackOffStrategy(), new(0, 0), Direction.North, 5);
        robot.Clean().ShouldBe(ActionResult.Success);
        room[0, 0].ShouldBe(CellKind.Clean);
        robot.Position.ShouldBe(new(0, 0));
        robot.BatteryLevel.ShouldBe(0);
    }
}