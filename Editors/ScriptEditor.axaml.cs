using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using PaintPower.ProjectSystem;
using System;
using System.Diagnostics;
using System.IO;

namespace PaintPower.Editors;

public partial class ScriptEditor : EditorBase
{
    private readonly string _relativePath;
    private readonly TempWorkspace _workspace;
    private TextMate.Installation _textMateInstallation;
    private RegistryOptions _registryOptions;

    public ScriptEditor(string relativePath, TempWorkspace workspace)
    {
        Debug.WriteLine("Loading Script Editor...");
        _relativePath = relativePath;
        _workspace = workspace;

        AvaloniaXamlLoader.Load(this);

        var editor = this.FindControl<TextEditor>("Editor");

        if (editor != null)
        {
            editor.Text = _workspace.LoadText(relativePath);
            editor.Focus();
        }

        // 1. Create registry options with a theme
        _registryOptions = new RegistryOptions(ThemeName.LightPlus);

        // 2. Install TextMate
        _textMateInstallation = editor.InstallTextMate(_registryOptions);

        // 3. Set theme
        _textMateInstallation.SetTheme(_registryOptions.LoadTheme(ThemeName.DarkPlus));

        // 4. Set grammar based on file extension
        var ext = Path.GetExtension(relativePath);
        var scope = _registryOptions.GetScopeByExtension(ext);

        if (scope != null)
        {
            _textMateInstallation.SetGrammar(scope);
        }
        else
        {
            Debug.WriteLine($"No TextMate grammar found for extension: {ext}");
        }

        // 5. Remove AvaloniaEdit built-in highlighting (must NOT be used)
        editor.SyntaxHighlighting = null;

        // Autosave
        editor.TextChanged += (_, __) =>
        {
            MainWindow.App.SetProjectStatus("Save Project");
            MainWindow.App.saveNeeded = true;
            Save();
        };
    }

    override public void Save()
    {
        var editor = this.FindControl<TextEditor>("Editor");
        _workspace.SaveFile(_relativePath, editor.Text);
    }
}