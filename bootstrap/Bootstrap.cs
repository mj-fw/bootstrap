using Serilog;

namespace mjFW.Core.Bootstrap;

public class Bootstrap : IBootstrap
{
    public async Task Run()
    {
        await Task.Delay(1);
        
        await PerformPreflightChecks();
    }
    
    private async Task PerformPreflightChecks()
    {
        Log.Information("Performing preflight checks...");
        
        // Check OS release information
        await CheckOsReleaseInfo();
        
    }

    private async Task CheckOsReleaseInfo()
    {
        // Get OS release information
        var osReleaseInfo = await GetOsReleaseInfo();
        
        // Make sure we are running on Ubuntu
        if (!string.Equals(osReleaseInfo["ID"], "ubuntu", StringComparison.OrdinalIgnoreCase))
        {
            Log.Fatal("This bootstrap is only supported on Ubuntu.");
            Environment.Exit(1);
        }
        
        // Make sure we are running on Ubuntu 20.04 or later
        if (int.TryParse(osReleaseInfo["VERSION_ID"].Trim('.'), out var version))
        {
            if (version < 2004)
            {
                Log.Fatal("This bootstrap is only supported on Ubuntu 20.04 or later.");
                Environment.Exit(1);
            }
        }
        else
        {
            Log.Fatal("Unable to parse Ubuntu version.");
            Environment.Exit(1);
        }
    }

    private async Task<Dictionary<string,string>> GetOsReleaseInfo()
    {
        var osReleaseInfo = new Dictionary<string, string>();
        
        if (File.Exists("/etc/os-release"))
        {
            var osRelease = await File.ReadAllTextAsync("/etc/os-release");
            
            foreach (var line in File.ReadLines("/etc/os-release"))
            {
                var parts = line.Split(new []{'='}, 2);
                if (parts.Length == 2)
                {
                    osReleaseInfo.Add(parts[0].Trim(), parts[1].Trim());
                }
            }
        }
        else
        {
            Log.Fatal("/etc/os-release does not exist.");
            Environment.Exit(1);
        }
        
        return osReleaseInfo;
    }
}