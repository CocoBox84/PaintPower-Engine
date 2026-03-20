using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintPower.Editors;

// Base class for editors like the Paint editor or the Script editor
public partial class EditorBase : UserControl
{
    public virtual void Save() { }
    public virtual void Load() { }

    public string RelativePath { get; private set; } = "";

    public virtual void SetRelativePath(string path)
    {
        RelativePath = path;
    }

    public EditorBase addText(TextBlock t) {
        Content = t;
        return this;
    }

    public string type = "";
}
