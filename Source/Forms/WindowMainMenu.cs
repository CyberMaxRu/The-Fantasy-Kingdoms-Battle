using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowMainMenu : CustomWindow
    {
        private VCBitmap bmpMenu;
        private VCButton btnTournament;
        //private VCButton btnPlayers;
        private VCButton btnPlayerPreferences;
        private VCButton btnGamePreferences;
        private VCButton btnAboutProgram;
        private VCButton btnExitToWindows;

        public WindowMainMenu()
        {
            bmpMenu = new VCBitmap(this, 0, 0, Program.formMain.bmpMainMenu);
            Width = bmpMenu.Width;
            Height = bmpMenu.Height;

            btnTournament = new VCButton(bmpMenu, 80, 88, "Турнир");
            btnTournament.Width = Width - 80 - 80;
            btnTournament.Click += BtnTournament_Click;

            btnExitToWindows = new VCButton(bmpMenu, 80, bmpMenu.Height - 96, "Выход в Windows");
            btnExitToWindows.Width = Width - 80 - 80;
            btnExitToWindows.Click += BtnExitToWindows_Click;

            btnAboutProgram = new VCButton(bmpMenu, 80, btnExitToWindows.ShiftY - 40, "О программе");
            btnAboutProgram.Width = Width - 80 - 80;
            btnAboutProgram.Click += BtnAboutProgram_Click;

            btnGamePreferences = new VCButton(bmpMenu, 80, btnAboutProgram.ShiftY - 40, "Настройки игры");
            btnGamePreferences.Width = Width - 80 - 80;
            btnGamePreferences.Click += BtnPreferences_Click;

            btnPlayerPreferences = new VCButton(bmpMenu, 80, btnGamePreferences.ShiftY - 40, "Настройки игрока");
            btnPlayerPreferences.Width = Width - 80 - 80;
            btnPlayerPreferences.Click += BtnPlayerPreferences_Click;

            //btnPlayers = new VCButton(bmpMenu, 80, btnPlayerPreferences.ShiftY - 40, "Игроки");
            //btnPlayers.Width = Width - 80 - 80;
            //btnPlayers.Click += BtnPlayers_Click;

            //CancelButton = btnExitToWindows;
        }

        private void BtnPlayerPreferences_Click(object sender, EventArgs e)
        {
            WindowPlayerPreferences w = new WindowPlayerPreferences();
            if (w.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void BtnPlayers_Click(object sender, EventArgs e)
        {
            
        }

        private void BtnExitToWindows_Click(object sender, EventArgs e)
        {
            Program.formMain.SetProgrameState(ProgramState.NeedQuit);
            CloseForm(DialogResult.Abort);
        }

        private void BtnAboutProgram_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowAboutProgram();
        }

        private void BtnPreferences_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowPreferences();
        }

        private void BtnTournament_Click(object sender, EventArgs e)
        {
            Program.formMain.StartNewLobby();
            CloseForm(DialogResult.OK);
        }
    }
}
