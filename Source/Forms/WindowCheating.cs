using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Окно настройки читинга
    internal sealed class WindowCheating : VCForm
    {
        private Player player;

        private VCButton btnShowAll;

        private VCButton btnAccept;
        private VCButton btnSetAll;
        private VCButton btnResetAll;
        private VCButton btnCancel;

        private VCCheckBox chkbIgnoreRequirements;
        private VCCheckBox chkbIgnoreResources;
        private VCCheckBox chkbRequirementBuildersAlwaysZero;
        private VCCheckBox chkbInstantlyBuilding;
        private VCCheckBox chkbInstantlyResearch;
        private VCCheckBox chkbInstantlyHire;

        public WindowCheating(Player p) : base()
        {
            Debug.Assert(p != null);
            player = p;

            windowCaption.Caption = "Настройка читинга";

            chkbIgnoreRequirements = new VCCheckBox(ClientControl, 0, 0, "Игнорировать требования к наличию сооружений и прочего");
            chkbIgnoreRequirements.Hint = "Игнорировать требования к наличию сооружений, исследований (кроме золота, строителей и тому подобного)";
            chkbIgnoreRequirements.Checked = player.CheatingIgnoreRequirements;

            chkbIgnoreResources = new VCCheckBox(ClientControl, 0, chkbIgnoreRequirements.NextTop(), "Игнорировать требования базовых ресурсов");
            chkbIgnoreResources.Hint = "Игнорировать требования к наличию достаточного количества базовых ресурсов и не тратить их";
            chkbIgnoreResources.Checked = player.CheatingIgnoreBaseResources;

            chkbRequirementBuildersAlwaysZero = new VCCheckBox(ClientControl, 0, chkbIgnoreResources.NextTop(), "Игнорировать требования строителей");
            chkbRequirementBuildersAlwaysZero.Hint = "Строительство не требует строителей";
            chkbRequirementBuildersAlwaysZero.Checked = player.CheatingIgnoreBuilders;

            chkbInstantlyBuilding = new VCCheckBox(ClientControl, 0, chkbRequirementBuildersAlwaysZero.NextTop(), "Мгновенная постройка сооружений");
            chkbInstantlyBuilding.Hint = "Сооружения строятся сразу же, минуя очередь и процесс постройки";
            chkbInstantlyBuilding.Checked = player.CheatingInstantlyBuilding;

            chkbInstantlyResearch = new VCCheckBox(ClientControl, 0, chkbInstantlyBuilding.NextTop(), "Мгновенное исследование в сооружении");
            chkbInstantlyResearch.Hint = "Исследования, постройка и прочее в сооружении происходят сразу же";
            chkbInstantlyResearch.Checked = player.CheatingInstantlyResearch;

            chkbInstantlyHire = new VCCheckBox(ClientControl, 0, chkbInstantlyResearch.NextTop(), "Мгновенный найм");
            chkbInstantlyHire.Hint = "Найм героев и иных существ происходит сразу же";
            chkbInstantlyHire.Checked = player.CheatingInstantlyHire;

            btnShowAll = new VCButton(ClientControl, 0, chkbInstantlyHire.NextTop() + (FormMain.Config.GridSize * 2), "Открыть все локации");
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
            chkbRequirementBuildersAlwaysZero.Width = ClientControl.Width;
            chkbInstantlyBuilding.Width = ClientControl.Width;
            chkbInstantlyResearch.Width = ClientControl.Width;
            chkbInstantlyHire.Width = ClientControl.Width;

            ApplyMaxSize();
        }

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            player.UnhideAll();
            Program.formMain.layerGame.UpdateNeighborhoods();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }

        private void SetAll(bool check)
        {
            chkbIgnoreRequirements.Checked = check;
            chkbIgnoreResources.Checked = check;
            chkbRequirementBuildersAlwaysZero.Checked = check;
            chkbInstantlyBuilding.Checked = check;
            chkbInstantlyResearch.Checked = check;
            chkbInstantlyHire.Checked = check;
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
            player.CheatingIgnoreRequirements = chkbIgnoreRequirements.Checked;
            player.CheatingIgnoreBaseResources = chkbIgnoreResources.Checked;
            player.CheatingIgnoreBuilders = chkbRequirementBuildersAlwaysZero.Checked;
            player.CheatingInstantlyBuilding = chkbInstantlyBuilding.Checked;
            player.CheatingInstantlyResearch = chkbInstantlyResearch.Checked;
            player.CheatingInstantlyHire = chkbInstantlyHire.Checked;

            CloseForm(DialogAction.OK);
        }
    }
}
