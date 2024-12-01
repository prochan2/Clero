namespace Clero.UnitTests.Services;

internal sealed class NoOpBackOffStrategy : IBackOffStrategy
{
    public ActionResult BackOff(Robot robot) => ActionResult.Obstacle;
}