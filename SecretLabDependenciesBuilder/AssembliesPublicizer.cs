using System.IO.Compression;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace SecretLabDependenciesBuilder;

public static class AssembliesPublicizer
{
    private static readonly string[] AssembliesToPublicize =
    {
        "Assembly-CSharp.dll",
        "Mirror.dll",
        "PluginAPI.dll",
    };
    
    public static void RunPublicizer(DirectoryInfo directoryInfo)
    {
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            if (!AssembliesToPublicize.Contains(file.Name))
                continue;
            
            PublicizeAssembly(file, directoryInfo);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Done!");

        ZipFile.CreateFromDirectory(directoryInfo.FullName, Path.Combine(Environment.CurrentDirectory, "References.zip"));
    }

    private static void PublicizeAssembly(FileSystemInfo file, FileSystemInfo referencesDirectory)
    {
        DefaultAssemblyResolver resolver = new ();
        resolver.AddSearchDirectory(referencesDirectory.FullName);
        
        using ModuleDefinition assembly = ModuleDefinition.ReadModule(file.FullName, new ReaderParameters
        {
            AssemblyResolver = resolver,
            ReadWrite = true
        });
        
        if (assembly is null)
        {
            Console.WriteLine($"[Publicizer] Assembly in {file.FullName} not found. Could not patch.");
            return;
        }

        foreach (TypeDefinition type in assembly.GetAllTypes())
        {
            type.IsPublic = true;

            foreach (FieldDefinition field in type.Fields)
                field.IsPublic = true;
            
            foreach (MethodDefinition method in type.Methods)
                method.IsPublic = true;

            foreach (PropertyDefinition property in type.Properties)
            {
                if (property.GetMethod != null)
                    property.GetMethod.IsPublic = true;
                
                if (property.SetMethod != null)
                    property.SetMethod.IsPublic = true;
            }

            foreach (EventDefinition eventDefinition in type.Events)
            {
                eventDefinition.AddMethod.IsPublic = true;
                eventDefinition.RemoveMethod.IsPublic = true;
                
                if (eventDefinition.InvokeMethod != null)
                    eventDefinition.InvokeMethod.IsPublic = true;
            }
        }
        
        assembly.Write(file.FullName[..^4] + "-Publicized.dll");
    }
}