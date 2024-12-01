namespace Clero.Serialization;

public class OutputSerializer
{
    public void Serialize(
        Stream stream,
        CellKind[,] map,
        Position finalPosition,
        Direction finalDirection,
        int finalBattery)
    {
        IEnumerable<Position> GetVisited()
        {
            for (var y = 0; y < map.GetLength(0); y++)
            {
                for (var x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] is CellKind.Clean or CellKind.DirtyVisited)
                    {
                        yield return new Position(x, y);
                    }
                }
            }
        }
        
        IEnumerable<Position> GetCleaned()
        {
            for (var y = 0; y < map.GetLength(0); y++)
            {
                for (var x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == CellKind.Clean)
                    {
                        yield return new Position(x, y);
                    }
                }
            }
        }
        
        Serialize(stream, GetVisited(), GetCleaned(), finalPosition, finalDirection, finalBattery);
    }
    
    public void Serialize(
        Stream stream,
        IEnumerable<Position> visited,
        IEnumerable<Position> cleaned,
        Position finalPosition,
        Direction finalDirection,
        int finalBattery)
    {
        // Using StreamWriter instead of Utf8JsonWriter to keep required formatting.
        // Not disposing the writer because it would dispose the stream.
        var writer = new StreamWriter(stream);
        
        void WritePositions(string name, IEnumerable<Position> positions)
        {
            writer.Write($"\"{name}\" : [");
            
            foreach (var position in positions)
            {
                writer.Write($"{{ \"X\" : {position.X}, \"Y\" : {position.Y} }}");
            }
            
            writer.Write("]");
        }
        
        writer.WriteLine("{");
        WritePositions("visited", visited);
        writer.WriteLine(",");
        WritePositions("cleaned", cleaned);
        writer.WriteLine(",");
        writer.WriteLine(
            $"\"final\" : {{ \"X\" : {finalPosition.X}, \"Y\" : {finalPosition.Y}, \"facing\" : \"{finalDirection switch
            {
                Direction.North => "N",
                Direction.East => "E",
                Direction.South => "S",
                Direction.West => "W",
                _ => throw new InvalidOperationException($"Invalid direction: {finalDirection}")
            }}\"}},");
        writer.WriteLine($"\"battery\" : {finalBattery}");
        writer.Write("}");
        
        writer.Flush();
    }
}