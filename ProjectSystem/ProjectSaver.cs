using PaintPower.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Save to server or on local machine;

namespace PaintPower.ProjectSystem;

class ProjectSaver {
    // Main function that should be called when saving a project.
    public static void Save(PaintProject project)
    {
        Editors.Editor.SaveEditor(); // Save in temp
        project.SaveToDisk(); // Make zip from temp
    }

    public static void PublishToServer(PaintProject project, Server server)
    {
        // To add later
    }
}