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

        public LayerCustom() : base()
        {
            if (Program.formMain.sizeGamespace.Width > 0)
            {
                Width = Program.formMain.sizeGamespace.Width;
                Height = Program.formMain.sizeGamespace.Height;
            }
        }

        internal static Config Config { get; set; }
        internal static Descriptors Descriptors { get; set; }
        internal bool Enabled { get => enabled; set { if (enabled != value) { enabled = value; OnEnabledChanged(); } } }

        internal virtual void ApplyCurrentWindowSize()
        {
            Width = Program.formMain.sizeGamespace.Width;
            Height = Program.formMain.sizeGamespace.Height;
        }

        protected virtual void OnEnabledChanged()
        {

        }

        internal virtual void BeforeDrawFrame()
        {

        }

        internal virtual void PrepareFrame()
        {

        }

        internal virtual void PreferencesChanged() { }
    }
}