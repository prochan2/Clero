using Clero.Cli.Commands;

var app = new CommandApp<CleanCommand>();

app.Configure(config =>
{
#if DEBUG
    config.ValidateExamples();
#endif

    config
        .AddCommand<CleanCommand>("clean")
        .WithAlias("c")
        .WithDescription("Clean the room")
        .WithExample("clean", "source.json", "result.json");
});
    
        
return app.Run(args);