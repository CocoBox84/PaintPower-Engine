using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintPower.Editors;

// Valid filenames for each type of editor.
public class EditorTypes
{
    public static string[] Paint = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp" };
    public static string[] Script = { ".txt", ".cs", ".json", ".xml", ".lua", ".py", ".coco", ".pss" };
    public static string[] Animation = { ".wxa" };
    public static string[] Video = { ".mp4", ".mov" };

    // Find editor from list of supported file extentions.
    public static string FindEditorFromExt(string ext)
    {
        // Check each list for the extention.

        var result = "?";

        // No need for a try/catch really, I just added it just in case.
        try
        {
            // Paint editor.
            if (Paint.Contains(ext))
            {
                result = "Paint";
            }
            // Script Editor
            else if (Script.Contains(ext))
            {
                result = "Script";
            }
            // Animation Editor (Animator)
            else if (Animation.Contains(ext))
            {
                result = "Animation";
            }
            // Video Editor
            else if (Video.Contains(ext))
            {
                result = "Video";
            }
            // Something else was put.
            else
            {
                result = "?";
            }
        }
        catch (Exception ex) {
            result = "?";
        }

        return result;
    }
}