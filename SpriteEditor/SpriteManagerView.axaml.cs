using Avalonia.Controls;
using Avalonia.Interactivity;
using PaintPower.ProjectSystem;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace PaintPower.SpriteEditor;

public partial class SpriteManagerView : UserControl
{
    public ObservableCollection<PaintSprite> Sprites { get; } = new();

    private PaintProject _project;

    public event Action<PaintSprite>? SpriteSelected;

    public SpriteManagerView()
    {
        InitializeComponent();
        SpriteList.ItemsSource = Sprites;
    }

    public void Initialize(PaintProject project)
    {
        _project = project;
        Sprites.Clear();

        foreach (var sprite in project.Sprites)
            Sprites.Add(sprite);
    }

    private void OnSpriteSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (SpriteList.SelectedItem is PaintSprite sprite)
            SpriteSelected?.Invoke(sprite);
    }

    private void OnSpriteDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (SpriteList.SelectedItem is PaintSprite sprite)
            SpriteSelected?.Invoke(sprite);
    }

    private void OnNewSprite(object? sender, RoutedEventArgs e)
    {
        var name = "NewSprite_" + (Sprites.Count + 1);

        string spritesDir = Path.Combine(_project.Workspace.ItemsDir, "sprites");
        Directory.CreateDirectory(spritesDir);

        string folder = Path.Combine(spritesDir, name);
        Directory.CreateDirectory(folder);

        // Create default files
        File.WriteAllText(Path.Combine(folder, "Sprite.json"), "{}");
        File.WriteAllText(Path.Combine(folder, "Sprite.pss"), "");
        File.WriteAllBytes(Path.Combine(folder, "Sprite.png"), Array.Empty<byte>());
        File.WriteAllBytes(Path.Combine(folder, "Sprite.wxa"), Array.Empty<byte>());
        Directory.CreateDirectory(Path.Combine(folder, "items"));

        var sprite = new PaintSprite
        {
            Name = name,
            SpriteFolder = folder
        };

        _project.Sprites.Add(sprite);
        Sprites.Add(sprite);
    }

    private void OnImportSprite(object? sender, RoutedEventArgs e)
    {
        // TODO: Implement .pSprite import
    }
}