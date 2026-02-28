using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using PaintPower.ProjectSystem;
using System;
using System.Diagnostics;
using System.IO;

namespace PaintPower.Editors;

public partial class ScriptEditor : UserControl
{
    private readonly string _relativePath;
    private readonly TempWorkspace _workspace;

    public ScriptEditor(string relativePath, TempWorkspace workspace)
    {
        Debug.WriteLine("Loading Script Editor...");
        _relativePath = relativePath;
        _workspace = workspace;

        AvaloniaXamlLoader.Load(this);

        var editor = this.FindControl<TextEditor>("Editor");

        if (editor != null)
        {

            // Load file text
            editor.Text = _workspace.LoadText(relativePath);
            Debug.WriteLine(editor.Text);
            editor.Focus();

            Debug.WriteLine($"Loading script: {relativePath}");
        }
        else {
            Debug.WriteLine("Editor is null.");
        }

        // Syntax highlighting based on extension
            var ext = Path.GetExtension(relativePath);
        editor.SyntaxHighlighting =
            HighlightingManager.Instance.GetDefinitionByExtension(ext)
            ?? HighlightingManager.Instance.GetDefinition("C#");

        // Autosave on change
        editor.TextChanged += (_, __) => Save();
    }

    public void Save()
    {
        var editor = this.FindControl<TextEditor>("Editor");
        _workspace.SaveFile(_relativePath, editor.Text);
    }
}