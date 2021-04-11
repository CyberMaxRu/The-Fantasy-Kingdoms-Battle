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
        private VCButton btnPreferences;
        private VCButton btnHelp;
        private VCButton btnExit;

        public WindowMainMenu()
        {
            bmpMenu = new VCBitmap(this, 0, 0, Program.formMain.bmpMainMenu);
            Width = bmpMenu.Width;
            Height = bmpMenu.Height;

            btnTournament = new VCButton(bmpMenu, 80, 88, "Турнир");
            btnTournament.Width = Width - 80 - 80;
            btnTournament.Click += BtnTournament_Click;

            btnExit = new VCButton(bmpMenu, 80, bmpMenu.Height - 96, "Выход");
            btnExit.Width = Width - 80 - 80;
            btnExit.Click += BtnExit_Click;

            btnHelp = new VCButton(bmpMenu, 80, btnExit.ShiftY - 48, "Справка");
            btnHelp.Width = Width - 80 - 80;
            btnHelp.Click += BtnHelp_Click;

            btnPreferences = new VCButton(bmpMenu, 80, btnHelp.ShiftY - 48, "Настройки");
            btnPreferences.Width = Width - 80 - 80;
            btnPreferences.Click += BtnPreferences_Click;

            CancelButton = btnExit;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Program.formMain.SetProgrameState(ProgramState.NeedQuit);
            CloseForm(DialogResult.Abort);
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowHelp();
        }

        private void BtnPreferences_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowPreferences();
        }

        private void BtnTournament_Click(object sender, EventArgs e)
        {
            
        }
    }
}
