namespace PaintPower.Editors;

using System;
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

        var ext = Path.GetExtension(path);
        var type = Editors.EditorTypes.FindEditorFromExt(ext.ToLower()); // Get editor type.

        return type switch
        {
            "Paint"
                => new PaintEditor(relative, _workspace),

            "Script"
                => new ScriptEditor(relative, _workspace),

            "Animation"
                => new AnimationEditor(relative, _workspace),

            "Video"
                => new VideoEditor(relative, _workspace),

            _ => new TextBlock { Text = $"Unsupported file: {ext}" }
        };
    }
}