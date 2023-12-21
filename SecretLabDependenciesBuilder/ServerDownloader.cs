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
        Console.Clear();
        ConsoleWriter.WriteTitle();
        ConsoleWriter.Write("Enter the server branch or beta name, leave it blank for none.", ConsoleColor.Yellow);

        _beta = Console.ReadLine();

        if (!string.IsNullOrEmpty(_beta))
        {
            ConsoleWriter.Write("Enter the server branch or beta password, leave it blank for none.",
                ConsoleColor.Yellow);
            _betaPassword = Console.ReadLine();
        }

        RepeatDecision:
        ConsoleWriter.Write("\nIs the provided data correct?", ConsoleColor.Yellow);
        ConsoleWriter.Write("Beta: " + (string.IsNullOrEmpty(_beta) ? "NONE" : $"\"{_beta}\""));
        ConsoleWriter.Write("Password: " + (string.IsNullOrEmpty(_betaPassword) ? "NONE" : $"\"{_betaPassword}\""));
        ConsoleWriter.Write("Respond with yes (y) or no (n). Default: yes", ConsoleColor.Yellow);

        string? decision = Console.ReadLine()?.ToLower();
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

        DirectoryInfo installationDirectory = new(Path.Combine(Environment.CurrentDirectory, "temp"));
        installationDirectory.Create();
        string filesPath = Path.Combine(installationDirectory.FullName, "files.txt");
        await File.WriteAllTextAsync(filesPath, "regex:SCPSL_Data/Managed/*");

        StringBuilder cmd =
            new(
                $"depotdownloader/DepotDownloader.dll -app 996560 -filelist \"{filesPath}\" -dir \"{installationDirectory.FullName}\" ");

        if (!string.IsNullOrEmpty(_beta))
        {
            cmd.Append("-beta " + _beta);

            if (!string.IsNullOrEmpty(_betaPassword))
                cmd.Append("-betapassword " + _betaPassword);
        }

        ProcessStartInfo pc = new()
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
            throw new Exception(
                "An error occurred while instantiating the download process. Make sure you have dotnet 6 runtime installed.");

        while (!process.HasExited)
        {
            ConsoleWriter.Write(await process.StandardOutput.ReadLineAsync());
        }

        DirectoryInfo managedDirectory = new(Path.Combine(installationDirectory.FullName, "SCPSL_Data/Managed"));

        AssembliesPublicizer.RunPublicizer(managedDirectory);

        installationDirectory.Delete(true);
    }
}