using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using PaintPower.Accessibility.Translation;

namespace PaintPower.VMPanel;

public partial class VmPanel : UserControl
{
    public VmPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void refresh()
    {
        translate();
        InvalidateVisual();
    }

    public void translate()
    {
        if (VMPanelText != null) VMPanelText.Text = Translator.Map("VM Panel");
    }
}