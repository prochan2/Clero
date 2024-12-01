using System.Text.Json;
using Clero.Actions;

namespace Clero.Serialization;

public class InputDeserializer
{
    public (CellKind[,] Room, Position Position, Direction Direction, RobotAction[] Commands, int BatteryLevel) Deserialize(Stream stream)
    {
        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;

        var map = root.GetProperty("map");
        
        var rows = new List<List<CellKind>>();
        List<CellKind>? previousRow = null;

        foreach (var row in map.EnumerateArray())
        {
            var rowValues = new List<CellKind>();

            foreach (var cell in row.EnumerateArray())
            {
                var cellString = cell.GetString();

                var cellValue = cellString switch
                {
                    "S" => CellKind.DirtyUnvisited,
                    "C" or "null" => CellKind.Obstacle,
                    _ => throw new InvalidOperationException($"Invalid cell value: {cellString}")
                };
                
                rowValues.Add(cellValue);
            }
            
            if (previousRow != null && rowValues.Count != previousRow.Count)
            {
                throw new InvalidOperationException("All rows must have the same number of cells");
            }
            
            previousRow = rowValues;
            rows.Add(rowValues);
        }
        
        var room = new CellKind[rows[0].Count, rows.Count];
        
        for (var y = 0; y < rows.Count; y++)
        {
            for (var x = 0; x < rows[y].Count; x++)
            {
                room[y, x] = rows[y][x];
            }
        }

        var start = root.GetProperty("start");
        var startX = start.GetProperty("X").GetInt32();
        var startY = start.GetProperty("Y").GetInt32();
        var startDirection = start.GetProperty("facing").GetString() switch
        {
            "N" => Direction.North,
            "E" => Direction.East,
            "S" => Direction.South,
            "W" => Direction.West,
            _ => throw new InvalidOperationException("Invalid direction")
        };

        var commands = root.GetProperty("commands").EnumerateArray()
            .Select(c => c.GetString() switch
            {
                "TR" => RobotActions.TurnRight,
                "TL" => RobotActions.TurnLeft,
                "A" => RobotActions.Advance,
                "B" => RobotActions.Back,
                "C" => RobotActions.Clean,
                _ => throw new InvalidOperationException("Invalid command")
            }).ToArray();
        
        var batteryLevel = root.GetProperty("battery").GetInt32();

        return (room, new(startX, startY), startDirection, commands, batteryLevel);
    }
}