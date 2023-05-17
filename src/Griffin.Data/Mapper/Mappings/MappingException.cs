using System;

namespace Griffin.Data.Mapper.Mappings;

/// <summary>
///     Could not use the mapping as expected.
/// </summary>
public class MappingException : GriffinException
{
    /// <summary>
    /// </summary>
    /// <param name="entityType">Entity that configuration failed for.</param>
    /// <param name="errorMessage">Why it failed.</param>
    /// <exception cref="ArgumentNullException">either parameter is null.</exception>
    public MappingException(Type entityType, string errorMessage)
        : base(errorMessage)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
    }

    /// <summary>
    /// </summary>
    /// <param name="entity">Entity that mapping failed for.</param>
    /// <param name="errorMessage">Why it failed.</param>
    /// <exception cref="ArgumentNullException">either parameter is null.</exception>
    public MappingException(object entity, string errorMessage)
        : base(errorMessage)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        EntityType = entity.GetType();
        Entity = entity;
    }

    /// <summary>
    ///     Entity that mapping failed for.
    /// </summary>
    public object? Entity { get; set; }

    /// <summary>
    ///     Entity that configuration failed for.
    /// </summary>
    public Type EntityType { get; }

    /// <inheritdoc />
    public override string Message => $"{EntityType.Name}: {base.Message}\r\n{Entity}";
}
