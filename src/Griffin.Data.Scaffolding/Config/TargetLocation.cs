namespace Griffin.Data.Scaffolding.Config;

internal class TargetLocation
{
    public TargetLocation(string projectDirectory, string projectName, string namespaceTemplate)
    {
        ProjectDirectory = projectDirectory;
        ProjectName = projectName;
        NamespaceTemplate = namespaceTemplate;
    }

    public string NamespaceTemplate { get; }
    public string ProjectDirectory { get; }
    public string ProjectName { get; }
}
