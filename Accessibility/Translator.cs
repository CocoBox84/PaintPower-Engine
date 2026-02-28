using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintPower.Accessibility;
public static class Translator
{
    private static Dictionary<string, string> langList = null;
    public static string Map(string s)
    {
        return langList[s];
    }
}
