using Serilog;
using Xunit.Abstractions;

namespace Clero.UnitTests;

public abstract class TestBase
{
    protected TestBase(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.TestOutput(output)
            .CreateLogger();
    }
}