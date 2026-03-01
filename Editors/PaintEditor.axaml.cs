using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PaintPower.ProjectSystem;
using System;
using System.Collections.Generic;
using System.IO;

namespace PaintPower.Editors;

public partial class PaintEditor : UserControl
{
    private readonly string _relativePath;
    private readonly TempWorkspace _workspace;

    private WriteableBitmap _bitmap;
    private bool _isDrawing;
    private Avalonia.Point _lastPoint;
    private Avalonia.Media.Color _currentColor = Colors.Black;

    private bool _isPanning;
    private Avalonia.Point _panStart;
    private Vector _scrollStart;

    // Undo/Redo setup
    private readonly Stack<WriteableBitmap> _undoStack = new();
    private readonly Stack<WriteableBitmap> _redoStack = new();

    public PaintEditor(string relativePath, TempWorkspace workspace)
    {
        _relativePath = relativePath;
        _workspace = workspace;

        InitializeComponent();

        // Add swatch colors
        ColorSwatches.ItemsSource = new SolidColorBrush[]
{
    new SolidColorBrush(Colors.Black),
    new SolidColorBrush(Colors.White),
    new SolidColorBrush(Colors.Red),
    new SolidColorBrush(Colors.Green),
    new SolidColorBrush(Colors.Blue),
    new SolidColorBrush(Colors.Yellow),
    new SolidColorBrush(Colors.Cyan),
    new SolidColorBrush(Colors.Magenta),
    new SolidColorBrush(Colors.Orange),
    new SolidColorBrush(Colors.Purple),
    new SolidColorBrush(Colors.Brown),
    new SolidColorBrush(Colors.Gray)
};

        LoadOrCreateImage();

        CanvasImage.PointerPressed += OnPointerPressed;
        CanvasImage.PointerReleased += OnPointerReleased;
        CanvasImage.PointerMoved += OnPointerMoved;

        SaveButton.Click += (_, __) => Save();
        EraserButton.Click += (_, __) => _currentColor = Colors.White;

        UndoButton.Click += (_, __) => Undo();
        RedoButton.Click += (_, __) => Redo();
        ClearButton.Click += (_, __) => Clear();

        ZoomSlider.PropertyChanged += (_, e) =>
        {
            if (e.Property == Slider.ValueProperty)
            {
                double scale = ZoomSlider.Value;
                ZoomContainer.LayoutTransform = new ScaleTransform(scale, scale);

                pixelGrid.Zoom = scale;
                pixelGrid.InvalidateVisual();
            }
        };

        ScrollViewer scroll = this.FindControl<ScrollViewer>("CanvasScroll");
        scroll.PointerWheelChanged += OnPointerWheelChanged;

        HandToolButton.Click += (_, __) =>
        {
            _isPanning = !_isPanning;
            HandToolButton.Content = _isPanning ? "Hand Tool (On)" : "Hand Tool";
        };

        scroll.PointerPressed += OnScrollPointerPressed;
        scroll.PointerReleased += OnScrollPointerReleased;
        scroll.PointerMoved += OnScrollPointerMoved;
    }

    private void OnSwatchClicked(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border b && b.Background is SolidColorBrush brush)
            _currentColor = brush.Color;
    }

    private void LoadOrCreateImage()
    {
        var fullPath = _workspace.MapToTemp(_relativePath);

        try
        {

            if (File.Exists(fullPath))
            {
                using var fs = File.OpenRead(fullPath);
                var bmp = new Bitmap(fs); // Not all the time is this valid, so, catch it, and make it valid.

                _bitmap = new WriteableBitmap(bmp.PixelSize, bmp.Dpi, PixelFormat.Bgra8888, AlphaFormat.Premul);

                using (var fb = _bitmap.Lock())
                {
                    // Fix: Provide required PixelRect argument for CopyPixels
                    bmp.CopyPixels(
                        new PixelRect(0, 0, bmp.PixelSize.Width, bmp.PixelSize.Height),
                        fb.Address,
                        fb.RowBytes * fb.Size.Height,
                        fb.RowBytes
                    );
                }
            }
            else
            {
                _bitmap = new WriteableBitmap(
                    new PixelSize(800, 600),
                    new Vector(96, 96),
                    PixelFormat.Bgra8888,
                    AlphaFormat.Premul
                );
            }
        } catch
        {
            _bitmap = new WriteableBitmap(
                new PixelSize(800, 600),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul
            );
        }

        pixelGrid.PixelWidth = _bitmap.PixelSize.Width;
        pixelGrid.PixelHeight = _bitmap.PixelSize.Height;

        CanvasImage.Source = _bitmap;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_isPanning)
            return;

        _isDrawing = true;
        _lastPoint = ToBitmapSpace(e);
        DrawPoint(_lastPoint);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isPanning)
            return;

        _isDrawing = false;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isPanning || !_isDrawing)
            return;

        var point = ToBitmapSpace(e);
        DrawLine(_lastPoint, point);
        _lastPoint = point;
    }

    private void DrawPoint(Avalonia.Point p)
    {
        using var fb = _bitmap.Lock();
        int size = (int)BrushSizeSlider.Value;
        uint color = _currentColor.ToUInt32();

        unsafe
        {
            uint* ptr = (uint*)fb.Address;
            int width = _bitmap.PixelSize.Width;
            int height = _bitmap.PixelSize.Height;

            for (int y = -size; y < size; y++)
            {
                for (int x = -size; x < size; x++)
                {
                    int px = (int)p.X + x;
                    int py = (int)p.Y + y;

                    if (px < 0 || py < 0 || px >= width || py >= height)
                        continue;

                    int offset = py * width + px;
                    ptr[offset] = color;
                }
            }
        }

        CanvasImage.InvalidateVisual();
    }

    private void DrawLine(Avalonia.Point a, Avalonia.Point b)
    {
        int steps = (int)Math.Max(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));

        for (int i = 0; i < steps; i++)
        {
            double t = (double)i / steps;
            var x = a.X + (b.X - a.X) * t;
            var y = a.Y + (b.Y - a.Y) * t;
            DrawPoint(new Avalonia.Point(x, y));
        }
    }

    public void Save()
    {
        var fullPath = _workspace.MapToTemp(_relativePath);

        using var fs = File.Open(fullPath, FileMode.Create);
        _bitmap.Save(fs);
    }

    private Avalonia.Point ToBitmapSpace(PointerEventArgs e)
    {
        // 1. Pointer position relative to ScrollViewer
        var scroll = this.FindControl<ScrollViewer>("CanvasScroll");
        var pos = e.GetPosition(scroll);

        // 2. Transform ScrollViewer → ZoomContainer
        var transform = scroll.TransformToVisual(ZoomContainer);
        var zoomSpace = transform?.Transform(pos) ?? pos;

        // 3. Divide by zoom scale to get bitmap pixel coordinates
        double scale = ZoomSlider.Value;
        return new Avalonia.Point(zoomSpace.X / scale, zoomSpace.Y / scale);
    }

    private WriteableBitmap CloneBitmap(WriteableBitmap source)
    {
        var clone = new WriteableBitmap(
            source.PixelSize,
            source.Dpi,
            source.Format,
            source.AlphaFormat);

        unsafe
        {

            using (var src = source.Lock())
            using (var dst = clone.Lock())
            {
                Buffer.MemoryCopy(
                    src.Address.ToPointer(),
                    dst.Address.ToPointer(),
                    dst.RowBytes * dst.Size.Height,
                    src.RowBytes * src.Size.Height);
            }
        }

        return clone;
    }

    private void Undo()
    {
        if (_undoStack.Count == 0)
            return;

        _redoStack.Push(CloneBitmap(_bitmap));
        _bitmap = _undoStack.Pop();
        CanvasImage.Source = _bitmap;
        CanvasImage.InvalidateVisual();
    }

    private void Redo()
    {
        if (_redoStack.Count == 0)
            return;

        _undoStack.Push(CloneBitmap(_bitmap));
        _bitmap = _redoStack.Pop();
        CanvasImage.Source = _bitmap;
        CanvasImage.InvalidateVisual();
    }

    private void Clear()
    {
        _undoStack.Push(CloneBitmap(_bitmap));
        _redoStack.Clear();

        using var fb = _bitmap.Lock();
        unsafe
        {
            Buffer.MemoryCopy(
                new byte[_bitmap.PixelSize.Width * _bitmap.PixelSize.Height * 4].AsSpan().ToArray().AsMemory().Pin().Pointer,
                fb.Address.ToPointer(),
                fb.RowBytes * fb.Size.Height,
                fb.RowBytes * fb.Size.Height);
        }

        CanvasImage.InvalidateVisual();
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control))
            return;

        var scroll = (ScrollViewer)sender;
        var pos = e.GetPosition(scroll);

        double oldZoom = ZoomSlider.Value;
        double delta = e.Delta.Y > 0 ? 1.1 : 0.9;
        double newZoom = Math.Clamp(oldZoom * delta, ZoomSlider.Minimum, ZoomSlider.Maximum);

        ZoomSlider.Value = newZoom;

        double factor = newZoom / oldZoom;

        scroll.Offset = new Vector(
            (scroll.Offset.X + pos.X) * factor - pos.X,
            (scroll.Offset.Y + pos.Y) * factor - pos.Y
        );

        e.Handled = true;
    }

    private void OnScrollPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var scroll = (ScrollViewer)sender;

        // Middle mouse OR Hand Tool mode
        if (e.GetCurrentPoint(scroll).Properties.IsMiddleButtonPressed || _isPanning)
        {
            _panStart = e.GetPosition(scroll);
            _scrollStart = scroll.Offset;
            scroll.Cursor = new Cursor(StandardCursorType.Hand);
            e.Pointer.Capture(scroll);
        }
    }

    private void OnScrollPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var scroll = (ScrollViewer)sender;
        scroll.Cursor = Cursor.Default;
        e.Pointer.Capture(null);
    }

    private void OnScrollPointerMoved(object? sender, PointerEventArgs e)
    {
        var scroll = (ScrollViewer)sender;

        if (e.Pointer.Captured == scroll)
        {
            var pos = e.GetPosition(scroll);
            var delta = pos - _panStart;

            scroll.Offset = new Vector(
                _scrollStart.X - delta.X,
                _scrollStart.Y - delta.Y
            );
        }
    }
}