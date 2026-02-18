namespace PaintPower.Editors;

using PaintPower.ProjectSystem;
using Avalonia.Controls;

public class VideoEditor : UserControl
{
    public VideoEditor(string path, TempWorkspace workspace)
    {
        Content = new TextBlock { Text = $"Video editor placeholder for: {path}" };
    }
}