namespace PaintPower.Editors;

using PaintPower.ProjectSystem;
using Avalonia.Controls;

public class PaintEditor : UserControl
{
    public PaintEditor(string path, TempWorkspace workspace)
    {
        Content = new TextBlock { Text = $"Paint editor placeholder for: {path}" };
    }
}