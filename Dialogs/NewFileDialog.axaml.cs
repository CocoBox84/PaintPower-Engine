/*

namespace PaintPower.Templates.FileTemplates;

public class  : FileTemplate
{
    public ()
    {
        filetype = "";
        name = "";
        description = "";
        category = "";
    }
}

*/

using Avalonia.Controls;
using Avalonia.Interactivity;
using PaintPower.Templates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaintPower.Dialogs;

public partial class NewFileDialog : Window
{
    public FileList FileTemplates { get; set; } = new FileList();
    
    public NewFileDialog()
    {
        InitializeComponent();
        FileTemplateList.ItemsSource = FileTemplates.templates;
    }

    public Task<string?> ShowAsync(Window parent)
    {
        // This returns a Task<string?> that completes when Close(result) is called
        return this.ShowDialog<string?>(parent);
    }

    public void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    public void OnSelectTemplate(object? sender, RoutedEventArgs e)
    {
        
    }
}