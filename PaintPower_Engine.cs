using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PaintPower.Dialogs;
using PaintPower.Editors;
using PaintPower.FileExplorer;
using PaintPower.Logging;
using PaintPower.Networking;
using PaintPower.ProjectSystem;
using PaintPower.SpriteEditor;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace PaintPower;

public class PaintPower_Engine {

    public static string version = "Pre-Alpha 1.0.0.2 build 6053282026";

    private Editor _editorManager;
    private PaintProject _project;
    private EditorBase _editor;
    public Server server;

    public bool isNewProject = true;

    private SpriteEditorView _spriteEditorView;

    public bool saveNeeded = false;

    public static PaintPower_Engine App { get; private set; }
    public static MainWindow window;

    public PaintPower_Engine()
    {
        _project = new PaintProject();
        _editorManager = new Editor(_project.Workspace);
        server = new Server();

        // After, make a static reference.
        App = this;
    }

    public void attachWindow(MainWindow w) {
        window = w;
    }

    public void StatusClicked(object sender, EventArgs e)
    {
        if (_project != null && saveNeeded)
        {
            Save();
        }
    }

    public string SetProjectStatus(string status)
    {
        window.ProjectStatus.Text = status;
        window.InvalidateVisual();
        return status;
    }

    private void OnSpriteSelected(PaintSprite sprite)
    {
        // Create the sprite editor panel
        _spriteEditorView = new SpriteEditorView(sprite, _project.Workspace);

        // Replace the center panel with the sprite editor
        window.CenterHost.Content = _spriteEditorView;

        SetProjectStatus($"Editing Sprite: {sprite.Name}");
    }

    public async void newProject()
    {
        if (saveNeeded)
        {
            var dialog = new SaveBeforeContinueDialog();
            var result = await dialog.ShowAsync(MainWindow.window);

            switch (result)
            {
                case "save":
                    await Save();
                    break;

                case "saveas":
                    SaveAs();
                    break;

                case "dontsave":
                    break; // continue without saving

                case null:
                    return; // cancel new project
            }
        }

        // Reset everything
        _project = new PaintProject();
        _editorManager = new Editor(_project.Workspace);
        server = new Server();

        Start();
    }

    public async void Start() {

        await Task.Yield();

        CloseEditor();

        _project.CreateNew();

        window.Title = $"PaintPower - {_project.Metadata.name}";

        SetProjectStatus("Not edited yet.");

        window.SpriteManager.Initialize(_project);
        window.SpriteManager.SpriteSelected += OnSpriteSelected;
    }

    /*public void OpenEditor(EditorBase editor)
    {
        _editor = editor;
        CenterHost.Content = editor;
    }*/

    public void OpenFile(string path)
    {
        var editor = _editorManager.GetEditorFromFileType(path);

        if (_spriteEditorView != null)
            _spriteEditorView.OpenEditor(editor, path);
    }

    public void CloseEditor()
    {
        window.CenterHost.Content = null;
        _editor = null;
    }

    public bool _isSavingAnimationRunning = false;
    public bool _isUploadAnimationRunning = false;

    public async Task RunSavingAnimation()
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
        if (!saveNeeded && !isNewProject) return;

        try
        {
            var dialog = new DoSaveWindowDialog();
            var result = await dialog.ShowAsync(window);

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

            // If no path yet -> ask user where to save
            if (string.IsNullOrWhiteSpace(_project.ProjectPath) || isNewProject)
            {
                isNewProject = true; // Keep isNewProject checks

                var res = await _project.SaveNewProject(MainWindow.window);

                if (string.IsNullOrWhiteSpace(res.Path))
                    return; // user cancelled

                _project.ProjectPath = res.Path;
                isNewProject = false;
            }

            Log.QuickLog("Saving Project...");

            // Start animation (non-blocking)
            var animationTask = RunSavingAnimation();

            // Run save off UI thread
            await ProjectSaver.Save(_project, _editor);
            

            // Stop animation
            _isSavingAnimationRunning = false;
            await animationTask; // wait for animation loop to exit

            // Final status
            Log.QuickLog(SetProjectStatus("Project Saved!"));
        }
        catch (Exception ex)
        {
            Log.QuickLog($"Error while saving project! {ex}");
        }
    }

    async public void SaveAs()
    {
        var savePicker = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
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
            
            await ProjectSaver.Save(_project, _editor);

            // Stop animation
            _isSavingAnimationRunning = false;
            await animationTask;

            // Update metadata + UI
            _project.Metadata.name = Path.GetFileNameWithoutExtension(newPath);
            window.Title = $"PaintPower - {_project.Metadata.name}";
            SetProjectStatus("Project Saved!");
            Log.QuickLog($"Project saved as {newPath}");
        }
        catch (Exception ex)
        {
            Log.QuickLog($"SaveAs failed: {ex}");
        }
    }

    public async void SaveToServer()
    {
        var doSave = false;
        if (!doSave) return;
        SetProjectStatus("Uploading Project to Server...");
        window.InvalidateVisual();
        ProjectSaver.PublishToServer(_project, _editor, server);
        SetProjectStatus("");
    }
}