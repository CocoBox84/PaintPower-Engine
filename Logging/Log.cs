using PaintPower.Logging;
using System;
using System.Diagnostics;

public static class Log
{
    public static void log(string message, int level)
    {
        switch (level)
        {
            case 0: Console.WriteLine(new Exception(message).Message); break;
            case 1: Console.WriteLine(message); break;
        }
    }
    // Made to just log something real quick
    public static void QuickLog(string message) {
        Info(message);
        Debug.WriteLine(message);
    }
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