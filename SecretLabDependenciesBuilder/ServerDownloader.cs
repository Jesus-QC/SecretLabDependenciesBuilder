using System.Diagnostics;
using System.Text;

namespace SecretLabDependenciesBuilder;

public static class ServerDownloader
{
    private static string? _beta;
    private static string? _betaPassword;
    
    public static async Task RunAsync()
    {
        RepeatFromBeginning:
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Write the server branch or beta name, let it blank for none.");
        Console.ForegroundColor = ConsoleColor.White;
        _beta = Console.ReadLine();

        if (!string.IsNullOrEmpty(_beta))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Write the server branch or beta password, let it blank for none.");
            Console.ForegroundColor = ConsoleColor.White;
            _betaPassword = Console.ReadLine();
        }
        
        RepeatDecision:
        Console.WriteLine("\nIs the provided data correct?");
        Console.WriteLine("Beta: " + (string.IsNullOrEmpty(_beta) ? "NONE" : $"\"{_beta}\""));
        Console.WriteLine("Password: " + (string.IsNullOrEmpty(_betaPassword) ? "NONE" : $"\"{_betaPassword}\""));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Respond with yes (y) or no (n). Default: yes");
        Console.ForegroundColor = ConsoleColor.White;

        string? decision = Console.ReadLine();
        if (!string.IsNullOrEmpty(decision))
        {
            if (decision is "n" or "no")
            {
                Console.Clear();
                goto RepeatFromBeginning;
            }
            
            if (decision is not ("y" or "yes"))
                goto RepeatDecision;
        }

        DirectoryInfo installationDirectory = new (Path.Combine(Environment.CurrentDirectory, "temp"));
        installationDirectory.Create();
        string filesPath = Path.Combine(installationDirectory.FullName, "files.txt");
        await File.WriteAllTextAsync(filesPath, "regex:SCPSL_Data/Managed/*");

        StringBuilder cmd = new ($"depotdownloader/DepotDownloader.dll -app 996560 -filelist \"{filesPath}\" -dir \"{installationDirectory.FullName}\" ");

        if (!string.IsNullOrEmpty(_beta))
        {
            cmd.Append("-beta " + _beta);

            if (!string.IsNullOrEmpty(_betaPassword))
                cmd.Append("-betapassword " + _betaPassword);
        }
        
        ProcessStartInfo pc = new ()
        {
            FileName = "dotnet",
            Arguments = cmd.ToString(),
            RedirectStandardInput = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            WorkingDirectory = Environment.CurrentDirectory
        };

        Process? process = Process.Start(pc);
        
        if (process is null)
            throw new Exception("An error occurred while instantiating the download process. Make sure you have dotnet 6 runtime installed.");
        
        while (!process.HasExited)
        {
            Console.WriteLine(await process.StandardOutput.ReadLineAsync());
        }
        
        AssembliesPublicizer.RunPublicizer(installationDirectory.GetDirectories()[1].GetDirectories()[0]);
        
        installationDirectory.Delete(true);
    }
}