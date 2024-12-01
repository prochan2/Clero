using Clero.Actions;

namespace Clero;

public sealed class DefaultBackOffStrategy : IBackOffStrategy
{
    private readonly RobotAction[][] _backOffActions =
    [
        [RobotActions.TurnRight, RobotActions.Advance, RobotActions.TurnLeft],
        [RobotActions.TurnRight, RobotActions.Advance, RobotActions.TurnRight],
        [RobotActions.TurnRight, RobotActions.Advance, RobotActions.TurnRight],
        [RobotActions.TurnRight, RobotActions.Back, RobotActions.TurnRight, RobotActions.Advance],
        [RobotActions.TurnLeft, RobotActions.TurnLeft, RobotActions.Advance]
    ];
    
    public ActionResult BackOff(Robot robot)
    {
        foreach (var actions in _backOffActions)
        {
            ActionResult result = ActionResult.Unknown;
            
            foreach (var action in actions)
            {
                result = action(robot);

                switch (result)
                {
                    case ActionResult.Success:
                        continue;
                    
                    case ActionResult.OutOfBattery:
                    case ActionResult.Obstacle:
                        break;
                    
                    default:
                        throw new InvalidOperationException();
                }
            }

            switch (result)
            {
                case ActionResult.Success:
                case ActionResult.OutOfBattery:
                    return result;
                
                case ActionResult.Obstacle:
                    continue;
                
                default:
                    throw new InvalidOperationException();
            }
        }
        
        return ActionResult.Obstacle;
    }
}