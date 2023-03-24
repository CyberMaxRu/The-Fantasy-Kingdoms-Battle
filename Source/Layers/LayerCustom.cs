using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class LayerCustom : VisualControl
    {
        private bool enabled = true;
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

        internal virtual void ApplyCurrentWindowSize()
        {
            Width = Program.formMain.sizeGamespace.Width;
            Height = Program.formMain.sizeGamespace.Height;
        }

        internal virtual void BeforeDrawFrame()
        {

        }

        internal virtual void PrepareFrame()
        {
            parentLayer?.PrepareFrame();
        }

        internal virtual void PreferencesChanged() { }
    }
}