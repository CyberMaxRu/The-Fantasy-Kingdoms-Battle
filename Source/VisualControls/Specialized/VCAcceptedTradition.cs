using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle.Source
{
    // Визуальный контрол - принятая традиция
    internal sealed class VCAcceptedTradition : VisualControl
    {
        public VCAcceptedTradition(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            CellTypeTradition = new VCCellSimple(this, 0, 0);
            TextName = new VCText(this, CellTypeTradition.NextLeft(), 0, Program.formMain.FontParagraphC, Color.White, 264);
            TextName.StringFormat.Alignment = StringAlignment.Near;
            TextName.StringFormat.LineAlignment = StringAlignment.Near;
            TextName.Height = CellTypeTradition.Height;
            LblLevel = new VCLabel(CellTypeTradition, 0, 0, Program.formMain.FontBigCaptionC, Color.White, CellTypeTradition.Height, "");
            LblLevel.Width = CellTypeTradition.Width;
            LblLevel.StringFormat.Alignment = StringAlignment.Center;
            LblLevel.StringFormat.LineAlignment = StringAlignment.Center;
            LblLevel.IsActiveControl = false;

            Height = CellTypeTradition.Height;
            Width = TextName.NextLeft();
        }

        internal VCCellSimple CellTypeTradition { get; }
        internal VCText TextName { get; }
        internal VCLabel LblLevel { get;
        }
    }
}
