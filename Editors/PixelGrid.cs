using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PaintPower.Editors;

public class PixelGrid : Control
{
    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<PixelGrid, double>(nameof(Zoom));

    public double Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public static readonly StyledProperty<int> PixelWidthProperty =
        AvaloniaProperty.Register<PixelGrid, int>(nameof(PixelWidth));

    public int PixelWidth
    {
        get => GetValue(PixelWidthProperty);
        set => SetValue(PixelWidthProperty, value);
    }

    public static readonly StyledProperty<int> PixelHeightProperty =
        AvaloniaProperty.Register<PixelGrid, int>(nameof(PixelHeight));

    public int PixelHeight
    {
        get => GetValue(PixelHeightProperty);
        set => SetValue(PixelHeightProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        if (Zoom < 4) return; // Only show grid when zoomed in enough

        var pen = new Pen(Brushes.Gray, 1);

        double step = Zoom; // Each pixel becomes Zoom units wide

        for (double x = 0; x <= PixelWidth * Zoom; x += step)
            context.DrawLine(pen, new Point(x, 0), new Point(x, PixelHeight * Zoom));

        for (double y = 0; y <= PixelHeight * Zoom; y += step)
            context.DrawLine(pen, new Point(0, y), new Point(PixelWidth * Zoom, y));
    }
}