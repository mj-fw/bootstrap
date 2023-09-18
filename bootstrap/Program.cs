using Serilog;

namespace mjFW.Core.Bootstrap;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("mjFW - Bootstrap");
        Console.WriteLine("-----------------");
        Console.WriteLine("Are you sure you want to start the bootstrap process for mjFW, this will perform many changes to the system, do not run this on a system you use for anything else, are you sure you want to proceed? y/N");

        var userInput = Console.ReadLine();
        if (userInput != "y")
        {
            Console.WriteLine("Exiting...");
            return;
        }
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("bootstrap.log")
            .CreateLogger();

        try
        {
            Log.Information("Starting bootstrap process...");
            
            var bootstrap = new Bootstrap();
            await bootstrap.Run();
            
            Log.Information("Bootstrap process completed.");
        }
        catch (Exception e)
        {
            Log.Fatal(e, "An error occurred while bootstrapping the system.");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}