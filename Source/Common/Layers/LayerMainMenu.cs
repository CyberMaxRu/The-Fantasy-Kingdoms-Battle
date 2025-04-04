﻿using System;
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
        private readonly VCBitmap bmpNameGame;
        private readonly Bitmap[] arrayBitmapNameGame;
        private readonly VCBitmap bmpMainMenu;
        private readonly VCLabel labelVersion;
        private readonly VCLabel labelVersionName;
        private readonly VCButtonForMenu btnRandomMission;
        private readonly VCButtonForMenu btnEditorConquest;
        private readonly VCButtonForMenu btnPlayerPreferences;
        private readonly VCButtonForMenu btnGamePreferences;
        private readonly VCButtonForMenu btnAboutGame;
        private readonly VCButtonForMenu btnExitToWindows;

        private DescriptorMission descriptorMission;
        private LobbySettings mission;
        private WindowSetupMission wsm;

        private LayerEditorConquest layerEditor;

        private int idxAnimation;

        public LayerMainMenu() : base()
        {
            // Мигающий баннер с названием игры
            bmpNameGame = new VCBitmap(this, 0, 0, LoadBitmap("NameGame.png"));

            double deltaRad = Math.PI / Config.MainMenuFramesAnimationBanner;
            arrayBitmapNameGame = new Bitmap[Config.MainMenuFramesAnimationBanner];
            for (int i = 0; i < arrayBitmapNameGame.Length; i++)
                arrayBitmapNameGame[i] = GuiUtils.ApplyDisappearance(bmpNameGame.Bitmap, Config.MainMenuMinAlphaBanner + (int)((255 - Config.MainMenuMinAlphaBanner) * Math.Sin(deltaRad * i)), 255);

            // Информация о версии игры
            labelVersionName = new VCLabel(this, 0, 0, Program.formMain.FontSmallC, Color.White, Program.formMain.FontSmallC.MaxHeightSymbol, $"({FormMain.VERSION_POSTFIX})");
            labelVersionName.SetWidthByText();
            labelVersion = new VCLabel(this, 0, 0, Program.formMain.FontSmallC, Color.White, Program.formMain.FontSmallC.MaxHeightSymbol,
                $"Сборка {FormMain.VERSION} от {FormMain.DATE_VERSION}");
            labelVersion.SetWidthByText();

            // Главное меню
            bmpMainMenu = new VCBitmap(this, 0, 0, LoadBitmap("MenuMain.png"));

            btnRandomMission = new VCButtonForMenu(bmpMainMenu, 88, "Случайный сценарий", BtnRandomMission_Click);

            /*btnEditorConquest = new VCButtonForMenu(bmpMainMenu, 80, btnTournament.NextTop(), "Редактор Завоевания");
            btnEditorConquest.Width = bmpMainMenu.Width - 80 - 80;
            btnEditorConquest.Click += BtnEditorConquest_Click;*/

            btnExitToWindows = new VCButtonForMenu(bmpMainMenu, bmpMainMenu.Height - 96, "Выход", BtnExitToWindows_Click);
            btnAboutGame = new VCButtonForMenu(bmpMainMenu, btnExitToWindows.ShiftY - 40, "Об игре", BtnAboutGame_Click);
            btnGamePreferences = new VCButtonForMenu(bmpMainMenu, btnAboutGame.ShiftY - 40, "Настройки игры", BtnPreferences_Click);
            btnPlayerPreferences = new VCButtonForMenu(bmpMainMenu, btnGamePreferences.ShiftY - 40, "Настройки игрока", BtnPlayerPreferences_Click);
        }

        internal override void Focused(DialogAction da)
        {
            base.Focused(da);

            if (mission != null)
            {
                if (da == DialogAction.OK)
                {
                    Program.formMain.CurrentHumanPlayer.TournamentSettings[0] = mission;
                    FormMain.Descriptors.SaveHumanPlayers();
                    Program.formMain.layerGame.StartNewLobby(descriptorMission);
                }

                mission = null;
                descriptorMission = null;
            }
        }

        private void BtnRandomMission_Click(object sender, EventArgs e)
        {
            Assert(mission is null);
            Assert(descriptorMission is null);

            descriptorMission = new DescriptorMission(Program.WorkFolder + @"SinglePlayer\Missions\DemoMission1.xml");
            descriptorMission.TuneLinks();

            mission = new LobbySettings(Program.formMain.CurrentHumanPlayer.TournamentSettings[0]);
            wsm = new WindowSetupMission(mission);
            wsm.Show();
        }

        private void BtnEditorConquest_Click(object sender, EventArgs e)
        {
            if (layerEditor is null)
                layerEditor = new LayerEditorConquest();

            Program.formMain.ExchangeLayer(this, layerEditor);
       }

        internal override void ArrangeControls()
        {
            bmpNameGame.ShiftX = (Width - bmpNameGame.Width) / 2;
            bmpNameGame.ShiftY = 32;//(bitmapLogo.ShiftY - bitmapNameGame.Height) / 2;
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

            bmpNameGame.Bitmap = arrayBitmapNameGame[idxAnimation];

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

        private void BtnAboutGame_Click(object sender, EventArgs e)
        {
            WindowAboutProgram w = new WindowAboutProgram();
            w.Show();
        }

        private void BtnExitToWindows_Click(object sender, EventArgs e)
        {
            WindowConfirmExit.ConfirmExit();
        }

        internal override void ApplyCurrentWindowSize(Size size)
        {
            base.ApplyCurrentWindowSize(size);

            bitmapLogo = Program.formMain.CollectionBackgroundImage.GetBitmap("MainMenu", Program.formMain.Size);
            ArrangeControls();
        }
    }
}