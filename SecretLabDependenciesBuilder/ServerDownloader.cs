using SecretLabDependenciesBuilder.Downloader;

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

        ArgumentBuilder args = new ArgumentBuilder()
            .AddArgument("-app", "996560")
            .AddArgument("-filelist", filesPath)
            .AddArgument("-dir", installationDirectory.FullName);

        if (!string.IsNullOrEmpty(_beta))
        {
            args.AddArgument("-beta", _beta);

            if (!string.IsNullOrEmpty(_betaPassword))
                args.AddArgument("-betapassword", _betaPassword);
        }

        bool success = await Downloader.Downloader.StartWithArgs(args.Build());

        if (!success)
        {
            ConsoleWriter.Write("An error occurred while downloading the files.", ConsoleColor.Red);
            ConsoleWriter.Write("Press any key to exit...", ConsoleColor.Red);
            Console.ReadKey();
            return;
        }

        DirectoryInfo managedDirectory = new(Path.Combine(installationDirectory.FullName, "SCPSL_Data/Managed"));

        AssembliesPublicizer.RunPublicizer(managedDirectory);

        installationDirectory.Delete(true);
    }
}