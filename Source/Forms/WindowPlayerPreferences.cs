using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowPlayerPreferences : VCForm
    {
        private int curImageIndexAvatar;

        private readonly VCButton btnAccept;
        private readonly VCButton btnCancel;

        private readonly VCEdit editName;
        private readonly VCSeparator sprTop;
        private readonly VCLabelM2 lblTextForAvatar;
        private readonly VCImageBig imgAvatar;
        private readonly VCIconButton btnPriorAvatar;
        private readonly VCIconButton btnNextAvatar;
        private readonly VCButton btnAddAvatar;
        private readonly VCButton btnChangeAvatar;
        private readonly VCButton btnDeleteAvatar;
        private readonly VCSeparator sprBottom;

        public WindowPlayerPreferences() : base()
        {
            curImageIndexAvatar = Program.formMain.CurrentHumanPlayer.GetImageIndexAvatar();

            windowCaption.Caption = Program.formMain.CurrentHumanPlayer.Name;

            editName = new VCEdit(ClientControl, 0, 0, "", FormMain.MAX_LENGTH_USERNAME);
            editName.Width = 240;
            editName.Text = Program.formMain.CurrentHumanPlayer.Name;

            sprTop = new VCSeparator(ClientControl, 0, editName.NextTop());
            lblTextForAvatar = new VCLabelM2(ClientControl, 0, sprTop.NextTop() - FormMain.Config.GridSize, Program.formMain.fontParagraph, Color.White, 20, "Аватар:");
            lblTextForAvatar.StringFormat.Alignment = StringAlignment.Near;
            lblTextForAvatar.StringFormat.LineAlignment = StringAlignment.Near;
            imgAvatar = new VCImageBig(ClientControl, lblTextForAvatar.NextTop());

            btnPriorAvatar = new VCIconButton(ClientControl, 0, 0, Program.formMain.ilGui24, FormMain.GUI_24_BUTTON_LEFT);
            btnPriorAvatar.ShiftY = imgAvatar.ShiftY + ((imgAvatar.Height - btnPriorAvatar.Height) / 2);
            btnPriorAvatar.Click += BtnPriorAvatar_Click;
            imgAvatar.ShiftX = btnPriorAvatar.Width;
            lblTextForAvatar.ShiftX = imgAvatar.ShiftX;
            btnNextAvatar = new VCIconButton(ClientControl, imgAvatar.ShiftX + imgAvatar.Width, btnPriorAvatar.ShiftY, Program.formMain.ilGui24, FormMain.GUI_24_BUTTON_RIGHT);
            btnNextAvatar.Click += BtnNextAvatar_Click;

            btnAddAvatar = new VCButton(ClientControl, btnNextAvatar.NextLeft(), imgAvatar.ShiftY, "Добавить аватар");
            btnAddAvatar.Width = 240;
            btnAddAvatar.Click += BtnAddAvatar_Click;
            btnChangeAvatar = new VCButton(ClientControl, btnAddAvatar.ShiftX, btnAddAvatar.NextTop(), "Изменить аватар");
            btnChangeAvatar.Width = 240;
            btnChangeAvatar.Click += BtnChangeAvatar_Click;
            btnDeleteAvatar = new VCButton(ClientControl, btnAddAvatar.ShiftX, btnChangeAvatar.NextTop(), "Удалить аватар");
            btnDeleteAvatar.Width = 240;
            btnDeleteAvatar.Click += BtnDeleteAvatar_Click;

            sprBottom = new VCSeparator(ClientControl, 0, imgAvatar.NextTop());

            btnAccept = new VCButton(ClientControl, 0, sprBottom.NextTop(), "Принять");
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

        internal override void AdjustSize()
        {
            lblTextForAvatar.Width = ClientControl.Width - lblTextForAvatar.ShiftX;

            base.AdjustSize();

            sprTop.Width = ClientControl.Width;
            sprBottom.Width = ClientControl.Width;
        }

        private void BtnChangeAvatar_Click(object sender, EventArgs e)
        {
            string filename = SelectFileAvatar();
            if (filename.Length > 0)
            {
                if (Program.formMain.ChangeAvatar(curImageIndexAvatar, filename))
                    UpdateNumberAvatar();
            }
        }

        private void BtnNextAvatar_Click(object sender, EventArgs e)
        {
            if (curImageIndexAvatar - Program.formMain.ImageIndexFirstAvatar < Program.formMain.AvatarsCount - 1)
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
                curImageIndexAvatar = Program.formMain.ImageIndexFirstAvatar + Program.formMain.AvatarsCount - 1;

            UpdateNumberAvatar();
        }

        private void BtnDeleteAvatar_Click(object sender, EventArgs e)
        {
            if (WindowConfirm.ShowConfirm("Подтверждение", "Удалить аватар?"))
            {
                Program.formMain.DeleteAvatar(curImageIndexAvatar);

                if (curImageIndexAvatar >= Program.formMain.ImageIndexFirstAvatar + Program.formMain.AvatarsCount)
                    curImageIndexAvatar--;

                UpdateNumberAvatar();
            }
        }

        private void BtnAddAvatar_Click(object sender, EventArgs e)
        {
            if (FormMain.Config.ExternalAvatars.Count >= FormMain.MAX_AVATARS)
            {
                WindowInfo.ShowInfo("Информация", $"Достигнуто максимальное количество внешних аватаров: {FormMain.MAX_AVATARS}.");
                return;
            }

            string filename = SelectFileAvatar();
            if (filename.Length > 0)
            {
                Program.formMain.AddAvatar(filename);
                curImageIndexAvatar = Program.formMain.ImageIndexFirstAvatar + Program.formMain.blInternalAvatars.Count
                    + Program.formMain.blExternalAvatars.Count - 1;
                UpdateNumberAvatar();
            }
        }

        private string SelectFileAvatar()
        {
            OpenFileDialog OPF = new OpenFileDialog();
            try
            {
                OPF.InitialDirectory = Program.formMain.CurrentHumanPlayer.DirectoryAvatar?.Length > 0 ? Program.formMain.CurrentHumanPlayer.DirectoryAvatar : Environment.CurrentDirectory;
                OPF.FileName = "";
                OPF.CheckFileExists = true;
                OPF.Multiselect = false;
                OPF.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

                if (OPF.ShowDialog() == DialogResult.OK)
                {
                    Program.formMain.CurrentHumanPlayer.DirectoryAvatar = Path.GetDirectoryName(OPF.FileName);
                    return OPF.FileName;
                }
                else
                    return "";
            }
            finally
            {
                OPF.Dispose();
            }
        }

        private void UpdateNumberAvatar()
        {
            imgAvatar.ImageIndex = curImageIndexAvatar;
            imgAvatar.Cost = $"{curImageIndexAvatar - Program.formMain.ImageIndexFirstAvatar + 1}/{Program.formMain.AvatarsCount}";

            btnChangeAvatar.Enabled = curImageIndexAvatar >= Program.formMain.ImageIndexExternalAvatar;
            btnDeleteAvatar.Enabled = btnChangeAvatar.Enabled;

            Program.formMain.CurrentHumanPlayer.ChangeImageIndex(curImageIndexAvatar - Program.formMain.ImageIndexFirstAvatar);
            FormMain.Config.SaveHumanPlayers();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if ((Program.formMain.CurrentLobby != null) && !Program.formMain.CurrentLobby.CheckUniqueAvatars())
            {
                WindowInfo.ShowInfo("Информация", "Выбранный аватар уже используется другим игроком.\n\rВыберите другой аватар.");
                return;
            }

            CloseForm(DialogResult.Cancel);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            if ((Program.formMain.CurrentLobby != null) && !Program.formMain.CurrentLobby.CheckUniqueAvatars())
            {
                WindowInfo.ShowInfo("Информация", "Выбранный аватар уже используется другим игроком.\n\rВыберите другой аватар.");
                return;
            }

            if (editName.Text.Length == 0)
            {
                WindowInfo.ShowInfo("Информация", "Введите имя игрока.");
                return;
            }

            if (Program.formMain.CurrentHumanPlayer.Name != editName.Text)
            {
                Program.formMain.CurrentHumanPlayer.SetName(editName.Text);
                FormMain.Config.SaveHumanPlayers();
                Program.formMain.ShowCurrentPlayerLobby();
            }

            CloseForm(DialogResult.OK);
        }
    }
}
