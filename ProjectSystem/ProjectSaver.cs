using Avalonia.Threading;
using PaintPower.Editors;
using PaintPower.Networking;
using Avalonia.Threading;

// Save to server or on local machine;

namespace PaintPower.ProjectSystem;

class ProjectSaver {
    // Main function that should be called when saving a project.

public static void Save(PaintProject project, EditorBase editor)
{
    // 1. Run editor.Save() on UI thread (safe)
    if (editor != null)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            editor.Save(); // touches UI safely
        }).Wait(); // wait for UI thread to finish
    }

    // 2. Run project save on background thread (safe)
    if (project != null)
        project.SaveToDisk();
}

async public static void PublishToServer(PaintProject project, EditorBase editor, Server server)
    {
        await PaintPower_Engine.App.Save();
        if (server.checkConnection() && project != null)
        {
            await server.UploadProject(project);
        }
    }
}