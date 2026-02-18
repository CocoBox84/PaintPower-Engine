using System;
using System.IO;

namespace PaintPower.ProjectSystem;

public class TempWorkspace
{
    public string Root { get; }
    public string ItemsDir => Path.Combine(Root, "items");

    public TempWorkspace()
    {
        Root = Path.Combine(Path.GetTempPath(), "PaintPower_" + Guid.NewGuid());
        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(ItemsDir);
    }

    public string MapToTemp(string projectRelativePath)
    {
        return Path.Combine(ItemsDir, projectRelativePath.Replace("/", "\\"));
    }

    public void ImportFile(string sourcePath, string relativePath)
    {
        string dest = MapToTemp(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        File.Copy(sourcePath, dest, overwrite: true);
    }

    public void SaveFile(string relativePath, string content)
    {
        string dest = MapToTemp(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        File.WriteAllText(dest, content);
    }

    public void SaveBinary(string relativePath, byte[] data)
    {
        string dest = MapToTemp(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        File.WriteAllBytes(dest, data);
    }

    public byte[] LoadBinary(string relativePath)
    {
        string path = MapToTemp(relativePath);
        return File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>();
    }

    public string LoadText(string relativePath)
    {
        string path = MapToTemp(relativePath);
        return File.Exists(path) ? File.ReadAllText(path) : "";
    }

    public void Delete(string relativePath)
    {
        string path = MapToTemp(relativePath);
        if (File.Exists(path))
            File.Delete(path);
    }

    public void Cleanup()
    {
        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }
}