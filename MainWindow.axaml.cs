using Avalonia.Controls;
using PaintPower.Editors;
using PaintPower.ProjectSystem;
using PaintPower.FileExplorer;
using PaintPower.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using PaintPower.Networking;
using PaintPower.Logging;
using System.Diagnostics;
namespace PaintPower;

public partial class MainWindow : Window
{
    private readonly Editor _editorManager;
    private readonly PaintProject _project;
    private EditorBase _editor;
    public Server server;

    public MainWindow()
    {
        InitializeComponent();

        _project = new PaintProject();
        _editorManager = new Editor(_project.Workspace);
        server = new Server();
        SaveButton.Click += (_, __) => Save();
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

    public void OpenEditor(EditorBase editor)
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

    async public void Save() {
        try
        {
            var doSave = false;
            var result = "";
            try
            {
                result = await new DoSaveWindowDialog().ShowAsync(this);
            }
            catch (Exception ex) { Log.QuickLog($"Error with dialog. {ex.ToString()}"); };
            ;
            doSave = result == "save";
            if (result == "saveas") SaveAs();
            if (!doSave) {
                Log.QuickLog($"Not saving. {result} {doSave}");
                return; 
            }
            Log.QuickLog("Saving Project...");
            ProjectSaver.Save(_project, _editor);
            Log.QuickLog("Project Saved!");
        } catch(Exception ex) { Log.QuickLog($"Error while saving project! {ex.ToString()}"); };
    }

    async public void SaveAs() { }

    public void SaveToServer()
    {
        var doSave = false;
        if (!doSave) return;
        ProjectSaver.PublishToServer(_project, _editor, server);
    }
}