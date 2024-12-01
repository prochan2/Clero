namespace Clero.UnitTests.Services;

public class ThrowingBackOffStrategy : IBackOffStrategy
{
    public ActionResult BackOff(Robot robot)
    {
        throw new InvalidOperationException("Unexpected back off.");
    }
}