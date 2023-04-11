using System;
using System.Text;

namespace Griffin.Data.Helpers;

public class TabbedStringBuilder
{
    private readonly StringBuilder _sb = new();
    private bool _indented;
    private int _indents;
    private string _spacing = "";

    public void Append(string str)
    {
        EnsureSpacing();
        _sb.Append(str);
    }

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

    public void AppendLineIndent(string? str = null)
    {
        AppendLine(str);
        Indent();
    }

    public void Dedent()
    {
        _indents--;
        _spacing = "".PadLeft(_indents * 4, ' ');
    }

    public void DedentAppendLine(string? str = null)
    {
        Dedent();
        AppendLine(str);
    }

    public void Indent()
    {
        _indents++;
        _spacing = "".PadLeft(_indents * 4, ' ');
    }

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

    public void RemoveLineEnding()
    {
        _sb.Remove(_sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
    }
}
