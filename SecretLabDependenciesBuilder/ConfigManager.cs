using System.ComponentModel;
using YamlDotNet.Serialization;

namespace SecretLabDependenciesBuilder;

public static class ConfigManager
{
    public static Config CurrentConfig { get; private set; } = new Config();

    public static async Task LoadConfigAsync()
    {
        CurrentConfig = await GetConfigAsync();
    }

    private static async Task<Config> GetConfigAsync()
    {
        string configPath = Path.Combine(Environment.CurrentDirectory, "config.yml");
        if (!File.Exists(configPath))
        {
            ConsoleWriter.Write("Config file not found. Creating one...", ConsoleColor.Yellow);

            await File.WriteAllTextAsync(configPath, new SerializerBuilder().Build().Serialize(new Config()));

            ConsoleWriter.Write("Config file created. Please fill it with the desired data.", ConsoleColor.Yellow);
            ConsoleWriter.Write("Press any key to continue...", ConsoleColor.Yellow);
            Console.ReadKey();
            Console.Clear();

            return new Config();
        }

        return new DeserializerBuilder().Build().Deserialize<Config>(await File.ReadAllTextAsync(configPath))!;
    }
}

public class Config
{
    public string[] AssembliesToPublicize { get; set; } = new string[3]
    {
        "Assembly-CSharp.dll",
        "Mirror.dll",
        "PluginAPI.dll",
    };

    [Description("Whether to zip the references or not.")]
    public bool ZipReferences { get; set; } = true;

    [Description("The name and/or path of the zip file.")]
    public string ReferencesZipName { get; set; } = "References.zip";

    [Description("Whether to save the references to a folder or not.")]
    public bool SaveReferencesToFolder { get; set; } = false;

    [Description("The directory to save the references to.")]
    public string ReferencesFolderName { get; set; } = "References";
}
