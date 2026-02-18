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