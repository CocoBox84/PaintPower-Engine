using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.IO;
using PaintPower.ProjectSystem;
using PaintPower.Dialogs;
using System;
using System.Threading.Tasks;

namespace PaintPower.FileExplorer;

public partial class ExplorerView : UserControl
{
    public ObservableCollection<ExplorerItem> Items { get; } = new();

    private string _currentDir = "";
    private TempWorkspace _workspace;

    public ExplorerView()
    {
        InitializeComponent();
    }

    // Called by MainWindow after project loads
    public void Initialize(TempWorkspace workspace)
    {
        _workspace = workspace;
        _currentDir = workspace.ActiveRoot;

        FileList.ItemsSource = Items;
        Refresh();
    }

    // Called when switching between project root and sprite root
    public void SetRoot(string newRoot)
    {
        _currentDir = newRoot;
        Refresh();
    }

    private void Refresh()
    {
        Items.Clear();

        if (_workspace == null || !Directory.Exists(_currentDir))
        {
            PathLabel.Text = "(no project)";
            return;
        }

        // Compute relative path inside ActiveRoot
        string relative = _currentDir.Replace(_workspace.ActiveRoot, "")
                                     .Replace("\\", "/");

        if (string.IsNullOrEmpty(relative))
            relative = "/";

        PathLabel.Text = relative;

        // Folders first
        foreach (var dir in Directory.GetDirectories(_currentDir))
        {
            Items.Add(new ExplorerItem
            {
                Name = Path.GetFileName(dir),
                FullPath = dir,
                IsDirectory = true
            });
        }

        // Files
        foreach (var file in Directory.GetFiles(_currentDir))
        {
            Items.Add(new ExplorerItem
            {
                Name = Path.GetFileName(file),
                FullPath = file,
                IsDirectory = false
            });
        }
    }

    // -----------------------------
    // Navigation
    // -----------------------------
    private void OnGoRoot(object? sender, RoutedEventArgs e)
    {
        _currentDir = _workspace.ActiveRoot;
        Refresh();
    }

    private void OnGoUp(object? sender, RoutedEventArgs e)
    {
        if (_currentDir == _workspace.ActiveRoot)
            return;

        _currentDir = Directory.GetParent(_currentDir)!.FullName;
        Refresh();
    }

    // -----------------------------
    // Create File / Folder
    // -----------------------------
    private async void OnNewFile(object? sender, RoutedEventArgs e)
    {
        if (_workspace == null)
            return;

        var dialog = new InputDialog("New File", "Enter file name:");
        var window = this.VisualRoot as Window;
        var name = await dialog.ShowAsync(window);

        if (string.IsNullOrWhiteSpace(name))
            return;

        string path = Path.Combine(_currentDir, name);

        if (!File.Exists(path) && !Directory.Exists(path))
            File.WriteAllText(path, "");
        else
            await ShowErrorPopup();

        MainWindow.App.SetProjectStatus("Save Project");
        MainWindow.App.saveNeeded = true;

        Refresh();
    }

    private async void OnNewFolder(object? sender, RoutedEventArgs e)
    {
        var dialog = new InputDialog("New Folder", "Enter folder name:");
        var window = this.VisualRoot as Window;
        var name = await dialog.ShowAsync(window);

        if (string.IsNullOrWhiteSpace(name))
            return;

        string path = Path.Combine(_currentDir, name);

        if (!Directory.Exists(path) && !File.Exists(path))
            Directory.CreateDirectory(path);
        else
            await ShowErrorPopup();

        MainWindow.App.SetProjectStatus("Save Project");
        MainWindow.App.saveNeeded = true;

        Refresh();
    }

    private async Task ShowErrorPopup()
    {
        var dialog = new PopupWindowDialog(
            "File/Folder Creation Error!",
            "File or folder already exists in this directory!",
            "Error"
        );

        var window = this.VisualRoot as Window;
        try { await dialog.ShowAsync(window); }
        catch { }
    }

    // -----------------------------
    // File selection
    // -----------------------------
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // No action on single click
    }

    private void OnItemDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem is not ExplorerItem item)
            return;

        if (item.IsDirectory)
        {
            _currentDir = item.FullPath;
            Refresh();
            return;
        }

        // Open file in editor
        if (VisualRoot is MainWindow main)
            main.OpenFile(item.FullPath);
    }
}