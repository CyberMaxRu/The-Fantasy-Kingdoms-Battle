using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    sealed class WindowMenuInGame : CustomWindow
    {
        private readonly LayerGameSingle layerGame;
        private Lobby lobby;
        private VCBitmap bmpMenu;
        private readonly VCButtonForMenu btnBackToGame;
        private readonly VCButtonForMenu btnNewGame;
        private readonly VCButtonForMenu btnGamePreferences;
        private readonly VCButtonForMenu btnPlayerPreferences;
        private readonly VCButtonForMenu btnExitToMainMenu;
        private readonly VCButtonForMenu btnExitToWindows;

        public WindowMenuInGame(LayerGameSingle layerGame, Lobby lobby) : base(true)
        {
            this.layerGame = layerGame;
            this.lobby = lobby;
            bmpMenu = new VCBitmap(this, 0, 0, Program.formMain.bmpMenuInGame);
            Width = bmpMenu.Width;
            Height = bmpMenu.Height;

            btnBackToGame = new VCButtonForMenu(bmpMenu, 72, "Вернуться к игре", BtnBackToGame_Click);
            btnNewGame = new VCButtonForMenu(bmpMenu, btnBackToGame.NextTop(), "Новая игра", BtnNewGame_Click);
            btnExitToWindows = new VCButtonForMenu(bmpMenu, bmpMenu.Height - 96, "Выход в Windows", BtnExitToWindows_Click);
            btnExitToMainMenu = new VCButtonForMenu(bmpMenu, btnExitToWindows.ShiftY - 40, "Выход в главное меню", BtnExitToMainMenu_Click);
            btnGamePreferences = new VCButtonForMenu(bmpMenu, btnExitToMainMenu.ShiftY - 40, "Настройки", BtnSettings_Click);
            btnPlayerPreferences = new VCButtonForMenu(bmpMenu, btnGamePreferences.ShiftY - 40, "Настройки игрока", BtnPlayerPreferences_Click);

            CancelButton = btnBackToGame;
        }

        private void BtnExitToMainMenu_Click(object sender, EventArgs e)
        {
            WindowConfirm.ShowConfirm("Подтверждение", "Текущая игра будет потеряна.\n\rПродолжить?", ExitToMainMenu);
        }

        private void ExitToMainMenu(object sender, EventArgs e)
        {
            CloseForm(DialogAction.MainMenu);
        }

        private void BtnPlayerPreferences_Click(object sender, EventArgs e)
        {
            WindowPlayerPreferences w = new WindowPlayerPreferences(lobby);
            w.Show();
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            WindowConfirm.ShowConfirm("Подтверждение", "Будет начата новая игра.\n\rТекущая игра будет потеряна.\n\rПродолжить?", NewGame);
        }

        private void NewGame(object sender, EventArgs e)
        {
            CloseForm(DialogAction.RestartGame);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowPreferences();
        }

        private void BtnExitToWindows_Click(object sender, EventArgs e)
        {
            WindowConfirmExit f = new WindowConfirmExit();
            f.Show();
        }

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            switch (da)
            {
                case DialogAction.None:
                    break;
                case DialogAction.Quit:
                    Program.formMain.SetProgrameState(ProgramState.NeedQuit);
                    break;
                case DialogAction.MainMenu:
                    layerGame.EndLobby();
                    break;
                case DialogAction.RestartGame:
                    layerGame.RestartLobby();
                    break;
                default:
                    throw new Exception($"Неизвестное действие: {da}.");
            }
        }

        private void BtnBackToGame_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

        }
    }
}
