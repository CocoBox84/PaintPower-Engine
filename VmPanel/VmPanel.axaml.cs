using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
}