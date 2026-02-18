namespace PaintPower.Editors;
using PaintPower.ProjectSystem;
using Avalonia.Controls;

public class ScriptEditor : UserControl
{
    private readonly string _relativePath;
    private readonly TempWorkspace _workspace;

    public ScriptEditor(string relativePath, TempWorkspace workspace)
    {
        _relativePath = relativePath;
        _workspace = workspace;

        var text = _workspace.LoadText(relativePath);
        Content = new TextBox { Text = text };
    }

    public void Save()
    {
        if (Content is TextBox tb)
            _workspace.SaveFile(_relativePath, tb.Text);
    }
}