using System;
using System.Text;

namespace Griffin.Data.Helpers;

/// <summary>
///     A string builder which is indented (with spaces).
/// </summary>
/// <remarks>
///     <para>
///         Indentation is handled by the library and added when needed.
///     </para>
/// </remarks>
public class TabbedStringBuilder
{
    private readonly StringBuilder _sb = new();
    private bool _indented;
    private int _indents;
    private string _spacing = "";

    /// <summary>
    ///     Append a string.
    /// </summary>
    /// <param name="str">String to append.</param>
    public void Append(string str)
    {
        EnsureSpacing();
        _sb.Append(str);
    }

    /// <summary>
    ///     Append a string and move to a new line.
    /// </summary>
    /// <param name="str">String to append (if any).</param>
    public void AppendLine(string? str = null)
    {
        if (str == null)
        {
            _sb.AppendLine();
            _indented = false;
            return;
        }

        EnsureSpacing();
        _sb.AppendLine(str);
        _indented = false;
    }

    /// <summary>
    ///     Append to a new line and start the new line with a deeper indentation.
    /// </summary>
    /// <param name="str">String to append to the current line.</param>
    public void AppendLineIndent(string? str = null)
    {
        AppendLine(str);
        Indent();
    }

    /// <summary>
    ///     Dedent a step on the next line.
    /// </summary>
    public void Dedent()
    {
        _indents--;
        _spacing = "".PadLeft(_indents * 4, ' ');
    }

    /// <summary>
    ///     Dedent, append a string and then move to a new line.
    /// </summary>
    /// <param name="str">String to append (if any).</param>
    public void DedentAppendLine(string? str = null)
    {
        Dedent();
        AppendLine(str);
    }

    /// <summary>
    ///     Increase indentation for the next line (or current line if nothing has been added to it yet).
    /// </summary>
    public void Indent()
    {
        _indents++;
        _spacing = "".PadLeft(_indents * 4, ' ');
    }

    /// <summary>
    /// Remove line ending from the last line.
    /// </summary>
    public void RemoveLineEnding()
    {
        var spaces = _indents * 4;
        _sb.Remove(_sb.Length - Environment.NewLine.Length - spaces, Environment.NewLine.Length + spaces);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _sb.ToString();
    }

    private void EnsureSpacing()
    {
        if (!_indented)
        {
            _indented = true;
            _sb.Append(_spacing);
        }
    }
}
