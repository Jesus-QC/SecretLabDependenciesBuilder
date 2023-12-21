namespace SecretLabDependenciesBuilder.Downloader;

public class Argument(string name, string value)
{
    public string Name { get; set; } = name;
    public string Value { get; set; } = value;

    public override string ToString()
    {
        return $"{Name} {Value}";
    }
    
    public string[] Build()
    {
        return new[] {Name, Value};
    }
    
}