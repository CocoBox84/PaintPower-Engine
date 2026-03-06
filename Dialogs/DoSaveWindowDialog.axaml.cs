using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PaintPower.Dialogs;

public partial class DoSaveWindowDialog : Window
{
    private TaskCompletionSource<string?> _tcs;

    public DoSaveWindowDialog()
    {
        InitializeComponent();
        Title = "Overwrite your old data?!";
        PromptText.Text = "Do you want to overwrite your save data?";
        _tcs = new TaskCompletionSource<string?>();
        this.Closed += (_, __) => _tcs.TrySetResult(null);
    }

    public Task<string?> ShowAsync(Window parent)
    {
        ShowDialog(parent);
        return _tcs.Task;
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        _tcs.SetResult("save");
        Close();
    }

    private void OnSaveAs(object? sender, RoutedEventArgs e)
    {
        _tcs.SetResult("saveas");
        Close();
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        _tcs.SetResult(null);
        Close();
    }
}