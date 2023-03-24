using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowMessage : VCForm
    {
        private readonly VCImage128 imgAvatar;
        private readonly VCText txtMain;
        private readonly VCButton btnPrior;
        private readonly VCButton btnNext;

        private Player player;
        private DescriptorMissionMessage message;
        private int currPart = -1;

        public WindowMessage() : base()
        {
            ClientControl.Width = 560;
            ClientControl.Height = 280;

            windowCaption.Color = Color.Orange;
            windowCaption.Width = ClientControl.Width;// - (FormMain.Config.GridSize * 2);

            ShowButtonClose = true;

            imgAvatar = new VCImage128(ClientControl, 0, 0);
            txtMain = new VCText(ClientControl, imgAvatar.NextLeft(), 0, Program.formMain.FontMedCaption, Color.White, 120);
            txtMain.StringFormat.Alignment = StringAlignment.Near;

            btnPrior = new VCButton(ClientControl, 0, 0, "Назад");
            btnPrior.Click += BtnPrior_Click;
            btnNext = new VCButton(ClientControl, 0, 0, "");
            btnNext.Click += BtnNext_Click;

            btnPrior.ShiftX = (ClientControl.Width - btnPrior.Width - btnNext.Width - FormMain.Config.GridSize) / 2;
            btnNext.ShiftX = btnPrior.NextLeft();
            btnPrior.ShiftY = ClientControl.Height - btnPrior.Height;
            btnNext.ShiftY = btnPrior.ShiftY;

            txtMain.Width = ClientControl.Width - txtMain.ShiftX;
            txtMain.Height = btnNext.ShiftY - FormMain.Config.GridSize;

            AcceptButton = btnNext;
        }

        private void BtnPrior_Click(object sender, EventArgs e)
        {
            ApplyPart(--currPart);
        }

        internal void SetMessage(Player p, DescriptorMissionMessage m)
        {
            player = p;
            message = m;
            ApplyPart(0);

            if (m.Parts.Count == 1)
            {
                btnPrior.Visible = false;
                btnNext.ShiftX = (ClientControl.Width - btnNext.Width) / 2;
                ClientControl.ArrangeControl(btnNext);
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currPart == message.Parts.Count - 1)
                CloseForm(DialogAction.None);
            else
                ApplyPart(++currPart);
        }

        private void ApplyPart(int part)
        {
            currPart = part;

            btnPrior.Enabled = part > 0;
            btnNext.Caption = part < message.Parts.Count - 1 ? "Далее" : "Закрыть";

            BigEntity be = player.FindBigEntityInSelfAndLobby(message.Parts[currPart].From);
            windowCaption.Caption = be.GetName();
            imgAvatar.ImageIndex = be.GetImageIndex();
            txtMain.Text = message.Parts[currPart].Text;
        }
    }
}
