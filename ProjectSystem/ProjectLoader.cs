using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaintPower.ProjectSystem;

public class ProjectLoader
{
    public void LoadDefaultProject(PaintProject project)
    {
        // Path to your embedded default project
        string defaultZip = "Assets/Untitled.xPaint";

        if (!File.Exists(defaultZip))
            throw new FileNotFoundException("Default project not found", defaultZip);

        // Extract into the project's workspace
        ZipFile.ExtractToDirectory(defaultZip, project.Workspace.Root, overwriteFiles: true);

        // Load metadata
        string metaPath = Path.Combine(project.Workspace.Root, "project.json");
        if (File.Exists(metaPath))
        {
            string json = File.ReadAllText(metaPath);
            project.Metadata = JsonSerializer.Deserialize<ProjectMetadata>(json) ?? new ProjectMetadata();
        }

        // Load sprites
        project.LoadSprites();
    }
}