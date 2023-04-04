using System;

namespace Griffin.Data.Configuration;

/// <summary>
/// </summary>
/// <typeparam name="TParentEntity">Type of parent entity (contains the discriminator property).</typeparam>
/// <typeparam name="TChildEntity">Base class for the child entity.</typeparam>
public class Discriminator<TParentEntity, TChildEntity>
{
    /// <summary>
    /// </summary>
    /// <param name="propertyName">Property used to select type of child entity.</param>
    /// <param name="typeSelector">Callback used to select child entity (based on the property value).</param>
    public Discriminator(string propertyName, Func<object, Type?> typeSelector)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        TypeSelector = typeSelector ?? throw new ArgumentNullException(nameof(typeSelector));
    }

    /// <summary>
    ///     Property used to select type of child entity.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    ///     Callback used to select child entity (based on the property value).
    /// </summary>
    public Func<object, Type?> TypeSelector { get; }
}