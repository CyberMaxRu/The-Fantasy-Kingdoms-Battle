using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    class WindowMenuInGame : CustomWindow
    {
        private VCBitmap bmpMenu;
        private VCButton btnBackToGame;
        private VCButton btnNewGame;
        private VCButton btnGamePreferences;
        private VCButton btnPlayerPreferences;
        private VCButton btnExitToMainMenu;
        private VCButton btnExitToWindows;

        public WindowMenuInGame()
        {
            bmpMenu = new VCBitmap(this, 0, 0, Program.formMain.bmpMenuInGame);
            Width = bmpMenu.Width;
            Height = bmpMenu.Height;

            btnBackToGame = new VCButton(bmpMenu, 80, 72, "Вернуться к игре");
            btnBackToGame.Width = Width - 80 - 80;
            btnBackToGame.Click += BtnBackToGame_Click;

            btnNewGame = new VCButton(bmpMenu, 80, btnBackToGame.NextTop(), "Новая игра");
            btnNewGame.Width = Width - 80 - 80;
            btnNewGame.Click += BtnNewGame_Click;

            btnExitToWindows = new VCButton(bmpMenu, 80, bmpMenu.Height - 96, "Выход в Windows");
            btnExitToWindows.Width = Width - 80 - 80;
            btnExitToWindows.Click += BtnExitToWindows_Click;

            btnExitToMainMenu = new VCButton(bmpMenu, 80, btnExitToWindows.ShiftY - 40, "Выход в главное меню");
            btnExitToMainMenu.Width = Width - 80 - 80;
            btnExitToMainMenu.Click += BtnExitToMainMenu_Click;

            btnGamePreferences = new VCButton(bmpMenu, 80, btnExitToMainMenu.ShiftY - 40, "Настройки");
            btnGamePreferences.Width = Width - 80 - 80;
            btnGamePreferences.Click += BtnSettings_Click;

            btnPlayerPreferences = new VCButton(bmpMenu, 80, btnGamePreferences.ShiftY - 40, "Настройки игрока");
            btnPlayerPreferences.Width = Width - 80 - 80;
            btnPlayerPreferences.Click += BtnPlayerPreferences_Click;

            CancelButton = btnBackToGame;
        }

        private void BtnExitToMainMenu_Click(object sender, EventArgs e)
        {
            if (WindowConfirm.ShowConfirm("Подтверждение", "Текущая игра будет потеряна.\n\rПродолжить?"))
            {
                CloseForm(DialogResult.Cancel);
            }
        }

        private void BtnPlayerPreferences_Click(object sender, EventArgs e)
        {
            WindowPlayerPreferences w = new WindowPlayerPreferences();
            if (w.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            if (WindowConfirm.ShowConfirm("Подтверждение", "Будет начата новая игра.\n\rТекущая игра будет потеряна.\n\rПродолжить?"))
            {
                Program.formMain.StartNewLobby();
                CloseForm(DialogResult.OK   );
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowPreferences();
        }

        private void BtnExitToWindows_Click(object sender, EventArgs e)
        {
            WindowConfirmExit f = new WindowConfirmExit();
            if (f.ShowDialog() == DialogResult.Yes)
            {
                Program.formMain.SetProgrameState(ProgramState.NeedQuit);
                CloseForm(DialogResult.Abort);
            }
        }

        private void BtnBackToGame_Click(object sender, EventArgs e)
        {
            CloseForm(DialogResult.Cancel);
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

        }
    }
}
