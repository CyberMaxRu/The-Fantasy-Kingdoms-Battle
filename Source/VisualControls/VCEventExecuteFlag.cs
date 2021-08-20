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
        private VCButton btnViewResult;
        private VCButton btnWShowBattle;

        public VCEventExecuteFlag(TypeFlag typeFlag, TypeConstruction tl, PlayerConstruction pl, bool winner, Battle b) : base()
        {
            TypeFlag = typeFlag;
            TypeLair = tl;
            Target = pl;
            Winner = winner;
            Battle = b;

            bmpTypeFlag = new VCImage(this, FormMain.Config.GridSize, FormMain.Config.GridSize, Program.formMain.ilGui, LobbyPlayer.TypeFlagToImageIndex(typeFlag));
            bmpTypeFlag.ShowBorder = true;
            bmpTarget = new VCImage(this, bmpTypeFlag.NextLeft(), bmpTypeFlag.ShiftY, Program.formMain.imListObjects48, tl.ImageIndex);
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

            btnViewResult = new VCButton(this, bmpWin.NextLeft(), 16, "Итоги");
            btnViewResult.Enabled = !(b is null);
            btnViewResult.Click += BtnViewResult_Click;
            btnWShowBattle = new VCButton(this, btnViewResult.NextLeft(), 16, "Просмотр");
            btnWShowBattle.Enabled = !(b is null);
            btnWShowBattle.Click += BtnWShowBattle_Click;

            ApplyMaxSize();

            Height += FormMain.Config.GridSize;
            Width += FormMain.Config.GridSize;
        }

        private void BtnWShowBattle_Click(object sender, EventArgs e)
        {
            //WindowBattle w = new WindowBattle(Battle);
            //w.ShowBattle();
            //w.Dispose();
        }

        private void BtnViewResult_Click(object sender, EventArgs e)
        {
            WindowResultBattle w = new WindowResultBattle(Battle);
            w.ShowDialog();
            w.Dispose();
        }

        private void BmpTarget_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(Target);
        }

        internal TypeFlag TypeFlag { get; }
        internal TypeConstruction TypeLair { get; }
        internal PlayerConstruction Target { get; }
        internal bool Winner { get; }
        internal Battle Battle { get; }
    }
}
