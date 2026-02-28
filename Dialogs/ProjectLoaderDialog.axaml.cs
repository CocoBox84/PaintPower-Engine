using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Markup.Xaml;

namespace PaintPower.Dialogs;

public partial class ProjectLoaderDialog : Window
{
    private TaskCompletionSource<ProjectLoaderResult?> _tcs;

    public ProjectLoaderDialog() => AvaloniaXamlLoader.Load(this);

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
            DefaultExtension = "xPaint",
            SuggestedFileName = "NewProject.xPaint"
        });

        if (savePicker != null)
        {
            var r = (new ProjectLoaderResult
            {
                Mode = ProjectLoaderMode.New,
                Path = savePicker.Path.LocalPath
            });
            Close(r);
        } else {
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
                    Patterns = new[] { "*.xPaint", "*.xpaint", "*.Paint", "*.paint" }
                }
            }
        });

        if (files.Count > 0)
        {
            var r = new ProjectLoaderResult { Mode = ProjectLoaderMode.Open, Path = files[0].Path.LocalPath };
            Close(r);
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