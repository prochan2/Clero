namespace Clero.Actions;

public static class RobotTurn
{
    public static RobotAction TurnLeft = r => r.TurnLeft();
    
    public static RobotAction TurnRight = r => r.TurnRight();
    
    public static RobotAction Advance = r => r.Advance();
    
    public static RobotAction Back = r => r.Back();
    
    public static RobotAction Clean = r => r.Clean();
}