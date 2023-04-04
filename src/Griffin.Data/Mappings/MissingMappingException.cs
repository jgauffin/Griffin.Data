using System;

namespace Griffin.Data.Mappings;

/// <summary>
///     A mapping is missing.
/// </summary>
public class MissingMappingException : Exception
{
    private readonly string _scannedAssemblies;

    /// <summary>
    /// </summary>
    /// <param name="entityType">Type that did not have a mapping.</param>
    /// <param name="scannedAssemblies">Comma-separated list if assembly names.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public MissingMappingException(Type entityType, string scannedAssemblies)
    {
        _scannedAssemblies = scannedAssemblies ?? throw new ArgumentNullException(nameof(scannedAssemblies));
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
    }

    /// <summary>
    ///     Type that did not have a mapping.
    /// </summary>
    public Type EntityType { get; }

    /// <inheritdoc />
    public override string Message =>
        $"Missing mapping for ${EntityType}. Have scanned the assemblies {_scannedAssemblies}";
}