// All in one - Server Downloader and Publicizer.

#if DEBUG
Environment.CurrentDirectory = @"D:\_Coding\Tools\SecretLabDependencyBuilder";
#endif

Console.WriteLine("""
                  ███████████████████████████████████████
                  █─▄▄▄▄█▄─▄█████▄─▄▄▀█▄─▄▄─█▄─▄▄─█─▄▄▄▄█
                  █▄▄▄▄─██─██▀████─██─██─▄█▀██─▄▄▄█▄▄▄▄─█
                  ▀▄▄▄▄▄▀▄▄▄▄▄▀▀▀▄▄▄▄▀▀▄▄▄▄▄▀▄▄▄▀▀▀▄▄▄▄▄▀
                  Made by Jesus-QC
                  """);

await SecretLabDependenciesBuilder.ServerDownloader.RunAsync();