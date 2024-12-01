namespace Clero.UnitTests.Services;

internal sealed class ThrowingBackOffStrategy : IBackOffStrategy
{
    public ActionResult BackOff(Robot robot)
    {
        throw new InvalidOperationException("Unexpected back off.");
    }
}