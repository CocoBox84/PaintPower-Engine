using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace PaintPower.ProjectSystem;

public class PaintProject
{
    public string ProjectPath { get; set; } = "";
    public TempWorkspace Workspace { get; }
    public ProjectMetadata Metadata { get; private set; }

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
}

// -------------------------
// PROJECT METADATA STRUCT
// -------------------------
public class ProjectMetadata
{
    public string name { get; set; } = "Untitled Project";
    public string? OpenFile { get; set; }
}