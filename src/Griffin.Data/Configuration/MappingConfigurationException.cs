using System;

namespace Griffin.Data.Configuration;

/// <summary>
///     Something was not configured correctly.
/// </summary>
public class MappingConfigurationException : Exception
{
    /// <summary>
    /// </summary>
    /// <param name="entityType">Entity that configuration failed for.</param>
    /// <param name="errorMessage">Why it failed.</param>
    /// <exception cref="ArgumentNullException">either parameter is null.</exception>
    public MappingConfigurationException(Type entityType, string errorMessage)
        : base(errorMessage)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
    }

    /// <summary>
    /// </summary>
    /// <param name="entityType">Entity that configuration failed for.</param>
    /// <param name="errorMessage">Why it failed.</param>
    /// <param name="inner">Inner exception.</param>
    /// <exception cref="ArgumentNullException">either parameter is null.</exception>
    public MappingConfigurationException(Type entityType, string errorMessage, Exception inner)
        : base(errorMessage, inner)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
    }

    /// <summary>
    ///     Entity that configuration failed for.
    /// </summary>
    public Type EntityType { get; }

    /// <inheritdoc />
    public override string Message => $"{EntityType.Name}: {base.Message}";
}
