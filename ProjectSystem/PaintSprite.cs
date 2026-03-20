using System.IO;

namespace PaintPower.ProjectSystem;

public class PaintSprite
{
    public string Name { get; set; } = "";
    public string SpriteFolder { get; set; } = ""; // absolute path in workspace

    public string JsonPath => Path.Combine(SpriteFolder, "Sprite.json");
    public string AnimationPath => Path.Combine(SpriteFolder, "Sprite.wxa");
    public string ThumbnailPath => Path.Combine(SpriteFolder, "Sprite.png");
    public string ScriptPath => Path.Combine(SpriteFolder, "Sprite.pss");
    public string ItemsFolder => Path.Combine(SpriteFolder, "items");
}