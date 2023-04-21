namespace Griffin.Data.Scaffolding.Mapper;

/// <summary>
///     Invoked before generators, can be used to modify the meta such as types, names or namespaces.
/// </summary>
public interface IMetaAnalyzer
{
    /// <summary>
    ///     Lowest priority runs first.
    /// </summary>
    int Priority { get; }

    void Analyze(GeneratorContext context);
}
