using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintPower.Editors;

public partial class EditorBase : UserControl
{
    public virtual void Save() { }
    public virtual void Load() { }
    public EditorBase addText(TextBlock t) {
        Content = t;
        return this;
    }
}
