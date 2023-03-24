using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс события - битва между игроками
    internal sealed class VCEventBattle : VCEvent
    {
        private VCButton btnViewResult;
        private VCButton btnWShowBattle;

        public VCEventBattle(Battle b) : base()
        {
            Battle = b;

            //
            VCCell cellPlayer1 = new VCCell(this, FormMain.Config.GridSize, FormMain.Config.GridSize);
            cellPlayer1.Entity = b.Player1;

            VCImage24 im1 = new VCImage24(this, cellPlayer1.NextLeft(), cellPlayer1.ShiftY, 
                b.Winner == b.Player1 ? FormMain.GUI_24_TRANSP_WIN : FormMain.GUI_24_TRANSP_LOSE);
            im1.ShiftY = im1.ShiftY + ((cellPlayer1.Height - im1.Height) / 2);
            im1.Hint = b.Winner == b.Player1 ? "Победитель" : "Проигравший";

            VCLabel textVersus = new VCLabel(this, im1.NextLeft(), cellPlayer1.ShiftY, Program.formMain.FontBigCaptionC, Color.White, cellPlayer1.Height, "vs");
            textVersus.StringFormat.Alignment = StringAlignment.Center;
            textVersus.StringFormat.LineAlignment = StringAlignment.Center;
            textVersus.Width = textVersus.Font.WidthText(textVersus.Text);

            VCImage24 im2 = new VCImage24(this, textVersus.NextLeft(), im1.ShiftY, 
                b.Winner == b.Player2 ? FormMain.GUI_24_TRANSP_WIN : FormMain.GUI_24_TRANSP_LOSE);
            im2.Hint = b.Winner == b.Player2 ? "Победитель" : "Проигравший";

            VCCell cellPlayer2 = new VCCell(this, im2.NextLeft(), cellPlayer1.ShiftY);
            cellPlayer2.Entity = b.Player2;

            btnViewResult = new VCButton(this, cellPlayer2.NextLeft(), 16, "Итоги");
            btnViewResult.Enabled = !(b is null);
            btnViewResult.Click += BtnViewResult_Click;
            btnWShowBattle = new VCButton(this, btnViewResult.NextLeft(), 16, "Просмотр");
            btnWShowBattle.Enabled = !(b is null);
            btnWShowBattle.Click += BtnWShowBattle_Click;

            //
            ApplyMaxSize();

            Height += FormMain.Config.GridSize;
            Width += FormMain.Config.GridSize;
        }

        internal Battle Battle { get; }

        private void BtnWShowBattle_Click(object sender, EventArgs e)
        {
            //WindowBattle w = new WindowBattle(Battle);
            //w.ShowBattle();
            //w.Dispose();
        }

        private void BtnViewResult_Click(object sender, EventArgs e)
        {
            WindowResultBattle w = new WindowResultBattle(Battle);
            w.Show();
        }
    }
}
