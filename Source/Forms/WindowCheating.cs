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
        private VCButton btnGiveHeroesGold;

        private VCButton btnAccept;
        private VCButton btnSetAll;
        private VCButton btnResetAll;
        private VCButton btnCancel;

        private VCCheckBox chkbIgnoreRequirements;
        private VCCheckBox chkbSpeedUpProgressBy10;
        private VCCheckBox chkbReduceCostBy10;
        private VCCheckBox chkbPointsTraditionMore10Times;

        public WindowCheating(Player p) : base()
        {
            Debug.Assert(p != null);
            player = p;

            windowCaption.Caption = "Настройка читинга";

            chkbIgnoreRequirements = new VCCheckBox(ClientControl, 0, 0, "Игнорировать требования к наличию сооружений и прочего");
            chkbIgnoreRequirements.Hint = "Игнорировать требования к наличию сооружений, исследований (кроме ресурсов и строителей)";
            chkbIgnoreRequirements.Checked = player.CheatingIgnoreRequirements;

            chkbSpeedUpProgressBy10 = new VCCheckBox(ClientControl, 0, chkbIgnoreRequirements.NextTop(), "Ускорить прогресс в 10 раз");
            chkbSpeedUpProgressBy10.Hint = "Прогресс действий (строительство, исследования, найм и т.д.) ускоряется в 10 раз";
            chkbSpeedUpProgressBy10.Checked = player.CheatingSpeedUpProgressBy10;

            chkbReduceCostBy10 = new VCCheckBox(ClientControl, 0, chkbSpeedUpProgressBy10.NextTop(), "Стоимость действий уменьшается в 10 раз");
            chkbReduceCostBy10.Hint = "Стоимость (золото, ресурсы) действий (строительство, исследования, найм и т.д.) уменьшается в 10 раз";
            chkbReduceCostBy10.Checked = player.CheatingReduceCostBy10;

            chkbPointsTraditionMore10Times = new VCCheckBox(ClientControl, 0, chkbReduceCostBy10.NextTop(), "Прирост очков традиции увеличивается в 10 раз");
            chkbPointsTraditionMore10Times.Hint = "Количество очков для принятия традиции увеличивается в 10 раз";
            chkbPointsTraditionMore10Times.Checked = player.CheatingPointsTraditionMore10Times;

            btnShowAll = new VCButton(ClientControl, 0, chkbPointsTraditionMore10Times.NextTop() + (FormMain.Config.GridSize * 2), "Открыть все локации");
            btnShowAll.Width = 240;
            btnShowAll.Click += BtnShowAll_Click;

            btnGiveHeroesGold = new VCButton(ClientControl, btnShowAll.NextLeft(), btnShowAll.ShiftY, "Дать героям 1000 золота");
            btnGiveHeroesGold.Width = 256;
            btnGiveHeroesGold.Click += BtnGiveHeroesGold_Click;

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
            chkbSpeedUpProgressBy10.Width = ClientControl.Width;
            chkbReduceCostBy10.Width = ClientControl.Width;
            chkbPointsTraditionMore10Times.Width = ClientControl.Width;

            ApplyMaxSize();
        }

        private void BtnGiveHeroesGold_Click(object sender, EventArgs e)
        {
            foreach (Creature c in player.CombatHeroes)
            {
                if (c.IsLive)
                {
                    c.AddGold(1000);
                }
            }
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
            chkbSpeedUpProgressBy10.Checked = check;
            chkbReduceCostBy10.Checked = check;
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
            player.CheatingSpeedUpProgressBy10 = chkbSpeedUpProgressBy10.Checked;
            player.CheatingReduceCostBy10 = chkbReduceCostBy10.Checked;
            player.CheatingPointsTraditionMore10Times = chkbPointsTraditionMore10Times.Checked;

            Program.formMain.CurrentHumanPlayer.CheatingIgnoreRequirements = chkbIgnoreRequirements.Checked;
            Program.formMain.CurrentHumanPlayer.CheatingSpeedUpProgressBy10 = chkbSpeedUpProgressBy10.Checked;
            Program.formMain.CurrentHumanPlayer.CheatingReduceCostBy10 = chkbReduceCostBy10.Checked;
            Program.formMain.CurrentHumanPlayer.CheatingPointsTraditionMore10Times = chkbPointsTraditionMore10Times.Checked;
            FormMain.Descriptors.SaveHumanPlayers();

            CloseForm(DialogAction.OK);
        }
    }
}
