using Clero.Serialization;
using Xunit.Abstractions;

namespace Clero.UnitTests.SerializationTests;

public class OutputSerializerTests : TestBase
{
    public OutputSerializerTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public void OutputIsSerialized()
    {
        var room = new[,]
        {
            { CellKind.DirtyUnvisited, CellKind.Clean, CellKind.Clean, CellKind.DirtyVisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited },
            { CellKind.DirtyUnvisited, CellKind.Obstacle, CellKind.DirtyUnvisited, CellKind.DirtyUnvisited }
        };
        
        var finalPosition = new Position(2, 0);
        var finalDirection = Direction.North;
        var finalBatteryLevel = 53;
        
        using var stream = new MemoryStream();
        var serializer = new OutputSerializer();
        serializer.Serialize(stream, room, finalPosition, finalDirection, finalBatteryLevel);
        
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = reader.ReadToEnd();

        output.ShouldBe("""
                        {
                        "visited" : [{ "X" : 1, "Y" : 0 }{ "X" : 2, "Y" : 0 }{ "X" : 3, "Y" : 0 }],
                        "cleaned" : [{ "X" : 1, "Y" : 0 }{ "X" : 2, "Y" : 0 }],
                        "final" : { "X" : 2, "Y" : 0, "facing" : "N"},
                        "battery" : 53
                        }
                        """);
    }
    
    // TODO: Add more tests
}