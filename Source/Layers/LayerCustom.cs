using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class LayerCustom : VisualControl
    {
        private LayerCustom parentLayer;

        public LayerCustom(bool active = false) : base()
        {
            Active = active;
            parentLayer = Program.formMain.currentLayer;

            if (Program.formMain.sizeGamespace.Width > 0)
            {
                Width = Program.formMain.sizeGamespace.Width;
                Height = Program.formMain.sizeGamespace.Height;
            }
        }

        internal static Config Config { get; set; }
        internal static Descriptors Descriptors { get; set; }
        internal bool Active { get; private set; }// Слой активен

        internal virtual void ApplyCurrentWindowSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        internal virtual void Deactivated()// Вызывается при деактивации слоя
        {
            Assert(Active);
            Active = false;
        }

        internal virtual void Activated()// Вызывается при активации слоя
        {
            Assert(!Active);
            Active = true;
        }

        internal virtual void Focused(DialogAction da)// Вызывается при получении управления
        {

        }

        internal virtual void PrepareFrame()
        {
            parentLayer?.PrepareFrame();
        }

        internal virtual void BeforeDrawFrame() { }
        internal virtual void PreferencesChanged() { }
    }
}