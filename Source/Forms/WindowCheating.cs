using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Окно настройки читинга
    internal sealed class WindowCheating : VCForm
    {
        private Settings settings;

        private VCButton btnShowAll;

        private VCButton btnAccept;
        private VCButton btnSetAll;
        private VCButton btnResetAll;
        private VCButton btnCancel;

        private VCCheckBox chkbIgnoreRequirements;
        private VCCheckBox chkbIgnoreResources;
        private VCCheckBox chkbIgnoreBuilders;

        public WindowCheating(Settings s) : base()
        {
            settings = s;

            windowCaption.Caption = "Настройка читинга";

            chkbIgnoreRequirements = new VCCheckBox(ClientControl, 0, 0, "Игнорировать требования к наличию сооружений и прочего");
            chkbIgnoreRequirements.Hint = "Игнорировать требования к наличию сооружений, исследований (кроме золота, строителей и тому подобного)";
            chkbIgnoreRequirements.Checked = settings.CheatingIgnoreRequirements;

            chkbIgnoreResources = new VCCheckBox(ClientControl, 0, chkbIgnoreRequirements.NextTop(), "Игнорировать требования базовых ресурсов");
            chkbIgnoreResources.Hint = "Игнорировать требования к наличию достаточного количества базовых ресурсов и не тратить их";
            chkbIgnoreResources.Checked = settings.CheatingIgnoreBaseResources;

            chkbIgnoreBuilders = new VCCheckBox(ClientControl, 0, chkbIgnoreResources.NextTop(), "Игнорировать требования строителей");
            chkbIgnoreBuilders.Hint = "Игнорировать требования к наличию достаточного количества строителей";
            chkbIgnoreBuilders.Checked = settings.CheatingIgnoreBuilders;

            btnShowAll = new VCButton(ClientControl, 0, chkbIgnoreBuilders.NextTop() + (FormMain.Config.GridSize * 2), "Открыть все локации");
            btnShowAll.Width = 240;
            btnShowAll.Click += BtnShowAll_Click;

            // Кнопки
            btnAccept = new VCButton(ClientControl, 0, btnShowAll.NextTop() + (FormMain.Config.GridSize * 2), "Принять");
            btnAccept.Width = 160;
            btnAccept.Click += BtnAccept_Click;

            btnSetAll = new VCButton(ClientControl, btnAccept.NextLeft(), btnAccept.ShiftY, "Выбрать все");
            btnSetAll.Width = 160;
            btnSetAll.Click += BtnSetAll_Click;

            btnResetAll = new VCButton(ClientControl, btnSetAll.NextLeft(), btnAccept.ShiftY, "Убрать все");
            btnResetAll.Width = 160;
            btnResetAll.Click += BtnResetAll_Click;

            btnCancel = new VCButton(ClientControl, btnResetAll.NextLeft(), btnAccept.ShiftY, "Отмена");
            btnCancel.Width = 160;
            btnCancel.Click += BtnCancel_Click;

            AcceptButton = btnAccept;
            CancelButton = btnCancel;

            ClientControl.Width = btnCancel.ShiftX + btnCancel.Width + btnCancel.Left;
            ClientControl.Height = btnCancel.NextTop();

            chkbIgnoreRequirements.Width = ClientControl.Width;
            chkbIgnoreResources.Width = ClientControl.Width;
            chkbIgnoreBuilders.Width = ClientControl.Width;

            ApplyMaxSize();
        }

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            Program.formMain.CurrentLobby.CurrentPlayer.UnhideAll();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }

        private void SetAll(bool check)
        {
            chkbIgnoreRequirements.Checked = check;
            chkbIgnoreResources.Checked = check;
            chkbIgnoreBuilders.Checked = check;
        }

        private void BtnResetAll_Click(object sender, EventArgs e)
        {
            SetAll(false);
        }

        private void BtnSetAll_Click(object sender, EventArgs e)
        {
            SetAll(true);
        }

        private void BtnAccept_Click(object sender, EventArgs e)
        {
            settings.CheatingIgnoreRequirements = chkbIgnoreRequirements.Checked;
            settings.CheatingIgnoreBaseResources = chkbIgnoreResources.Checked;
            settings.CheatingIgnoreBuilders = chkbIgnoreBuilders.Checked;

            CloseForm(DialogAction.OK);
        }
    }
}
