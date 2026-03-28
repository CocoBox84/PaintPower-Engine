using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;

namespace PaintPower.Dialogs;

public partial class SaveBeforeContinueDialog : Window
{
    public SaveBeforeContinueDialog()
    {
        InitializeComponent();
        Title = "Save Before Continuing?";
    }

    public Task<string?> ShowAsync(Window parent)
    {
        return this.ShowDialog<string?>(parent);
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        Close("save");
    }

    private void OnSaveAs(object? sender, RoutedEventArgs e)
    {
        Close("saveas");
    }

    private void OnDontSave(object? sender, RoutedEventArgs e)
    {
        Close("dontsave");
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}