namespace Griffin.Data.Scaffolding.Config;

/// <summary>
///     Choices for <see cref="ScaffoldingConfiguration" />.
/// </summary>
/// <remarks>
///<para>
///Thi is an flags enum so that we can generate tests for each type.
/// </para>
/// </remarks>
[Flags]
public enum TypeToGenerate
{
    /// <summary>
    ///     Generate entity classes.
    /// </summary>
    Entities = 1,

    /// <summary>
    ///     Generate mappings for entities.
    /// </summary>
    Mappings = 2,

    /// <summary>
    ///     Generate repository classes and interfaces.
    /// </summary>
    Repositories = 4,

    /// <summary>
    ///     Generate queries.
    /// </summary>
    Queries = 8,

    /// <summary>
    ///     Generate tests (for those types that are generated).
    /// </summary>
    Tests = 32
}