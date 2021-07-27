using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCEventExecuteFlag : VCEvent
    {
        private VCImage bmpTypeFlag;
        private VCImage bmpTarget;
        private VCLabel textColon;
        private VCImage bmpWin;

        public VCEventExecuteFlag(TypeFlag typeFlag, TypeLair tl, PlayerLair pl, bool winner, Battle b) : base()
        {
            TypeFlag = typeFlag;
            TypeLair = tl;
            Target = pl;
            Winner = winner;
            Battle = b;

            bmpTypeFlag = new VCImage(this, FormMain.Config.GridSize, FormMain.Config.GridSize, Program.formMain.ilGui, LobbyPlayer.TypeFlagToImageIndex(typeFlag));
            bmpTypeFlag.ShowBorder = true;
            bmpTarget = new VCImage(this, bmpTypeFlag.NextLeft(), bmpTypeFlag.ShiftY, Program.formMain.imListObjectsCell, tl.ImageIndex);
            bmpTarget.ShowBorder = true;
            if (Target is null)
            {
                bmpTarget.ImageIsEnabled = false;
            }
            else
            {
                bmpTarget.Click += BmpTarget_Click;
            }
            textColon = new VCLabel(this, bmpTarget.NextLeft(), bmpTypeFlag.ShiftY, Program.formMain.fontBigCaptionC, Color.White, bmpTypeFlag.Height, ":");
            textColon.StringFormat.Alignment = StringAlignment.Center;
            textColon.StringFormat.LineAlignment = StringAlignment.Center;
            textColon.Width = textColon.Font.WidthText(textColon.Text);
            bmpWin = new VCImage(this, textColon.NextLeft(), bmpTypeFlag.ShiftY, Program.formMain.ilGui, winner ? FormMain.GUI_WIN : FormMain.GUI_LOSE);
            bmpWin.ShowBorder = true;

            ApplyMaxSize();

            Height += FormMain.Config.GridSize;
            Width += FormMain.Config.GridSize;
        }

        private void BmpTarget_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(Target);
        }

        internal TypeFlag TypeFlag { get; }
        internal TypeLair TypeLair { get; }
        internal PlayerLair Target { get; }
        internal bool Winner { get; }
        internal Battle Battle { get; }
    }
}
