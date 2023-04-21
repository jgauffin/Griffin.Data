namespace Griffin.Data.Scaffolding.Config;

/// <summary>
///     Folders auto-discovered by scanning solution folders.
/// </summary>
public class ProjectFolders
{
    /// <summary>
    ///     Root folder for data implementations.
    /// </summary>
    public string DataFolder { get; set; } = "";

    /// <summary>
    ///     Namespace for root in the data project.
    /// </summary>
    public string DataNamespace { get; set; } = "";

    /// <summary>
    ///     Root folder for data tests.
    /// </summary>
    public string DataTestFolder { get; set; } = "";

    /// <summary>
    ///     Namespace for root in the data test project.
    /// </summary>
    public string DataTestNamespace { get; set; } = "";

    /// <summary>
    ///     Root folder for business/domain entities.
    /// </summary>
    public string DomainFolder { get; set; } = "";

    /// <summary>
    ///     Namespace for root in the domain project (business layer).
    /// </summary>
    public string DomainNamespace { get; set; } = "";

    /// <summary>
    ///     Root folder for domain tests.
    /// </summary>
    public string DomainTestFolder { get; set; } = "";

    /// <summary>
    ///     Namespace for root in the domain test project.
    /// </summary>
    public string DomainTestNamespace { get; set; } = "";
}
