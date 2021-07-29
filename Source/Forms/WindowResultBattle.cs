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
        private readonly VCImageBig imgAvatarParticipant1;
        private readonly VCImageBig imgAvatarParticipant2;

        private readonly VCButton btnOk;

        public WindowResultBattle(Battle b) : base()
        {
            Debug.Assert(!(b is null));

            windowCaption.Caption = b.Player1.GetName() + " vs " + b.Player2.GetName();

            // Игроки
            imgAvatarParticipant1 = new VCImageBig(ClientControl, 0);
            imgAvatarParticipant1.ShiftX = 0;
            //imgAvatarParticipant1.Cost = b.Player1.GetName();
            imgAvatarParticipant1.ImageIndex = b.Player1.GetImageIndexAvatar();
            imgAvatarParticipant2 = new VCImageBig(ClientControl, imgAvatarParticipant1.ShiftY);
            //imgAvatarParticipant2.Cost = b.Player2.GetName();
            imgAvatarParticipant2.ImageIndex = b.Player2.GetImageIndexAvatar();

            int leftCell = imgAvatarParticipant1.NextLeft();
            int topCell = imgAvatarParticipant1.ShiftY;
            VCCell cell = null;

            for (int y = 0; y < b.СreaturesPlayer1.GetLength(0); y++)
            {
                leftCell = imgAvatarParticipant1.NextLeft();

                for (int x = 0; x < b.СreaturesPlayer1.GetLength(1); x++)
                {
                    cell = new VCCell(ClientControl, leftCell, topCell);

                    if (!(b.СreaturesPlayer1[y, x] is null))
                    {
                        cell.ShowCell(b.СreaturesPlayer1[y, x]);
                    }

                    leftCell = cell.NextLeft();
                }

                topCell = cell.NextTop();
            }

            int leftCellPrior = leftCell;
            topCell = imgAvatarParticipant1.ShiftY;
            for (int y = 0; y < b.СreaturesPlayer2.GetLength(0); y++)
            {
                leftCell = leftCellPrior;

                for (int x = 0; x < b.СreaturesPlayer2.GetLength(1); x++)
                {
                    cell = new VCCell(ClientControl, leftCell, topCell);

                    if (!(b.СreaturesPlayer2[y, b.СreaturesPlayer2.GetLength(1) - x - 1] is null))
                    {
                        cell.ShowCell(b.СreaturesPlayer1[y, b.СreaturesPlayer2.GetLength(1) - x - 1]);
                    }

                    leftCell = cell.NextLeft();
                }

                topCell = cell.NextTop();
            }

            imgAvatarParticipant2.ShiftX = leftCell;

            btnOk = new VCButton(ClientControl, 0, topCell, "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            AcceptButton = btnOk;

            //
            ClientControl.Width = imgAvatarParticipant2.NextLeft();
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.OK);
        }
    }
}
