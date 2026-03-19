using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.TextMate;
using PaintPower.Editors.EditorTools.ScriptEditorTools;
using PaintPower.EditorTools.ScriptEditorTools;
using PaintPower.ProjectSystem;
using System;
using System.Diagnostics;
using System.IO;
using TextMateSharp.Grammars;

namespace PaintPower.Editors;

public partial class ScriptEditor : EditorBase
{
    private readonly string _relativePath;
    private readonly TempWorkspace _workspace;
    private TextMate.Installation _textMateInstallation;
    private RegistryOptions _registryOptions;

    private RainbowBracketColorizer _bracketColorizer;
    private FoldingManager _foldingManager;
    private CodeFoldingStrategy _foldingStrategy;

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

        // NOW add bracket colorizer
        _bracketColorizer = new RainbowBracketColorizer(editor.Document);
        editor.TextArea.TextView.LineTransformers.Add(_bracketColorizer);

        _foldingManager = FoldingManager.Install(editor.TextArea);
        _foldingStrategy = new CodeFoldingStrategy();
        _foldingStrategy.UpdateFoldings(_foldingManager, editor.Document);


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

            _bracketColorizer.Update(editor.Document);
            editor.TextArea.TextView.InvalidateVisual();

            _foldingStrategy.UpdateFoldings(_foldingManager, editor.Document);

            Save();
        };
    }

    override public void Save()
    {
        var editor = this.FindControl<TextEditor>("Editor");
        _workspace.SaveFile(_relativePath, editor.Text);
    }
}