using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Окно результатов битвы
    internal sealed class WindowResultBattle : VCForm
    {
        private readonly VCImage128 imgAvatarParticipant1;
        private readonly VCImage128 imgAvatarParticipant2;
        private readonly VCCell[,] cells;

        private readonly VCButton btnOk;

        public WindowResultBattle(Battle b) : base()
        {
            Debug.Assert(!(b is null));

            windowCaption.Caption = b.Player1.GetName() + " vs " + b.Player2.GetName();

            // Игроки
            imgAvatarParticipant1 = new VCImage128(ClientControl, 0, 0);
            imgAvatarParticipant1.ShiftX = 0;
            //imgAvatarParticipant1.Cost = b.Player1.GetName();
            imgAvatarParticipant1.ImageIndex = b.Player1.GetImageIndexAvatar();
            imgAvatarParticipant2 = new VCImage128(ClientControl, 0, imgAvatarParticipant1.ShiftY);
            //imgAvatarParticipant2.Cost = b.Player2.GetName();
            imgAvatarParticipant2.ImageIndex = b.Player2.GetImageIndexAvatar();

            // Создаем массив ячеек
            cells = new VCCell[FormMain.Config.HeroInRow, FormMain.Config.HeroRows * 2];
            int leftCell = imgAvatarParticipant1.NextLeft();
            int topCell = imgAvatarParticipant1.ShiftY;
            VCCell cell = null;

            for (int y = 0; y < FormMain.Config.HeroInRow; y++)
            {
                leftCell = imgAvatarParticipant1.NextLeft();

                for (int x = 0; x < FormMain.Config.HeroRows * 2; x++)
                {
                    cell = new VCCell(ClientControl, leftCell, topCell);
                    cells[y, x] = cell;

                    leftCell = cell.NextLeft();
                    if (x == FormMain.Config.HeroRows - 1)
                        leftCell += FormMain.Config.GridSize * 2;
                }

                topCell = cell.NextTop();
            }

            imgAvatarParticipant2.ShiftX = leftCell;

            // Указываем существ у ячеек
            foreach (HeroInBattle hb in b.heroesPlayer1)
            {
                Debug.Assert(cells[hb.StartCoord.Y, hb.StartCoord.X].Cell is null);

                cells[hb.StartCoord.Y, hb.StartCoord.X].ShowCell(hb);
            }

            foreach (HeroInBattle hb in b.heroesPlayer2)
            {
                Debug.Assert(cells[hb.StartCoord.Y, FormMain.Config.HeroRows * 2 - hb.StartCoord.X - 1].Cell is null);

                cells[hb.StartCoord.Y, FormMain.Config.HeroRows * 2 - hb.StartCoord.X - 1].ShowCell(hb);
            }

            //
            btnOk = new VCButton(ClientControl, 0, topCell, "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            AcceptButton = btnOk;
            CancelButton = btnOk;

            //
            ClientControl.Width = imgAvatarParticipant2.NextLeft() - FormMain.Config.GridSize;
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.OK);
        }
    }
}
