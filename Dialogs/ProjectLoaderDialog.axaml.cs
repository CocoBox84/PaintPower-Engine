using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Markup.Xaml;

namespace PaintPower.Dialogs;

public partial class ProjectLoaderDialog : Window
{
    private TaskCompletionSource<ProjectLoaderResult?> _tcs;

    public ProjectLoaderDialog()
    {
        AvaloniaXamlLoader.Load(this);
        _tcs = new TaskCompletionSource<ProjectLoaderResult?>();
        this.Closed += (_, __) => _tcs.TrySetResult(null); // ensure completion on close
    }

    public Task<ProjectLoaderResult?> ShowAsync(Window parent)
    {
        ShowDialog(parent);
        return _tcs.Task;
    }

    private async void OnNewProject(object? sender, RoutedEventArgs e)
    {
        var savePicker = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Create New Project",
            DefaultExtension = "paint",
            SuggestedFileName = "NewProject.paint"
        });

        if (savePicker != null)
        {
            _tcs.SetResult(new ProjectLoaderResult
            {
                Mode = ProjectLoaderMode.New,
                Path = savePicker.Path.LocalPath
            });
            Close();
        }
    }

    private async void OnOpenProject(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Project",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("PaintPower Project")
                {
                    Patterns = new[] { "*.paint" }
                }
            }
        });

        if (files.Count > 0)
        {
            _tcs.SetResult(new ProjectLoaderResult
            {
                Mode = ProjectLoaderMode.Open,
                Path = files[0].Path.LocalPath
            });
            Close();
        }
    }

    private void SetResultAndClose(ProjectLoaderResult r)
    {
        _tcs.TrySetResult(r);
        Close();
    } 
}

public class ProjectLoaderResult
{
    public ProjectLoaderMode Mode { get; set; }
    public string Path { get; set; } = "";
}

public enum ProjectLoaderMode
{
    New,
    Open
}