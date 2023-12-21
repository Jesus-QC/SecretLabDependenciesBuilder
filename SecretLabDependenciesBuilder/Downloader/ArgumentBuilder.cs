using System.Text;

namespace SecretLabDependenciesBuilder.Downloader;

public class ArgumentBuilder
{
    private readonly List<Argument> _arguments = new();

    public ArgumentBuilder AddArgument(string name, string value)
    {
        _arguments.Add(new Argument(name, value));
        return this;
    }

    public ArgumentBuilder AddArgument(Argument argument)
    {
        _arguments.Add(argument);
        return this;
    }

    public ArgumentBuilder AddArguments(IEnumerable<Argument> arguments)
    {
        _arguments.AddRange(arguments);
        return this;
    }

    public string[] Build()
    {
        return _arguments.SelectMany(argument => argument.Build()).ToArray();
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        foreach (Argument argument in _arguments)
        {
            builder.Append(argument.ToString());
            builder.Append(" ");
        }

        return builder.ToString();
    }
}