using PaintPower.Networking;
using PaintPower.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Save to server or on local machine;

namespace PaintPower.ProjectSystem;

class ProjectSaver {
    // Main function that should be called when saving a project.
    public static void Save(PaintProject project, EditorBase editor)
    {
        if (editor != null) editor.Save(); // Save editor first
        if (project != null) project.SaveToDisk(); // Make zip from temp
    }

    async public static void PublishToServer(PaintProject project, EditorBase editor, Server server)
    {
        await new MainWindow().Save();
        if (server.checkConnection() && project != null)
        {
            server.uploadProject(project);
        }
    }
}