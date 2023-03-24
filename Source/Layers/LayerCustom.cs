using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class LayerCustom : VisualControl
    {
        private LayerCustom parentLayer;

        public LayerCustom() : base()
        {
            parentLayer = Program.formMain.currentLayer;

            if (Program.formMain.sizeGamespace.Width > 0)
            {
                Width = Program.formMain.sizeGamespace.Width;
                Height = Program.formMain.sizeGamespace.Height;
            }
        }

        internal static Config Config { get; set; }
        internal static Descriptors Descriptors { get; set; }

        internal virtual void ApplyCurrentWindowSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        internal virtual void PrepareFrame()
        {
            parentLayer?.PrepareFrame();
        }

        internal virtual void BeforeDrawFrame() { }
        internal virtual void PreferencesChanged() { }
    }
}