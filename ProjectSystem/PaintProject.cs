using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace PaintPower.ProjectSystem;

public class PaintProject
{
    public string ProjectPath { get; set; } = ""; // Path to zip file.
    public TempWorkspace Workspace { get; }
    public ProjectMetadata Metadata { get; private set; }

    public List<PaintSprite> Sprites { get; private set; } = new(); // Sprite list

    public string ProjectName { get; private set; } = string.Empty;

    public PaintProject()
    {
        Workspace = new TempWorkspace();
        Metadata = new ProjectMetadata();
    }

    // -------------------------
    // CREATE NEW PROJECT
    // -------------------------
    public void CreateNew(string projectPath, string Name)
    {
        ProjectPath = projectPath;
        Metadata = new ProjectMetadata
        {
            name = Name,
            OpenFile = null
        };

        SaveMetadata();
        SaveToDisk();
    }

    // -------------------------
    // LOAD EXISTING PROJECT
    // -------------------------
    public void Load(string projectPath)
    {
        ProjectPath = projectPath;

        if (Directory.Exists(Workspace.Root)) { }

        // Extract ZIP into temp workspace
        ZipFile.ExtractToDirectory(projectPath, Workspace.Root, overwriteFiles: true);

        // Load metadata
        string metaPath = Path.Combine(Workspace.Root, "project.json");
        if (File.Exists(metaPath))
        {
            string json = File.ReadAllText(metaPath);
            Metadata = JsonSerializer.Deserialize<ProjectMetadata>(json) ?? new ProjectMetadata();
        }
        else
        {
            Metadata = new ProjectMetadata();
        }

        // Now that the project is loaded
        LoadSprites();
    }

    // -------------------------
    // SAVE PROJECT AS ZIP
    // -------------------------
    public void SaveToZip()
    {
        if (string.IsNullOrWhiteSpace(ProjectPath))
            throw new Exception("ProjectPath is not set.");

        // Update metadata file
        SaveMetadata();

        // Recreate ZIP
        if (File.Exists(ProjectPath))
            File.Delete(ProjectPath);

        ZipFile.CreateFromDirectory(Workspace.Root, Path.Combine(Workspace.Root, "..", "Paintfile", "file.zip"));
    }
    public void ClearTempZip() {
        File.Delete(Path.Combine(Workspace.Root, "..", "Paintfile", "file.zip"));
    }
    // -------------------------
    // SAVE PROJECT
    // -------------------------
    public void SaveToDisk()
    {
        if (string.IsNullOrWhiteSpace(ProjectPath))
            throw new Exception("ProjectPath is not set.");

        // Update metadata file
        SaveMetadata();

        // Recreate ZIP
        if (File.Exists(ProjectPath))
            File.Delete(ProjectPath);

        ZipFile.CreateFromDirectory(Workspace.Root, ProjectPath);
    }

    private void SaveMetadata()
    {
        string json = JsonSerializer.Serialize(Metadata, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(Workspace.Root, "project.json"), json);
    }


    public void LoadSprites()
    {
        string spritesDir = Path.Combine(Workspace.ItemsDir, "sprites");

        if (!Directory.Exists(spritesDir))
            return;

        foreach (var dir in Directory.GetDirectories(spritesDir))
        {
            var sprite = new PaintSprite
            {
                Name = Path.GetFileName(dir),
                SpriteFolder = dir
            };

            Sprites.Add(sprite);
        }
    }
}

// -------------------------
// PROJECT METADATA STRUCT
// -------------------------
public class ProjectMetadata
{
    public string name { get; set; } = "Untitled Project";
    public string? OpenFile { get; set; }
}