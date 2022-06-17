using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowMessage : VCForm
    {
        private readonly VCImage128 imgAvatar;
        private readonly VCText txtMain;
        private readonly VCButton btnNext;

        private DescriptorMissionMessage m;
        private int currPart;

        public WindowMessage()
        {
            windowCaption.Color = Color.Orange;
            ShowButtonClose = true;

            imgAvatar = new VCImage128(ClientControl, 0, 0);
            txtMain = new VCText(ClientControl, imgAvatar.NextLeft(), 0, Program.formMain.fontMedCaption, Color.White, 120);
            txtMain.StringFormat.Alignment = StringAlignment.Near;

            ClientControl.Width = 560;
            ClientControl.Height = 280;

            btnNext = new VCButton(ClientControl, 0, 0, "Дальше");
            btnNext.Click += BtnNext_Click;

            AcceptButton = btnNext;
        }

        internal void SetMessage(DescriptorMissionMessage message)
        {
            m = message;
            ApplyPart(0);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currPart == m.Parts.Count - 1)
                CloseForm(DialogAction.None);
            else
                ApplyPart(++currPart);
        }

        internal override void AdjustSize()
        {
            windowCaption.Width = ClientControl.Width;// - (FormMain.Config.GridSize * 2);

            base.AdjustSize();

            txtMain.Width = ClientControl.Width - txtMain.ShiftX;
            btnNext.ShiftX = (ClientControl.Width - btnNext.Width) / 2;
            btnNext.ShiftY = ClientControl.Height - btnNext.Height;
        }

        private void ApplyPart(int part)
        {
            currPart = part;

            windowCaption.Caption = m.Parts[currPart].From.Name;
            imgAvatar.ImageIndex = m.Parts[currPart].From.ImageIndex;
            txtMain.Text = m.Parts[currPart].Text;
            txtMain.Height = txtMain.MinHeigth();

            Program.formMain.SetNeedRedrawFrame();
        }
    }
}
