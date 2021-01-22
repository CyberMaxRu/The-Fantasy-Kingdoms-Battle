using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Визуальный контрол - кнопка со стоимостью
    internal sealed class VCButtonWithCost : VCButton
    {
        private VCLabel labelCost;
        public VCButtonWithCost(VisualControl parent, int shiftX, int shiftY, ImageList imageList, int imageIndex) : base(parent, shiftX, shiftY, imageList, imageIndex)
        {
            labelCost = new VCLabel(this, 0, Height - 16, FormMain.Config.FontCost, FormMain.Config.CommonCost, 16, "");
            labelCost.Width = Width;
            labelCost.StringFormat.LineAlignment = StringAlignment.Far;
            labelCost.Visible = false;// Текст перекрывается иконкой. Поэтому рисуем вручную
        }

        internal int Cost { get; set; }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Cost >= 0);

            base.Draw(g);

            if (Cost > 0)
            {
                labelCost.Text = Cost.ToString();
                labelCost.Draw(g);
            }
        }
    }
}
