using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Save to server or on local machine;

namespace PaintPower.ProjectSystem;

class ProjectSaver {
    public static void Save(PaintProject project)
    {
        project.SaveToDisk();
    }

    public static void PublishToServer(PaintProject project)
    {
    }
}