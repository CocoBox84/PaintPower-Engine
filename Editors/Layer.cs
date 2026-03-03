using Avalonia.Media.Imaging;

namespace PaintPower.Editors;

public class Layer
{
    public string Name { get; set; }
    public bool Visible { get; set; } = true;
    public WriteableBitmap Bitmap { get; set; }
    public WriteableBitmap Thumbnail { get; set; }
    public double Opacity { get; set; } = 1.0;
}