using Clero.Actions;

namespace Clero;

public class Robot
{
    // TODO: Encapsulate the room, position, direction and battery level to enhance readability.
    
    private readonly CellKind[,] _room;
    private readonly IBackOffStrategy _backOffStrategy;
    
    private bool _isBackingOff;

    public Position Position { get; private set; }

    public Direction Direction { get; private set; }

    public int BatteryLevel { get; private set; }

    public Robot(CellKind[,] room, IBackOffStrategy backOffStrategy, Position initialPosition, Direction initialDirection, int initialBatteryLevel)
    {
        _room = room;
        _backOffStrategy = backOffStrategy;
        Position = initialPosition;
        Direction = initialDirection;
        BatteryLevel = initialBatteryLevel;
        
        if (_room[Position.Y, Position.X] != CellKind.DirtyUnvisited)
        {
            throw new ArgumentException("Initial position must be dirty and unvisited.");
        }
        
        _room[Position.Y, Position.X] = CellKind.DirtyVisited;
    }

    private void LogAction(string action, ActionResult status)
    {
        // TODO: Use DI to avoid logging race conditions in parallel tests.
        Log.Verbose(
            "{action}: {status} | [{x}, {y}] > {direction} / bat {battery}",
            action,
            status,
            Position.X,
            Position.Y,
            Direction.ToString()[0],
            BatteryLevel);
    }
    
    private ActionResult LogAndReturn(string action, ActionResult status)
    {
        LogAction(action, status);
        return status;
    }
    
    private bool ConsumeEnergy(int energy)
    {
        if (BatteryLevel < energy)
        {
            return false;
        }
        
        BatteryLevel -= energy;

        return true;
    }

    public ActionResult TurnLeft()
    {
        const string action = "TL";
        
        if (!ConsumeEnergy(1))
        {
            return LogAndReturn(action, ActionResult.OutOfBattery);
        }
        
        Direction = Direction switch
        {
            Direction.North => Direction.West,
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South,
            _ => throw new ArgumentOutOfRangeException()
        };

        return LogAndReturn(action, ActionResult.Success);
    }
    
    public ActionResult TurnRight()
    {
        const string action = "TR";
        
        if (!ConsumeEnergy(1))
        {
            return LogAndReturn(action, ActionResult.OutOfBattery);
        }
        
        Direction = Direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return LogAndReturn(action, ActionResult.Success);
    }
    
    private ActionResult MoveTo(Position newPosition, int energyRequired)
    {
        if (!ConsumeEnergy(energyRequired))
        {
            return ActionResult.OutOfBattery;
        }

        var isObstacleHit = false;
        
        if (newPosition.X < 0
            || newPosition.X >= _room.GetLength(1)
            || newPosition.Y < 0
            || newPosition.Y >= _room.GetLength(0))
        {
            isObstacleHit = true;
        }
        else if (_room[newPosition.Y, newPosition.X] == CellKind.Obstacle)
        {
            isObstacleHit = true;
        }
        
        if (isObstacleHit)
        {
            if (_isBackingOff)
            {
                return ActionResult.Obstacle;
            }

            Log.Verbose("Backing off...");
            _isBackingOff = true;

            try
            {
                var backOffResult = _backOffStrategy.BackOff(this);
                Log.Verbose("Backed off: {result}", backOffResult);
                return backOffResult;
            }
            finally
            {
                _isBackingOff = false;
            }
        }
        
        Position = newPosition;
        
        if (_room[Position.Y, Position.X] == CellKind.DirtyUnvisited)
        {
            _room[Position.Y, Position.X] = CellKind.DirtyVisited;
        }

        return ActionResult.Success;
    }
    
    public ActionResult Advance()
    {
        var newPosition = Direction switch
        {
            Direction.North => Position with { Y = Position.Y - 1 },
            Direction.East => Position with { X = Position.X + 1 },
            Direction.South => Position with { Y = Position.Y + 1 },
            Direction.West => Position with { X = Position.X - 1 },
            _ => throw new ArgumentOutOfRangeException()
        };

        return LogAndReturn("A", MoveTo(newPosition, 2));
    }

    public ActionResult Back()
    {
        var newPosition = Direction switch
        {
            Direction.North => Position with { Y = Position.Y + 1 },
            Direction.East => Position with { X = Position.X - 1 },
            Direction.South => Position with { Y = Position.Y - 1 },
            Direction.West => Position with { X = Position.X + 1 },
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return LogAndReturn("B", MoveTo(newPosition, 3));
    }

    public ActionResult Clean()
    {
        const string action = "C";
        
        if (!ConsumeEnergy(5))
        {
            return LogAndReturn(action, ActionResult.OutOfBattery);
        }
        
        _room[Position.Y, Position.X] = CellKind.Clean;
        
        return LogAndReturn("C", ActionResult.Success);
    }

    public ActionResult Execute(IEnumerable<RobotAction> sequence)
    {
        foreach (var action in sequence)
        {
            var result = action(this);

            if (result != ActionResult.Success)
            {
                return result;
            }
        }
        
        return ActionResult.Success;
    }
}