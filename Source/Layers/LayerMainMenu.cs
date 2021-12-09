using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class LayerMainMenu : LayerCustom
    {
        private readonly Bitmap bitmapLogo;
        private readonly VCBitmap bitmapNameGame;
        private readonly VCBitmap bmpMainMenu;
        private readonly VCLabel labelVersion;
        private readonly VCButton btnTournament;
        private readonly VCButton btnPlayerPreferences;
        private readonly VCButton btnGamePreferences;
        private readonly VCButton btnAboutProgram;
        private readonly VCButton btnExitToWindows;

        public LayerMainMenu() : base()
        {
            // Фон
            bitmapLogo = LoadBitmap("Logo.png");
            bitmapNameGame = new VCBitmap(this, 0, 0, LoadBitmap("NameGame.png"));

            labelVersion = new VCLabel(this, 0, 0, Program.formMain.fontSmallC, Color.White, Program.formMain.fontSmall.MaxHeightSymbol,
                $"Сборка {FormMain.VERSION} от {FormMain.DATE_VERSION}");
            labelVersion.SetWidthByText();

            // Главное меню
            bmpMainMenu = new VCBitmap(this, 0, 0, LoadBitmap("MenuMain.png"));

            btnTournament = new VCButton(bmpMainMenu, 80, 88, "Турнир");
            btnTournament.Width = bmpMainMenu.Width - 80 - 80;
            btnTournament.Click += BtnTournament_Click;

            btnExitToWindows = new VCButton(bmpMainMenu, 80, bmpMainMenu.Height - 96, "Выход в Windows");
            btnExitToWindows.Width = bmpMainMenu.Width - 80 - 80;
            btnExitToWindows.Click += BtnExitToWindows_Click;

            btnAboutProgram = new VCButton(bmpMainMenu, 80, btnExitToWindows.ShiftY - 40, "О программе");
            btnAboutProgram.Width = bmpMainMenu.Width - 80 - 80;
            btnAboutProgram.Click += BtnAboutProgram_Click;

            btnGamePreferences = new VCButton(bmpMainMenu, 80, btnAboutProgram.ShiftY - 40, "Настройки игры");
            btnGamePreferences.Width = bmpMainMenu.Width - 80 - 80;
            btnGamePreferences.Click += BtnPreferences_Click;

            btnPlayerPreferences = new VCButton(bmpMainMenu, 80, btnGamePreferences.ShiftY - 40, "Настройки игрока");
            btnPlayerPreferences.Width = bmpMainMenu.Width - 80 - 80;
            btnPlayerPreferences.Click += BtnPlayerPreferences_Click;
        }

        internal override void ArrangeControls()
        {
            bitmapNameGame.ShiftX = (Width - bitmapNameGame.Width) / 2;
            bitmapNameGame.ShiftY = 32;//(bitmapLogo.ShiftY - bitmapNameGame.Height) / 2;
            labelVersion.ShiftX = Program.formMain.sizeGamespace.Width - labelVersion.Width - FormMain.Config.GridSize;
            labelVersion.ShiftY = Program.formMain.sizeGamespace.Height - labelVersion.Height - FormMain.Config.GridSize;
            bmpMainMenu.ShiftX = Program.formMain.sizeGamespace.Width - bmpMainMenu.Width - FormMain.Config.GridSize;
            bmpMainMenu.ShiftY = (Program.formMain.sizeGamespace.Height - bmpMainMenu.Height) / 2 - (FormMain.Config.GridSize * 1);

            base.ArrangeControls();
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            g.DrawImageUnscaled(bitmapLogo, 0, 0);
        }

        private void BtnTournament_Click(object sender, EventArgs e)
        {
            Program.formMain.StartNewLobby();
            if (Program.formMain.CurrentLobby != null)
                Program.formMain.ReturnFromLobby();
        }

        private void BtnPlayerPreferences_Click(object sender, EventArgs e)
        {
            WindowPlayerPreferences w = new WindowPlayerPreferences();
            w.ShowDialog();
        }

        private void BtnPreferences_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowPreferences();
        }

        private void BtnAboutProgram_Click(object sender, EventArgs e)
        {
            WindowAboutProgram w = new WindowAboutProgram();
            w.ShowDialog();
            w.Dispose();
            Program.formMain.ShowFrame(true);
        }

        private void BtnExitToWindows_Click(object sender, EventArgs e)
        {
            WindowConfirmExit f = new WindowConfirmExit();
            if (f.ShowDialog() == DialogAction.OK)
            {
                Program.formMain.SetProgrameState(ProgramState.NeedQuit);
                Program.formMain.Close();
            }
        }
    }
}