namespace SecretLabDependenciesBuilder;

public static class ConsoleWriter
{
    public static void Write(string? message, ConsoleColor color = ConsoleColor.White)
    {
        if (message is null)
            return;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void WriteTitle()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("""
                          ███████████████████████████████████████
                          █─▄▄▄▄█▄─▄█████▄─▄▄▀█▄─▄▄─█▄─▄▄─█─▄▄▄▄█
                          █▄▄▄▄─██─██▀████─██─██─▄█▀██─▄▄▄█▄▄▄▄─█
                          ▀▄▄▄▄▄▀▄▄▄▄▄▀▀▀▄▄▄▄▀▀▄▄▄▄▄▀▄▄▄▀▀▀▄▄▄▄▄▀
                          """);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Made by Jesus-QC and x3rt");
    }
}