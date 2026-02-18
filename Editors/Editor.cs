namespace PaintPower.Editors;

using System.IO;
using Avalonia.Controls;
using PaintPower.ProjectSystem;

public class Editor
{
    private readonly TempWorkspace _workspace;

    public Editor(TempWorkspace workspace)
    {
        _workspace = workspace;
    }

    public Control GetEditorFromFileType(string path)
    {
        string relative = Path.GetFileName(path); // later: full relative path

        var ext = Path.GetExtension(path).ToLower();

        return ext switch
        {
            ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp"
                => new PaintEditor(relative, _workspace),

            ".txt" or ".cs" or ".json" or ".xml" or ".lua" or ".py"
                => new ScriptEditor(relative, _workspace),

            ".wxa"
                => new AnimationEditor(relative, _workspace),

            _ => new TextBlock { Text = $"Unsupported file: {ext}" }
        };
    }
}