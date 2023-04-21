using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Config;

namespace Griffin.Data.Scaffolding.Mapper.Generators;

public abstract class GeneratorWithNamespace : IClassGenerator
{
    public void Generate(Table table, GeneratorContext context)
    {
        var sb = new TabbedStringBuilder();

        AddUsings(table, sb, context);
        sb.AppendLine();
        BeginNamespace(context.Folders, table, sb);
        GenerateClass(sb, table, context);
        EndNamespace(table, sb);

        var file = GenerateFile(table, context, sb.ToString());
        file.RelativeDirectory = GetDirectory(table, file.FileType, context.Folders);
        context.Add(file);
    }

    protected virtual void AddUsings(Table table, TabbedStringBuilder sb, GeneratorContext context)
    {
    }

    protected virtual void BeginNamespace(ProjectFolders folders, Table table, TabbedStringBuilder sb)
    {
        if (table.RelativeNamespace.Length > 0)
        {
            sb.AppendLine($"namespace {GetNamespaceName(table, folders)}");
            sb.AppendLineIndent("{");
        }
    }

    protected virtual void EndNamespace(Table table, TabbedStringBuilder sb)
    {
        if (table.RelativeNamespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }
    }

    protected abstract void GenerateClass(TabbedStringBuilder sb, Table table, GeneratorContext context);

    protected abstract GeneratedFile GenerateFile(Table table, GeneratorContext context, string contents);

    protected virtual string GetDirectory(Table table, FileType fileFileType, ProjectFolders folders)
    {
        var baseFolder = fileFileType switch
        {
            FileType.Data => folders.DataFolder,
            FileType.DataTest => folders.DataTestFolder,
            FileType.DomainTest => folders.DomainTestFolder,
            _ => folders.DomainFolder
        };

        return
            $"{baseFolder}{Path.DirectorySeparatorChar}{table.RelativeNamespace.Replace('.', Path.DirectorySeparatorChar)}";
    }

    protected virtual string GetNamespaceName(Table table, ProjectFolders projectFolders)
    {
        return table.RelativeNamespace;
    }
}
