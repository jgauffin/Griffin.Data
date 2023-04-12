namespace Griffin.Data.Scaffolding;

/// <summary>
///     A generated file (i.e. a C# class).
/// </summary>
public class GeneratedFile
{
    public GeneratedFile(string className, string contents)
    {
        ClassName = className ?? throw new ArgumentNullException(nameof(className));
        Contents = contents ?? throw new ArgumentNullException(nameof(contents));
    }

    /// <summary>
    ///     Name of class.
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    ///     File contents (text version of a class).
    /// </summary>
    public string Contents { get; }

    /// <summary>
    ///     Directory to store in (from the working directory).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         May not start with a slash/backslash.
    ///     </para>
    /// </remarks>
    public string RelativeDirectory { get; set; } = "";
}
