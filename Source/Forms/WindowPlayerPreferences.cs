using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowPlayerPreferences : VCForm
    {
        private int curImageIndexAvatar;

        private readonly VCButton btnAccept;
        private readonly VCButton btnCancel;

        private readonly VCEdit editName;
        private readonly VCSeparator sprTop;
        private readonly VCLabel lblTextForAvatar;
        private readonly VCImage128 imgAvatar;
        private readonly VCIconButton24 btnPriorAvatar;
        private readonly VCIconButton24 btnNextAvatar;
        private readonly VCButton btnAddAvatar;
        private readonly VCButton btnChangeAvatar;
        private readonly VCButton btnDeleteAvatar;
        private readonly VCSeparator sprBottom;

        public WindowPlayerPreferences() : base()
        {
            curImageIndexAvatar = Program.formMain.CurrentHumanPlayer.ImageIndex;

            windowCaption.Caption = "Настройки игрока";

            editName = new VCEdit(ClientControl, 0, 0, "", FormMain.Config.MaxLengthObjectName);
            editName.Width = 240;
            editName.Text = Program.formMain.CurrentHumanPlayer.Name;
            editName.CursorToEnd();

            sprTop = new VCSeparator(ClientControl, 0, editName.NextTop());
            lblTextForAvatar = new VCLabel(ClientControl, 0, sprTop.NextTop() - FormMain.Config.GridSize, Program.formMain.fontParagraph, Color.White, 20, "Аватар:");
            lblTextForAvatar.StringFormat.Alignment = StringAlignment.Near;
            lblTextForAvatar.StringFormat.LineAlignment = StringAlignment.Near;
            imgAvatar = new VCImage128(ClientControl, FormMain.Config.GridSize, lblTextForAvatar.NextTop());
            imgAvatar.HighlightUnderMouse = false;

            btnPriorAvatar = VCIconButton24.CreateButton(ClientControl, 0, 0, FormMain.GUI_24_BUTTON_LEFT, BtnPriorAvatar_Click);
            btnPriorAvatar.ShiftY = imgAvatar.ShiftY + ((imgAvatar.Height - btnPriorAvatar.Height) / 2);
            imgAvatar.ShiftX = btnPriorAvatar.Width;
            lblTextForAvatar.ShiftX = imgAvatar.ShiftX;
            btnNextAvatar = VCIconButton24.CreateButton(ClientControl, imgAvatar.ShiftX + imgAvatar.Width, btnPriorAvatar.ShiftY, FormMain.GUI_24_BUTTON_RIGHT, BtnNextAvatar_Click);

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

        private void BtnPriorAvatar_Click(object sender, EventArgs e)
        {
            if (curImageIndexAvatar > FormMain.Config.ImageIndexFirstAvatar)
                curImageIndexAvatar--;
            else
                curImageIndexAvatar = FormMain.Config.ImageIndexLastAvatar;

            UpdateNumberAvatar();
        }

        private void BtnNextAvatar_Click(object sender, EventArgs e)
        {
            if (curImageIndexAvatar < FormMain.Config.ImageIndexLastAvatar)
                curImageIndexAvatar++;
            else
                curImageIndexAvatar = FormMain.Config.ImageIndexFirstAvatar;

            UpdateNumberAvatar();
        }

        private void BtnDeleteAvatar_Click(object sender, EventArgs e)
        {
            if (WindowConfirm.ShowConfirm("Подтверждение", "Удалить аватар?"))
            {
                Program.formMain.DeleteAvatar(curImageIndexAvatar);

                if (curImageIndexAvatar > FormMain.Config.ImageIndexLastAvatar)
                    curImageIndexAvatar--;

                UpdateNumberAvatar();
            }
        }

        private void BtnAddAvatar_Click(object sender, EventArgs e)
        {
            if (FormMain.Config.ExternalAvatars.Count >= FormMain.Config.MaxQuantityExternalAvatars)
            {
                WindowInfo.ShowInfo("Информация", $"Достигнуто максимальное количество внешних аватаров: {FormMain.Config.MaxQuantityExternalAvatars}.");
                return;
            }

            string filename = SelectFileAvatar();
            if (filename.Length > 0)
            {
                Program.formMain.AddAvatar(filename);
                curImageIndexAvatar = FormMain.Config.ImageIndexLastAvatar;
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
            imgAvatar.Cost = $"{curImageIndexAvatar - FormMain.Config.ImageIndexFirstAvatar + 1}/{FormMain.Config.QuantityAllAvatars}";

            btnChangeAvatar.Enabled = curImageIndexAvatar >= FormMain.Config.ImageIndexExternalAvatar;
            btnDeleteAvatar.Enabled = btnChangeAvatar.Enabled;

            Program.formMain.CurrentHumanPlayer.SetImageIndex(curImageIndexAvatar);
            FormMain.Config.SaveHumanPlayers();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if ((Program.formMain.CurrentLobby != null) && !Program.formMain.CurrentLobby.CheckUniqueAvatars())
            {
                WindowInfo.ShowInfo("Информация", "Выбранный аватар уже используется другим игроком.\n\rВыберите другой аватар.");
                return;
            }

            CloseForm(DialogAction.None);
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
                if (!FormMain.Config.CheckNonExistsNamePlayer(editName.Text))
                    {
                        WindowInfo.ShowInfo("Информация", "Выбранное имя уже используется другим игроком.\n\rВыберите другое имя.");
                        return;
                    }

                if (Program.formMain.CurrentLobby != null)
                {
                    Debug.Assert(Program.formMain.CurrentLobby.CheckUniqueNamePlayers());
                }

                Program.formMain.CurrentHumanPlayer.SetName(editName.Text);
                FormMain.Config.SaveHumanPlayers();
                if (Program.formMain.CurrentLobby != null)
                    Program.formMain.ShowCurrentPlayerLobby();
            }

            CloseForm(DialogAction.OK);
        }
    }
}
