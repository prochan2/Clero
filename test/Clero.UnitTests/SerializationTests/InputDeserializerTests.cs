using System.Text;
using Clero.Serialization;

namespace Clero.UnitTests.SerializationTests;

public class InputDeserializerTests
{
    [Fact]
    public void InputIsDeserialized()
    {
        const string input = """
                             {
                               "map": [
                                 ["S", "S", "S", "S"],
                                 ["S", "S", "C", "S"],
                                 ["S", "S", "S", "S"],
                                 ["S", "null", "S", "S"]
                               ],
                               "start": {"X": 3, "Y": 1, "facing": "S"},
                               "commands": [ "TR","A","C","A","C","TR","A","C"],
                               "battery": 1094
                             }
                             """;
        
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
        
        var deserializer = new InputDeserializer();

        var deserialized = deserializer.Deserialize(stream);
        
        deserialized.Room.ShouldBe(new[,]
        {
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited }
        });

        deserialized.Position.ShouldBe(new(3, 1));
        deserialized.Direction.ShouldBe(Direction.South);
        deserialized.Commands.ShouldBe(new[] { "TR", "A", "C", "A", "C", "TR", "A", "C" });
        deserialized.BatteryLevel.ShouldBe(1094);
    }
    
    // TODO: Add more tests
}