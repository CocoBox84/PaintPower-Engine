using Avalonia.Controls;
using PaintPower.Editors;
using PaintPower.FileExplorer;
using PaintPower.ProjectSystem;
using PaintPower.Dialogs;
using System.IO;

namespace PaintPower;

public partial class MainWindow : Window
{
    private readonly Editor _editorManager;
    private readonly PaintProject _project;

    private ExplorerView explorerView;

    public MainWindow()
    {
        InitializeComponent();

        _project = new PaintProject();
        _editorManager = new Editor(_project.Workspace);
        explorerView = new ExplorerView();

        // Show project loader dialog
        LoadProjectAtStartup();
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

    public void LoadProject(string path)
    {
        _project.Load(path);
    }

    public void CreateNewProject(string path, string name)
    {
        _project.CreateNew(path, name);
    }

    private async void LoadProjectAtStartup()
    {
        var dialog = new ProjectLoaderDialog();
        var result = await dialog.ShowDialog<ProjectLoaderResult?>(this);

        if (result == null)
        {
            Close();
            return;
        }

        if (result.Mode == ProjectLoaderMode.New)
        {
            _project.CreateNew(result.Path, "New Project");
        }
        else if (result.Mode == ProjectLoaderMode.Open)
        {
            _project.Load(result.Path);
        }

        Title = $"PaintPower - {_project.Metadata.Name}";

        // Initialize explorer
        explorerView.Initialize(_project.Workspace);

        // Open last file if exists
        if (!string.IsNullOrWhiteSpace(_project.Metadata.OpenFile))
        {
            string fullPath = _project.Workspace.MapToTemp(_project.Metadata.OpenFile);
            if (File.Exists(fullPath))
                OpenFile(fullPath);
        }

        explorerView.Initialize(_project.Workspace);
    }
}