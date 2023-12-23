using System.IO.Compression;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace SecretLabDependenciesBuilder;

public static class AssembliesPublicizer
{
    public static void RunPublicizer(DirectoryInfo directoryInfo)
    {
        foreach (FileInfo file in directoryInfo.GetFiles("*.dll"))
        {
            if (!ConfigManager.CurrentConfig.AssembliesToPublicize.Contains(file.Name))
                continue;

            PublicizeAssembly(file, directoryInfo);
        }

        ConsoleWriter.Write("Done", ConsoleColor.Green);

        if (ConfigManager.CurrentConfig.SaveReferencesToFolder)
            SaveReferencesToFolder(directoryInfo);

        if (ConfigManager.CurrentConfig.ZipReferences)
            SaveReferencesToZip(directoryInfo);
    }

    private static void SaveReferencesToFolder(DirectoryInfo directoryInfo)
    {
        DirectoryInfo referencesDirectory = new DirectoryInfo(ConfigManager.CurrentConfig.ReferencesFolderName);
        if (!referencesDirectory.Exists)
            referencesDirectory.Create();

        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.CopyTo(Path.Combine(referencesDirectory.FullName, file.Name), true);
        }
    }

    private static void SaveReferencesToZip(DirectoryInfo directoryInfo)
    {
        string zipFile = ConfigManager.CurrentConfig.ReferencesZipName;
        if (File.Exists(zipFile))
            File.Delete(zipFile);

        // create directory if it doesn't exist
        new FileInfo(zipFile).Directory?.Create();


        ZipFile.CreateFromDirectory(directoryInfo.FullName, zipFile);
    }

    private static void PublicizeAssembly(FileSystemInfo file, FileSystemInfo referencesDirectory)
    {
        using DefaultAssemblyResolver resolver = new();
        resolver.AddSearchDirectory(referencesDirectory.FullName);

        using ModuleDefinition assembly = ModuleDefinition.ReadModule(file.FullName, new ReaderParameters
        {
            AssemblyResolver = resolver,
            ReadWrite = true
        });

        if (assembly is null)
        {
            ConsoleWriter.Write($"[Publicizer] Assembly in {file.FullName} not found. Could not patch.",
                ConsoleColor.Red);
            return;
        }

        foreach (TypeDefinition type in assembly.GetAllTypes())
        {
            switch (type.IsNested)
            {
                case false:
                    type.IsPublic = true;
                    break;
                case true:
                    type.IsNestedPublic = true;
                    break;
            }

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
                CheckForBackfields(eventDefinition);

                eventDefinition.AddMethod.IsPublic = true;
                eventDefinition.RemoveMethod.IsPublic = true;

                if (eventDefinition.InvokeMethod != null)
                    eventDefinition.InvokeMethod.IsPublic = true;
            }
        }

        assembly.Write(file.FullName[..^4] + "-Publicized.dll");
    }

    // Basically we are checking for fields with the same name as the event,
    // if there is any field with the same name then we make the field private,
    // ironically privatizing the field instead of publicizing it,
    // but needs to be done so the events we are publicizing can be used without
    // any ambitious reference issue.
    private static void CheckForBackfields(EventDefinition eventDefinition)
    {
        foreach (FieldDefinition fieldDefinition in eventDefinition.DeclaringType.Fields)
        {
            if (fieldDefinition.Name != eventDefinition.Name)
                continue;

            fieldDefinition.IsPrivate = true;
        }
    }
}