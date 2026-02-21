using Avalonia.Controls;
using PaintPower.Editors;
using PaintPower.ProjectSystem;
using PaintPower.FileExplorer;
using PaintPower.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
namespace PaintPower;

public partial class MainWindow : Window
{
    private readonly Editor _editorManager;
    private readonly PaintProject _project;

    public MainWindow()
    {
        InitializeComponent();

        _project = new PaintProject();
        _editorManager = new Editor(_project.Workspace);
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        await Task.Yield();

        var dialog = new ProjectLoaderDialog();
        // In MainWindow.OnOpened
        var result = await dialog.ShowAsync(this); // use ShowAsync, not ShowDialog<T>
        if (result == null)
        {
            Close(); // user cancelled
            return;
        }
        if (result.Mode == ProjectLoaderMode.New)
            _project.CreateNew(result.Path, "New Project");
        else
            _project.Load(result.Path);

        Title = $"PaintPower - {_project.Metadata.Name}";

        Explorer.Initialize(_project.Workspace);

        if (!string.IsNullOrWhiteSpace(_project.Metadata.OpenFile))
        {
            string fullPath = _project.Workspace.MapToTemp(_project.Metadata.OpenFile);
            if (File.Exists(fullPath))
                OpenFile(fullPath);
        }
    }

    public void OpenEditor(Control editor)
    {
        EditorHost.Content = editor;
    }

    public void OpenFile(string path)
    {
        var editor = _editorManager.GetEditorFromFileType(path);
        OpenEditor(editor);
    }

    public void CloseEditor()
    {
        EditorHost.Content = null;
    }
    }