using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;

using PaintPower.Accessibility.Translation;

namespace PaintPower.Dialogs;

public partial class InputDialog : Window
{
    private TaskCompletionSource<string?> _tcs;

    public InputDialog(string title, string prompt)
    {
        InitializeComponent();
        Title = Translator.Map(title);
        PromptText.Text = Translator.Map(prompt);
        _tcs = new TaskCompletionSource<string?>();
        this.Closed += (_, __) => _tcs.TrySetResult(null);
    }

    public Task<string?> ShowAsync(Window parent)
    {
        ShowDialog(parent);
        return _tcs.Task;
    }

    private void OnOk(object? sender, RoutedEventArgs e)
    {
        _tcs.SetResult(InputBox.Text);
        Close();
    }
}