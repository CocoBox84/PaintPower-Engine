namespace PaintPower.Editors;

using System;
using System.IO;
using Avalonia.Controls;
using PaintPower.ProjectSystem;

public class Editor
{
    private readonly TempWorkspace _workspace;
    private static Control ActiveEditor = null;

    public Editor(TempWorkspace workspace)
    {
        _workspace = workspace;
    }

    public Control GetEditorFromFileType(string path)
    {
        // FIX: keep full relative path inside items/
        string relative = Path.GetRelativePath(_workspace.ItemsDir, path);

        var ext = Path.GetExtension(path);
        var type = Editors.EditorTypes.FindEditorFromExt(ext.ToLower());

        return type switch
        {
            "Paint" => new PaintEditor(relative, _workspace),
            "Script" => new ScriptEditor(relative, _workspace),
            "Animation" => new AnimationEditor(relative, _workspace),
            "Video" => new VideoEditor(relative, _workspace),
            _ => new TextBlock { Text = $"Unsupported file: {ext}" }
        };
    }

    private Control addPaintEditor(string relative, TempWorkspace _workspace) {
        var x = new PaintEditor(relative, _workspace);
        ActiveEditor = x;
        return x;
    }
    private Control addScriptEditor(string relative, TempWorkspace _workspace)
    {
        var x = new ScriptEditor(relative, _workspace);
        ActiveEditor = x;
        return x;
    }

    private Control addAnimationEditor(string relative, TempWorkspace _workspace)
    {
        var x = new AnimationEditor(relative, _workspace);
        ActiveEditor = x;
        return x;
    }

    private Control addVideoEditor(string relative, TempWorkspace _workspace)
    {
        var x = new VideoEditor(relative, _workspace);
        ActiveEditor = x;
        return x;
    }

    // For the future, use it's own class. For now, use ScriptEditor
    private Control addTextEditor(string relative, TempWorkspace _workspace)
    {
        var x = new ScriptEditor(relative, _workspace);
        ActiveEditor = x;
        return x;
    }

    // Save items in the editor to the temp directory.
    public static void SaveEditor() {
        //ActiveEditor.save();
    }
}