// All in one - Server Downloader and Publicizer.

using SecretLabDependenciesBuilder;
#if DEBUG
Environment.CurrentDirectory = @"C:\Users\jesus\Documents\_Coding\SecretLabDependencyBuilder";
#endif

ConsoleWriter.WriteTitle();
await ServerDownloader.RunAsync();