using Avalonia.Controls;
using PaintPower.Editors;
using PaintPower.ProjectSystem;
using PaintPower.FileExplorer;
using PaintPower.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using PaintPower.Networking;
namespace PaintPower;

public partial class MainWindow : Window
{
    private readonly Editor _editorManager;
    private readonly PaintProject _project;
    private Control _editor;
    public Server server;

    public MainWindow()
    {
        InitializeComponent();

        _project = new PaintProject();
        _editorManager = new Editor(_project.Workspace);
        server = new Server();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        await Task.Yield();

        var dialog = new ProjectLoaderDialog();
        var result = await new ProjectLoaderDialog().ShowDialog<ProjectLoaderResult?>(this);
        if (result == null) { Close(); return; }

        if (result.Mode == ProjectLoaderMode.New)
            _project.CreateNew(result.Path, "New Project");
        else
            _project.Load(result.Path);

        Title = $"PaintPower - {_project.Metadata.name}";

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
        _editor = editor;
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
        _editor = null;
    }

    public void save() {
    }
}