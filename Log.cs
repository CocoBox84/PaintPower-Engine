using System;

public static class Log
{
    private static void LogToServer(string message, bool serverError = true) {
        if (serverError)
        {
            return; // Prevent infinte loop!
        }
        // TODO: Add server logging logic.
    }
    public static void Error(Exception ex) => Console.WriteLine(ex.ToString());
    public static void Info(string msg) => Console.WriteLine(msg);
}