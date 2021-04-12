using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - гиперссылка
    internal sealed class VCLink : VCLabelM2
    {
        public VCLink(VisualControl parent, int shiftX, int shiftY, string text, string link) : base(parent, shiftX, shiftY, Program.formMain.fontMedCaption, Color.HotPink, 16, text)
        {
            Link = link;
            Width = Font.WidthText(Text);
        }

        internal string Link { get; set; }

        internal override void DoClick()
        {
            base.DoClick();

            if (Link.Length > 0)
                Process.Start(Link);
        }

        internal override void Draw(Graphics g)
        {
            Color = MouseEntered ? Color.HotPink : Color.LightSkyBlue;

            base.Draw(g);
        }

    }
}
