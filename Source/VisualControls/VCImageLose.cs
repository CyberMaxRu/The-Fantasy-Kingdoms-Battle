using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - информация о поражении
    internal sealed class VCImageLose : VCImage
    {
        public VCImageLose(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, Program.formMain.ilGui24, -1)
        {
        }

        internal LoseInfo Info { get; set; }

        internal override void Draw(Graphics g)
        {
            ImageIndex = Info is null ? FormMain.GUI_24_NEUTRAL : FormMain.GUI_24_LOSE;

            base.Draw(g);
        }

        internal override bool PrepareHint()
        {
            if (!(Info is null))
            {
                Program.formMain.formHint.AddStep1Header($"Поражение от: {Info.Opponent.GetName()}", "", $"День: {Info.Day}");
            }

            return true;
        }
    }
}
