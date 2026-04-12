/*
    File containing routes for the interweb, if you want to create your own server,
    then use these routes or modify them for your server.
*/

namespace PaintPower.Networking;

public class Routes
{

    // Upload project routes
    public string uploadNew() {
        return "api/projects/new/upload/paintfile/";
    }

    public string uploadUpdate(string id)
    {
        return $"api/projects/{id}/upload/paintfile/";
    }

    // Download project routes

    public string downloadProject(string id)
    {
        return $"api/projects/{id}/download";
    }
}
