using Avalonia.Threading;
using PaintPower.Editors;
using PaintPower.Networking;
using Avalonia.Threading;
using System.Threading.Tasks;

// Save to server or on local machine;

namespace PaintPower.ProjectSystem;

class ProjectSaver {
    // Main function that should be called when saving a project.

    public static async Task Save(PaintProject project, EditorBase editor)
    {
        if (editor != null)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                editor.Save();
            });
        }

        if (project != null)
            await project.SaveToDisk();
    }

    async public static void PublishToServer(PaintProject project, EditorBase editor, Server server)
    {
        await PaintPower_Engine.App.Save();
        if (await server.checkConnection() && project != null)
        {
            await server.UploadProject(project);
        }
    }
}