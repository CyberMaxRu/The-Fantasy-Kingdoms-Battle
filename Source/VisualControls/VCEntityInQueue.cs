using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс контрола - кнопка в очереди строительства/исследования
    internal sealed class VCEntityInQueue : VCImage
    {
        public VCEntityInQueue(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, Program.formMain.imListObjects32, -1)
        {

        }

        internal ActionForEntity Action { get; set; }

        internal override void Draw(Graphics g)
        {
            if (Visible)
                g.DrawImageUnscaled(Program.formMain.bmpBackgroundEntityInQueue, Left - 1, Top - 1);

            ImageIndex = Action != null ? Action.GetImageIndex() : -1;
            RestTimeExecuting = Action != null ? Action.GetExtInfo() : "";

            base.Draw(g);
        }
    }
}
