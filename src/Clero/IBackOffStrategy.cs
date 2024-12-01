namespace Clero;

public interface IBackOffStrategy
{
    ActionResult BackOff(Robot robot);
}