using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowPlayerPreferences : VCForm
    {
        private string lastDirAvatar;
        private int curImageIndexAvatar;

        private readonly VCButton btnAccept;
        private readonly VCButton btnCancel;

        private readonly VCImageBig imgAvatar;
        private readonly VCIconButton btnPriorAvatar;
        private readonly VCIconButton btnNextAvatar;
        private readonly VCButton btnLoadAvatar;
        private readonly VCButton btnDeleteAvatar;

        private VCButton btnRename;

        public WindowPlayerPreferences() : base()
        {
            curImageIndexAvatar = Program.formMain.CurrentHumanPlayer.GetImageIndexAvatar();

            windowCaption.Caption = Program.formMain.CurrentHumanPlayer.Name;

            imgAvatar = new VCImageBig(ClientControl, 0);

            btnPriorAvatar = new VCIconButton(ClientControl, 0, 0, Program.formMain.ilGui24, FormMain.GUI_24_BUTTON_LEFT);
            btnPriorAvatar.ShiftY = imgAvatar.ShiftY + ((imgAvatar.Height - btnPriorAvatar.Height) / 2);
            btnPriorAvatar.Click += BtnPriorAvatar_Click;
            imgAvatar.ShiftX = btnPriorAvatar.Width;
            btnNextAvatar = new VCIconButton(ClientControl, imgAvatar.ShiftX + imgAvatar.Width, btnPriorAvatar.ShiftY, Program.formMain.ilGui24, FormMain.GUI_24_BUTTON_RIGHT);
            btnNextAvatar.Click += BtnNextAvatar_Click;                 

            btnLoadAvatar = new VCButton(ClientControl, btnNextAvatar.NextLeft(), imgAvatar.ShiftY, "Загрузить аватар");
            btnLoadAvatar.Width = 240;
            btnLoadAvatar.Click += BtnLoadAvatar_Click;
            btnDeleteAvatar = new VCButton(ClientControl, btnLoadAvatar.ShiftX, btnLoadAvatar.NextTop(), "Удалить аватар");
            btnDeleteAvatar.Width = 240;
            btnDeleteAvatar.Click += BtnDeleteAvatar_Click;

            btnRename = new VCButton(ClientControl, 0, imgAvatar.NextTop() + FormMain.Config.GridSize, "Переименовать");
            btnRename.Width = 200;
            btnRename.Click += BtnRename_Click;

            btnAccept = new VCButton(ClientControl, 0, imgAvatar.NextTop() + (FormMain.Config.GridSize * 8), "Принять");
            btnAccept.Click += BtnAccept_Click;
            btnCancel = new VCButton(ClientControl, 0, btnAccept.ShiftY, "Отмена");
            btnCancel.ShiftX = ClientControl.Width - btnCancel.Width;
            btnCancel.Click += BtnCancel_Click;

            AcceptButton = btnAccept;
            CancelButton = btnCancel;

            ClientControl.Width = btnCancel.ShiftX + btnCancel.Width + btnCancel.Left;
            ClientControl.Height = btnCancel.NextTop();

            UpdateNumberAvatar();
        }

        private void BtnNextAvatar_Click(object sender, EventArgs e)
        {
            if (curImageIndexAvatar - Program.formMain.ImageIndexFirstAvatar < Program.formMain.blPlayerAvatars.Count - 1)
                curImageIndexAvatar++;
            else
                curImageIndexAvatar = Program.formMain.ImageIndexFirstAvatar;

            UpdateNumberAvatar();
        }

        private void BtnPriorAvatar_Click(object sender, EventArgs e)
        {
            if (curImageIndexAvatar - Program.formMain.ImageIndexFirstAvatar > 0)
                curImageIndexAvatar--;
            else
                curImageIndexAvatar = Program.formMain.ImageIndexFirstAvatar + Program.formMain.blPlayerAvatars.Count - 1;

            UpdateNumberAvatar();
        }

        private void BtnDeleteAvatar_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnLoadAvatar_Click(object sender, EventArgs e)
        {
            /*OpenFileDialog OPF = new OpenFileDialog();
            OPF.InitialDirectory = lastDirAvatar?.Length > 0 ? lastDirAvatar : Environment.CurrentDirectory;
            OPF.FileName = "";
            OPF.CheckFileExists = true;
            OPF.Multiselect = false;
            OPF.Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg";
            if (OPF.ShowDialog() == DialogResult.OK)
            {
                filenameAvatar = OPF.FileName;
                lastDirAvatar = Path.GetDirectoryName(filenameAvatar);
                ShowAvatar();
            }*/
        }

        private void BtnRename_Click(object sender, EventArgs e)
        {
            
        }

        private void UpdateNumberAvatar()
        {
            imgAvatar.ImageIndex = curImageIndexAvatar;
            imgAvatar.Cost = $"{curImageIndexAvatar - Program.formMain.ImageIndexFirstAvatar + 1}/{Program.formMain.blPlayerAvatars.Count}";
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm(DialogResult.Cancel);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            CloseForm(DialogResult.OK);
        }
    }
}
