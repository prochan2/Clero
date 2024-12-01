namespace Clero;

public class Robot
{
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
        if (!ConsumeEnergy(1))
        {
            return ActionResult.OutOfBattery;
        }
        
        Direction = Direction switch
        {
            Direction.North => Direction.West,
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South,
            _ => throw new ArgumentOutOfRangeException()
        };

        return ActionResult.Success;
    }
    
    public ActionResult TurnRight()
    {
        if (!ConsumeEnergy(1))
        {
            return ActionResult.OutOfBattery;
        }
        
        Direction = Direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return ActionResult.Success;
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

            _isBackingOff = true;
            
            var backOffResult = _backOffStrategy.BackOff(this);

            _isBackingOff = false;

            return backOffResult;
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
        
        return MoveTo(newPosition, 2);
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
        
        return MoveTo(newPosition, 3);
    }

    public ActionResult Clean()
    {
        if (!ConsumeEnergy(5))
        {
            return ActionResult.OutOfBattery;
        }
        
        _room[Position.Y, Position.X] = CellKind.Clean;
        
        return ActionResult.Success;
    }
}