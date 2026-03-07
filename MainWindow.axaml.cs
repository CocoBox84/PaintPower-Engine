using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PaintPower.Dialogs;
using PaintPower.Editors;
using PaintPower.FileExplorer;
using PaintPower.Logging;
using PaintPower.Networking;
using PaintPower.ProjectSystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace PaintPower;

public partial class MainWindow : Window
{
    private readonly Editor _editorManager;
    private readonly PaintProject _project;
    private EditorBase _editor;
    public Server server;

    public bool saveNeeded = false;

    public static MainWindow App { get; private set; }

    public MainWindow()
    {
        InitializeComponent();

        _project = new PaintProject();
        _editorManager = new Editor(_project.Workspace);
        server = new Server();
        SaveButton.Click += (_, __) => Save();
        SaveAsButton.Click += (_, __) => SaveAs();
        ProjectStatus.PointerPressed += StatusClicked;

        // After, make a static reference.
        App = this;
    }

    void StatusClicked(object sender, EventArgs e)
    {
        if (_project != null && saveNeeded)
        {
            Save();
        }
    }

    public string SetProjectStatus(string status)
    {
        ProjectStatus.Text = status;
        this.InvalidateVisual();
        return status;
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        await Task.Yield();

        var dialog = new ProjectLoaderDialog();
        var result = await new ProjectLoaderDialog().ShowDialog<ProjectLoaderResult?>(this);
        if (result == null) { Close(); return; }

        if (result.Mode == ProjectLoaderMode.New)
            _project.CreateNew(result.Path, Path.GetFileNameWithoutExtension(result.Path));
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

    private bool _isSavingAnimationRunning = false;

    private async Task RunSavingAnimation()
    {
        _isSavingAnimationRunning = true;

        string[] frames = new[]
        {
        "Saving Project",
        "Saving Project.",
        "Saving Project..",
        "Saving Project..."
    };

        int index = 0;

        while (_isSavingAnimationRunning)
        {
            SetProjectStatus(frames[index]);
            index = (index + 1) % frames.Length;
            await Task.Delay(300); // smooth animation
        }
    }

    // Save function. Don't even care about what it returns, but C#
    // Requires it in order to await it. C# core.
    async public Task Save()
    {
        if (!saveNeeded) return;

        try
        {
            var dialog = new DoSaveWindowDialog();
            var result = await dialog.ShowAsync(this);

            if (result == "saveas")
            {
                SaveAs();
                return;
            }

            if (result != "save")
            {
                Log.QuickLog($"Not saving. {result}");
                return;
            }

            Log.QuickLog("Saving Project...");

            // Start animation (non-blocking)
            var animationTask = RunSavingAnimation();

            // Run save off UI thread
            await Task.Run(() =>
            {
                ProjectSaver.Save(_project, _editor);
            });

            // Stop animation
            _isSavingAnimationRunning = false;
            await animationTask; // wait for animation loop to exit

            // Final status
            Log.QuickLog(SetProjectStatus("Project Saved!")); 2146362026;
        }
        catch (Exception ex)
        {
            Log.QuickLog($"Error while saving project! {ex}");
        }
    }

    async public void SaveAs()
    {
        var savePicker = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Save {_project.Metadata.name} As",
            DefaultExtension = "xPaint",
            SuggestedFileName = $"{_project.Metadata.name}.xPaint",
            ShowOverwritePrompt = true
        });

        if (savePicker == null)
            return;

        string newPath = savePicker.Path.LocalPath;

        // Update project path
        _project.ProjectPath = newPath;

        try
        {
            // Start animation
            var animationTask = RunSavingAnimation();

            // Run save off UI thread
            await Task.Run(() =>
            {
                ProjectSaver.Save(_project, _editor);
            });

            // Stop animation
            _isSavingAnimationRunning = false;
            await animationTask;

            // Update metadata + UI
            _project.Metadata.name = Path.GetFileNameWithoutExtension(newPath);
            Title = $"PaintPower - {_project.Metadata.name}";
            SetProjectStatus("Project Saved!");
            Log.QuickLog($"Project saved as {newPath}");
        }
        catch (Exception ex)
        {
            Log.QuickLog($"SaveAs failed: {ex}");
        }
    }

    public void SaveToServer()
    {
        var doSave = false;
        if (!doSave) return;
        ProjectSaver.PublishToServer(_project, _editor, server);
    }
}