// All in one - Server Downloader and Publicizer.

#if DEBUG
Environment.CurrentDirectory = @"C:\Users\jesus\Documents\_Coding\SecretLabDependencyBuilder";
#endif

Console.WriteLine("""
                  ███████████████████████████████████████
                  █─▄▄▄▄█▄─▄█████▄─▄▄▀█▄─▄▄─█▄─▄▄─█─▄▄▄▄█
                  █▄▄▄▄─██─██▀████─██─██─▄█▀██─▄▄▄█▄▄▄▄─█
                  ▀▄▄▄▄▄▀▄▄▄▄▄▀▀▀▄▄▄▄▀▀▄▄▄▄▄▀▄▄▄▀▀▀▄▄▄▄▄▀
                  Made by Jesus-QC
                  """);

await SecretLabDependenciesBuilder.ServerDownloader.RunAsync();