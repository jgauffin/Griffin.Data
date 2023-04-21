using Griffin.Data.Scaffolding.Config;

namespace Griffin.Data.Scaffolding.Mapper;

public class GeneratorContext
{
    private readonly List<GeneratedFile> _files = new();

    public GeneratorContext(IReadOnlyList<Table> tables, ProjectFolders folders)
    {
        Tables = tables;
        Folders = folders;
    }

    public ProjectFolders Folders { get; }

    public IReadOnlyList<GeneratedFile> GeneratedFiles => _files;
    public IReadOnlyList<Table> Tables { get; }

    public void Add(GeneratedFile file)
    {
        _files.Add(file);
    }
}
