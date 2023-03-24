using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class LayerMainMenu : LayerScene
    {
        private Bitmap bitmapLogo;
        private readonly VCBitmap bitmapNameGame;
        private readonly Bitmap[] arrayBitmapNameGame;
        private readonly VCBitmap bmpMainMenu;
        private readonly VCLabel labelVersion;
        private readonly VCLabel labelVersionName;
        private readonly VCButtonForMenu btnWarOfLords;
        private readonly VCButtonForMenu btnSingleMission;
        private readonly VCButtonForMenu btnEditorConquest;
        private readonly VCButtonForMenu btnPlayerPreferences;
        private readonly VCButtonForMenu btnGamePreferences;
        private readonly VCButtonForMenu btnAboutProgram;
        private readonly VCButtonForMenu btnExitToWindows;

        private LayerEditorConquest layerEditor;

        private int idxAnimation;

        public LayerMainMenu() : base()
        {
            // Фон
            bitmapNameGame = new VCBitmap(this, 0, 0, LoadBitmap("NameGame.png"));

            int minAlpha = Config.MainMenuMinAlphaBanner;
            int frames = Config.MainMenuFramesAnimationBanner;
            double deltaRad = Math.PI / frames;
            arrayBitmapNameGame = new Bitmap[frames];
            for (int i = 0; i < arrayBitmapNameGame.Length; i++)
            {
                int alpha = minAlpha + (int)((255 - minAlpha) * Math.Sin(deltaRad * i));
                arrayBitmapNameGame[i] = GuiUtils.ApplyDisappearance(bitmapNameGame.Bitmap, alpha, 255);
            }

            labelVersionName = new VCLabel(this, 0, 0, Program.formMain.FontSmallC, Color.White, Program.formMain.FontSmall.MaxHeightSymbol, $"({FormMain.VERSION_POSTFIX})");
            labelVersionName.SetWidthByText();
            labelVersion = new VCLabel(this, 0, 0, Program.formMain.FontSmallC, Color.White, Program.formMain.FontSmall.MaxHeightSymbol,
                $"Сборка {FormMain.VERSION} от {FormMain.DATE_VERSION}");
            labelVersion.SetWidthByText();

            // Главное меню
            bmpMainMenu = new VCBitmap(this, 0, 0, LoadBitmap("MenuMain.png"));

            //btnWarOfLords = new VCButtonForMenu(bmpMainMenu, 88, "Война лордов", BtnTournament_Click);
            //btnWarOfLords.Enabled = false;

            btnSingleMission = new VCButtonForMenu(bmpMainMenu, 88, "Одиночная игра", BtnSingleMission_Click);

            /*btnEditorConquest = new VCButtonForMenu(bmpMainMenu, 80, btnTournament.NextTop(), "Редактор Завоевания");
            btnEditorConquest.Width = bmpMainMenu.Width - 80 - 80;
            btnEditorConquest.Click += BtnEditorConquest_Click;*/

            btnExitToWindows = new VCButtonForMenu(bmpMainMenu, bmpMainMenu.Height - 96, "Выход", BtnExitToWindows_Click);
            btnAboutProgram = new VCButtonForMenu(bmpMainMenu, btnExitToWindows.ShiftY - 40, "О программе", BtnAboutProgram_Click);
            btnGamePreferences = new VCButtonForMenu(bmpMainMenu, btnAboutProgram.ShiftY - 40, "Настройки игры", BtnPreferences_Click);
            btnPlayerPreferences = new VCButtonForMenu(bmpMainMenu, btnGamePreferences.ShiftY - 40, "Настройки игрока", BtnPlayerPreferences_Click);
        }

        private void BtnSingleMission_Click(object sender, EventArgs e)
        {
            DescriptorMission m = new DescriptorMission(Program.WorkFolder + @"SinglePlayer\Missions\DemoMission1.xml");
            m.TuneLinks();

            LobbySettings ls = new LobbySettings(Program.formMain.CurrentHumanPlayer.TournamentSettings[0]);
            //WindowSetupTournament w = new WindowSetupTournament(ls);
            //if (w.ShowDialog() == DialogAction.OK)

            Program.formMain.CurrentHumanPlayer.TournamentSettings[0] = ls;
            FormMain.Descriptors.SaveHumanPlayers();
            Program.formMain.StartNewLobby(m);
        }

        private void BtnEditorConquest_Click(object sender, EventArgs e)
        {
            if (layerEditor is null)
                layerEditor = new LayerEditorConquest();

            Program.formMain.ExchangeLayer(this, layerEditor);
       }

        internal override void ArrangeControls()
        {
            bitmapNameGame.ShiftX = (Width - bitmapNameGame.Width) / 2;
            bitmapNameGame.ShiftY = 32;//(bitmapLogo.ShiftY - bitmapNameGame.Height) / 2;
            labelVersionName.ShiftX = labelVersionName.Parent.Width - labelVersionName.Width - FormMain.Config.GridSize;
            labelVersionName.ShiftY = labelVersionName.Parent.Height - labelVersionName.Height - FormMain.Config.GridSize;
            labelVersion.ShiftX = labelVersion.Parent.Width - labelVersion.Width - FormMain.Config.GridSize;
            labelVersion.ShiftY = labelVersionName.ShiftY - labelVersion.Height - FormMain.Config.GridSize;
            bmpMainMenu.ShiftX = Program.formMain.sizeGamespace.Width - bmpMainMenu.Width - FormMain.Config.GridSize;
            bmpMainMenu.ShiftY = (Program.formMain.sizeGamespace.Height - bmpMainMenu.Height) / 2 - (FormMain.Config.GridSize * 1);

            base.ArrangeControls();
        }

        internal override void DrawBackground(Graphics g)
        {
            Assert(Program.formMain.Size == bitmapLogo.Size);

            base.DrawBackground(g);

            DrawImage(g, bitmapLogo, 0, 0);
        }

        internal override void Draw(Graphics g)
        {
            idxAnimation++;
            if (idxAnimation == arrayBitmapNameGame.Length)
                idxAnimation = 0;

            bitmapNameGame.Bitmap = arrayBitmapNameGame[idxAnimation];

            base.Draw(g);
        }

        internal override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);

            if (e.KeyCode == Keys.Escape)
                btnExitToWindows.DoClick();
        }

        private void BtnTournament_Click(object sender, EventArgs e)
        {
            /*LobbySettings ls = new LobbySettings(Program.formMain.CurrentHumanPlayer.TournamentSettings[0]);
            WindowSetupTournament w = new WindowSetupTournament(ls);
            if (w.ShowDialog() == DialogAction.OK)
            {
                Program.formMain.CurrentHumanPlayer.TournamentSettings[0] = ls;
                FormMain.Descriptors.SaveHumanPlayers();
                Program.formMain.StartNewLobby();
            }*/
        }

        private void BtnPlayerPreferences_Click(object sender, EventArgs e)
        {
            WindowPlayerPreferences w = new WindowPlayerPreferences(null);
            w.Show();
        }

        private void BtnPreferences_Click(object sender, EventArgs e)
        {
            Program.formMain.ShowWindowPreferences();
        }

        private void BtnAboutProgram_Click(object sender, EventArgs e)
        {
            WindowAboutProgram w = new WindowAboutProgram();
            w.Show();
        }

        private void BtnExitToWindows_Click(object sender, EventArgs e)
        {
            WindowConfirmExit f = new WindowConfirmExit();
            f.Show();
        }

        internal override void ApplyCurrentWindowSize(Size size)
        {
            base.ApplyCurrentWindowSize(size);

            bitmapLogo = Program.formMain.CollectionBackgroundImage.GetBitmap("MainMenu", Program.formMain.Size);
            ArrangeControls();
        }
    }
}