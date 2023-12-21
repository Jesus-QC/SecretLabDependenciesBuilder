// All in one - Server Downloader and Publicizer.

using SecretLabDependenciesBuilder;

ConsoleWriter.WriteTitle();
await ConfigManager.LoadConfigAsync();
await ServerDownloader.RunAsync();