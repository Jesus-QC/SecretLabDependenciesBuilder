// All in one - Server Downloader and Publicizer.

#if DEBUG
Environment.CurrentDirectory = @"D:\_Coding\Tools\SecretLabDependencyBuilder";
#endif

Console.WriteLine("███████████████████████████████████████\n█─▄▄▄▄█▄─▄█████▄─▄▄▀█▄─▄▄─█▄─▄▄─█─▄▄▄▄█\n█▄▄▄▄─██─██▀████─██─██─▄█▀██─▄▄▄█▄▄▄▄─█\n▀▄▄▄▄▄▀▄▄▄▄▄▀▀▀▄▄▄▄▀▀▄▄▄▄▄▀▄▄▄▀▀▀▄▄▄▄▄▀\nMade by Jesus-QC\n");

await SecretLabDependenciesBuilder.ServerDownloader.RunAsync();