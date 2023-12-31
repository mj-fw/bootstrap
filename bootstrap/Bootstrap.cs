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
        
        // Check if we are running as root or via Sudo
        await CheckIfRunningAsRootOrSudo();
        
        Log.Information("Preflight checks complete.");
    }
    
    private Task CheckIfRunningAsRootOrSudo()
    {
        // Check if we are running as root via env USER
        if (!string.Equals(Environment.GetEnvironmentVariable("USER"), "root", StringComparison.OrdinalIgnoreCase) &&
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUDO_USER")))
        {
            Log.Fatal("\t❌ This bootstrap needs to be run as root or via Sudo.");
            Environment.Exit(1);
        }
        
        Log.Information("\t✅ Running as root or via Sudo.");
        
        return Task.CompletedTask;
    }
    
    private async Task CheckOsReleaseInfo()
    {
        // Get OS release information
        var osReleaseInfo = await GetOsReleaseInfo();
        
        // Make sure we are running on Ubuntu
        if (!string.Equals(osReleaseInfo["ID"], "ubuntu", StringComparison.OrdinalIgnoreCase))
        {
            Log.Fatal("\t❌ This bootstrap is only supported on Ubuntu.");
            Environment.Exit(1);
        }
        
        // Make sure we are running on Ubuntu 20.04 or later
        if (double.TryParse(osReleaseInfo["VERSION_ID"].Trim('"'), out var version))
        {
            if (version < 20.04)
            {
                Log.Fatal("\t❌ This bootstrap is only supported on Ubuntu 20.04 or later.");
                Environment.Exit(1);
            }
        }
        else
        {
            Log.Fatal("\t❌ Unable to parse Ubuntu version.");
            Environment.Exit(1);
        }
        
        Log.Information("\t✅ Running on supported Ubuntu version.");
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
            Log.Fatal("\t❌ /etc/os-release does not exist.");
            Environment.Exit(1);
        }
        
        return osReleaseInfo;
    }
}