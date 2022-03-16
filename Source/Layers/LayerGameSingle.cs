using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class LayerGameSingle : LayerCustom
    {
        // Главные страницы игры
        private readonly VCPageControl pageControl;
        private readonly VCPageButton pageResultTurn;
        private readonly VCPageButton pageFinance;
        private readonly VCPageButton pageHeroes;
        private readonly VCPageButton pageTournament;
        private readonly List<VCPageButton> pagesCapital;
        private readonly VCPageButton pageRealMap;
        private readonly VCPageButton pageMap;
        private readonly VCPageButton pageLocation;
        //private readonly VCPageButton pageTemples;

        private PanelWithPanelEntity panelWarehouse;
        private PanelWithPanelEntity panelHeroes;
        private readonly VisualControl vcRightPanel;
        private PanelWithPanelEntity panelCombatHeroes;

        private VCMap mapArdania;
        private DescriptorMap descriptorMap;

        // Поддержка режима отладки
        internal bool debugMode = false;
        internal Pen penDebugBorder = new Pen(Color.Red);
        internal VisualControl vcDebugInfo;
        internal VCLabel labelTimeDrawFrame;
        internal VCLabel labelLayers;
        internal DateTime startDebugAction;
        internal TimeSpan durationDrawFrame;

        private readonly List<VCIconButton48> listBtnLevelTax;

        internal readonly VisualControl MainControl;

        private readonly VisualControl panelPlayers;// Панель, на которой находятся панели игроков лобби

        // Контролы тулбара
        private readonly VCToolLabel labelDay;
        private readonly VCToolLabel labelBuilders;
        private readonly VCToolLabelResource[] labelsResources;
        private readonly VCToolLabel labelHeroes;
        private readonly VCToolLabel labelCorruption;
        private readonly VCToolLabel labelGreatness;
        private readonly VCLabel labelNamePlayer;

        private readonly VCIconButton48 btnInGameMenu;
        private readonly VCIconButton48 btnCheating;
        private readonly VCIconButton48 btnEndTurn;

        private readonly VisualControl panelLairWithFlags;
        private readonly List<VCButtonTargetLair> listBtnTargetLair = new List<VCButtonTargetLair>();
        private readonly List<VCImageLose> listBtnLoses = new List<VCImageLose>();

        private readonly VCBitmap bitmapMenu;
        private readonly VCBitmap bmpTopPanel;
        private readonly VCBitmap bmpPreparedToolbar;

        internal BigEntity selectedPlayerObject;


        private WindowAdvice winAdvice;

        private VCCell[] pageTournamentPlayers;
        private readonly List<VCResultRound> listResultRound = new List<VCResultRound>();
        private VCImage128[,] imgLocations;
        private PanelConstruction[,] constructionsOfLocation;

        private Lobby lobby;
        private Player curAppliedPlayer;

        internal Lobby CurrentLobby { get { return lobby; } }


        public LayerGameSingle() : base()
        {
            // Создаем панели игроков
            bmpTopPanel = new VCBitmap(this, 0, 0, null);
            panelPlayers = new VisualControl(bmpTopPanel, 0, Config.GridSize);

            CellPlayer pp;
            int nextLeftPanelPlayer = 0;
            for (int i = 0; i < Descriptors.TypeLobbies[0].QuantityPlayers; i++)
            {
                pp = new CellPlayer(panelPlayers, nextLeftPanelPlayer);
                nextLeftPanelPlayer = pp.NextLeft();
            }

            panelPlayers.ApplyMaxSize();

            // Полоса игрового тулбара
            bmpPreparedToolbar = new VCBitmap(this, 0, 0, null);

            // Главное игровое поле
            MainControl = new VisualControl(this, 0, 0);
            MainControl.Click += MainControl_Click;

            // Метки с информацией о Королевстве
            labelDay = new VCToolLabel(bmpPreparedToolbar, Config.GridSize, 6, "", FormMain.GUI_16_DAY);
            labelDay.Click += LabelDay_Click;
            labelDay.ShowHint += LabelDay_ShowHint;
            labelDay.Width = 80;
            labelBuilders = new VCToolLabel(bmpPreparedToolbar, labelDay.NextLeft() - 4, labelDay.ShiftY, "", FormMain.GUI_16_BUILDER);
            labelBuilders.ShowHint += LabelBuilders_ShowHint;
            labelBuilders.Width = 88;

            labelsResources = new VCToolLabelResource[Descriptors.BaseResources.Count];

            int nextLeft = labelBuilders.NextLeft() - 4;
            foreach (DescriptorBaseResource br in Descriptors.BaseResources)
            {
                VCToolLabelResource lblRes = new VCToolLabelResource(bmpPreparedToolbar, nextLeft, labelDay.ShiftY, br);
                lblRes.Width = 88;
                nextLeft = lblRes.NextLeft() - 4;
                labelsResources[br.Number] = lblRes;
            }

            labelHeroes = new VCToolLabel(bmpPreparedToolbar, nextLeft + 240, labelDay.ShiftY, "", FormMain.GUI_16_HEROES);
            labelHeroes.ShowHint += LabelHeroes_ShowHint;
            labelHeroes.Width = 96;
            labelCorruption = new VCToolLabel(bmpPreparedToolbar, labelHeroes.NextLeft(), labelDay.ShiftY, "", FormMain.GUI_16_CORRUPTION);
            labelCorruption.ShowHint += LabelCorruption_ShowHint;
            labelCorruption.Width = 128;
            labelGreatness = new VCToolLabel(bmpPreparedToolbar, labelCorruption.NextLeft(), labelDay.ShiftY, "", FormMain.GUI_16_GREATNESS);
            labelGreatness.ShowHint += LabelGreatness_ShowHint;
            labelGreatness.Width = 152;

            labelNamePlayer = new VCLabel(bmpPreparedToolbar, 0, 0, Program.formMain.fontMedCaptionC, Color.White, Program.formMain.fontMedCaptionC.MaxHeightSymbol, "");
            labelNamePlayer.StringFormat.LineAlignment = StringAlignment.Center;
            labelNamePlayer.Width = 16;

            btnInGameMenu = Program.formMain.CreateButton(bmpTopPanel, Config.Gui48_Settings, Config.GridSize, Config.GridSize, BtnInGameMenu_Click, null);
            btnInGameMenu.HighlightUnderMouse = true;
            btnInGameMenu.ShowBorder = false;
            btnInGameMenu.Hint = "Меню";
            btnInGameMenu.HintDescription = "Показать внутриигровое меню";
            btnCheating = Program.formMain.CreateButton(bmpTopPanel, Config.Gui48_Cheating, btnInGameMenu.NextLeft(), btnInGameMenu.ShiftY, BtnCheating_Click, null);
            btnCheating.HighlightUnderMouse = true;
            btnCheating.Hint = "Читинг";
            btnCheating.HintDescription = "Открыть настройки читинга";

            btnEndTurn = Program.formMain.CreateButton(bmpTopPanel, Config.Gui48_Hourglass, 0, Config.GridSize, BtnEndTurn_Click, null);
            btnEndTurn.HighlightUnderMouse = true;
            btnEndTurn.ShowBorder = true;
            btnEndTurn.Hint = "Конец хода";
            btnEndTurn.HintDescription = "Завершение хода";

            panelLairWithFlags = new VisualControl(MainControl, 0, Config.GridSize);
            panelLairWithFlags.Width = Program.formMain.imListObjects48.Size.Width;
            panelLairWithFlags.Height = Program.formMain.imListObjects48.Size.Height;

            // Отладочная информация
            vcDebugInfo = new VisualControl();
            labelTimeDrawFrame = new VCLabel(vcDebugInfo, Config.GridSize * 8, Config.GridSize, Program.formMain.fontParagraph, Color.White, 16, "");
            labelTimeDrawFrame.StringFormat.Alignment = StringAlignment.Near;
            labelTimeDrawFrame.Width = 300;
            labelLayers = new VCLabel(vcDebugInfo, labelTimeDrawFrame.ShiftX, labelTimeDrawFrame.NextTop(), Program.formMain.fontParagraph, Color.White, 16, "Layers");
            labelLayers.StringFormat.Alignment = StringAlignment.Near;
            labelLayers.Width = 300;
            vcDebugInfo.ApplyMaxSize();
            vcDebugInfo.ArrangeControls();

            // Правая панель с героями и меню
            vcRightPanel = new VisualControl(MainControl, 0, Config.GridSize);
            vcRightPanel.IsActiveControl = false;

            // Создаем меню
            bitmapMenu = new VCBitmap(vcRightPanel, 0, 0, LoadBitmap("Menu.png"));
            //Debug.Assert(panelHeroInfo.Width >= bitmapMenu.Width);

            CellsMenu = new VCMenuCell[FormMain.PANEL_MENU_CELLS.Height, FormMain.PANEL_MENU_CELLS.Width];
            for (int y = 0; y < FormMain.PANEL_MENU_CELLS.Height; y++)
                for (int x = 0; x < FormMain.PANEL_MENU_CELLS.Width; x++)
                    CellsMenu[y, x] = new VCMenuCell(bitmapMenu, 22 + (x * (Program.formMain.imListObjects48.Size.Width + FormMain.DISTANCE_BETWEEN_CELLS)), 95 + (y * (Program.formMain.imListObjects48.Size.Height + FormMain.DISTANCE_BETWEEN_CELLS)));

            // Панель со всеми героями
            panelCombatHeroes = new PanelWithPanelEntity(4, false, 12, 12);
            panelCombatHeroes.Click += PanelCombatHeroes_Click;
            vcRightPanel.AddControl(panelCombatHeroes);

            vcRightPanel.Width = Math.Max(bitmapMenu.Width, panelCombatHeroes.Width + Config.GridSize);

            // Панели информации об объектахs
            panelHeroInfo = new PanelHeroInfo(MainControl, Config.GridSize, panelLairWithFlags.NextTop());
            panelHeroInfo.Width = vcRightPanel.Width - Config.GridSize;
            panelHeroInfo.ApplyMaxSize();
            panelHeroInfo.Width = panelHeroInfo.Width + Config.GridSize;
            vcRightPanel.Width = Math.Max(vcRightPanel.Width, panelHeroInfo.Width);
            panelHeroInfo.Width = vcRightPanel.Width;

            panelConstructionInfo = new PanelConstructionInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
            panelConstructionInfo.Width = panelHeroInfo.Width;
            panelConstructionInfo.ApplyMaxSize();

            panelLairInfo = new PanelLairInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
            panelLairInfo.Width = panelHeroInfo.Width;
            panelLairInfo.ApplyMaxSize();

            panelMonsterInfo = new PanelMonsterInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
            panelMonsterInfo.Width = panelHeroInfo.Width;
            panelMonsterInfo.ApplyMaxSize();

            panelLocationInfo = new PanelLocationInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
            panelLocationInfo.Width = panelHeroInfo.Width;
            panelLocationInfo.ApplyMaxSize();

            panelEmptyInfo = new VisualControl(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY)
            {
                Width = panelHeroInfo.Width,
                Height = panelHeroInfo.Height,
                ShowBorder = true
            };

            // Страницы игры
            pageControl = new VCPageControl(MainControl, 0, panelLairWithFlags.ShiftY);
            pageControl.PageChanged += PageControl_PageChanged;
            pageResultTurn = pageControl.AddPage(Config.Gui48_ResultDay, "Итоги хода", "Сводка", null);
            pageResultTurn.Hint = "Итоги хода";
            pageFinance = pageControl.AddPage(Config.Gui48_Finance, "Финансы", "Информация о финансах", null);
            pageFinance.Hint = "Финансовая информация";
            pageHeroes = pageControl.AddPage(Config.Gui48_Heroes, "Герои", "Здесь можно посмотреть своих героев", PageHeroes_ShowHint);
            pageTournament = pageControl.AddPage(Config.Gui48_Tournament, "Турнир", "Здесь можно увидеть положение всех игроков на турнире", PageTournament_ShowHint);
            //pageRealMap = pageControl.AddPage(Config.Gui48_Map, "Карта Ардании", "Просмотр провинций Ардании", null);
            //pageRealMap.Hint = "Карта Ардании";
            pageControl.Separate();

            pagesCapital = new List<VCPageButton>();

            foreach (CapitalPage cp in Descriptors.CapitalPages)
            {
                VCPageButton pageCapital = pageControl.AddPage(cp.ImageIndex, cp.Name, "", null);
                pageCapital.Hint = cp.Description;
                pagesCapital.Add(pageCapital);
            }

            //pageTemples = pageControl.AddPage(Config.Gui48_Temple, "Храмы", "Храмы позволяют нанимать самых сильных героев", PageTemples_ShowHint);
            pageControl.Separate();
            pageMap = pageControl.AddPage(Config.Gui48_Map, "Карта графства", "Просмотр своих владений", null);
            pageMap.Hint = "Карта графства";
            pageLocation = pageControl.AddPage(0, "", "", null);
            pageLocation.Hint = "Тут должна быть подсказка";

            listBtnLevelTax = new List<VCIconButton48>();


            DrawPageConstructions();
            DrawPageFinance();
            DrawHeroes();
            DrawWarehouse();
            DrawPageTournament();
            DrawPageLocation();
            DrawPageRealMap();

            // Вычисляем максимальный размер страниц
            pageControl.ApplyMaxSize();
            pageControl.ShiftX = panelEmptyInfo.NextLeft();

            vcRightPanel.ShiftX = pageControl.NextLeft();
            vcRightPanel.ShiftY = panelLairWithFlags.NextTop();

            //
            Debug.Assert(panelConstructionInfo.Height > 0);
            Debug.Assert(panelLairInfo.Height > 0);
            Debug.Assert(panelHeroInfo.Height > 0);
            Debug.Assert(panelMonsterInfo.Height > 0);
            Debug.Assert(panelLocationInfo.Height > 0);

            int maxHeightPanelInfo = Math.Max(panelConstructionInfo.Height, panelLairInfo.Height);
            maxHeightPanelInfo = Math.Max(panelHeroInfo.Height, maxHeightPanelInfo);
            maxHeightPanelInfo = Math.Max(panelMonsterInfo.Height, maxHeightPanelInfo);
            int maxHeightControls = Math.Max(pageControl.Height, maxHeightPanelInfo);

            // Все контролы созданы, устанавливаем размеры bitmapMenu
            MainControl.Width = vcRightPanel.ShiftX + vcRightPanel.Width;
            bmpPreparedToolbar.Bitmap = PrepareToolbar();
            bmpPreparedToolbar.ShiftY = panelPlayers.NextTop();
            MainControl.ShiftY = bmpPreparedToolbar.NextTop() - Config.GridSize;

            bmpTopPanel.Bitmap = GuiUtils.MakeBackground(new Size(MainControl.Width, bmpPreparedToolbar.ShiftY));
            bmpTopPanel.Width = bmpTopPanel.Bitmap.Width;
            bmpTopPanel.Height = bmpTopPanel.Bitmap.Height;

            MainControl.Height = pageResultTurn.ShiftY + maxHeightControls + (Config.GridSize * 2);

            // Теперь когда известна ширина окна, можно создавать картинку тулбара
            labelNamePlayer.Height = bmpPreparedToolbar.Height;
            panelPlayers.ShiftX = (MainControl.Width - panelPlayers.Width) / 2;
            vcRightPanel.Height = MainControl.Height - panelLairWithFlags.NextTop();

            Program.formMain.sizeGamespace = new Size(MainControl.Width, MainControl.ShiftY + MainControl.Height);

            bitmapMenu.ShiftX = vcRightPanel.Width - bitmapMenu.Width;
            bitmapMenu.ShiftY = vcRightPanel.Height - bitmapMenu.Height;
            panelCombatHeroes.ShiftX = vcRightPanel.Width - panelCombatHeroes.Width - Config.GridSize;

            panelConstructionInfo.Height = MainControl.Height - panelConstructionInfo.ShiftY - Config.GridSize;
            panelLairInfo.Height = panelConstructionInfo.Height;
            panelHeroInfo.Height = panelConstructionInfo.Height;
            panelMonsterInfo.Height = panelConstructionInfo.Height;
            panelLocationInfo.Height = panelConstructionInfo.Height;
            panelEmptyInfo.Height = panelConstructionInfo.Height;

            btnEndTurn.ShiftX = btnEndTurn.Parent.Width - btnEndTurn.Width - Config.GridSize;

            bmpPreparedToolbar.ShiftX = 0;
            MainControl.ShiftX = 0;

            if (pageRealMap != null)
            {
                mapArdania.Width = pageRealMap.Page.Width;
                mapArdania.Height = pageRealMap.Page.Height;
            }

            pageResultTurn.PageImage = MainControlbackground("Paper");
            pageFinance.PageImage = MainControlbackground("Finance");
            pageHeroes.PageImage = MainControlbackground("Heroes");
            pageTournament.PageImage = MainControlbackground("Tournament");

            Width = Program.formMain.sizeGamespace.Width;
            Height = Program.formMain.sizeGamespace.Height;

            for (int i = 0; i < Descriptors.CapitalPages.Count; i++)
            {
                pagesCapital[i].PageImage = MainControlbackground(Descriptors.CapitalPages[i].NameTexture);
            }

            pageControl.ActivatePage(pageResultTurn);
            ShowNamePlayer(pageControl.CurrentPage.Caption);
        }

        private readonly VisualControl panelEmptyInfo;
        internal PanelConstructionInfo panelConstructionInfo { get; private set; }
        internal PanelLairInfo panelLairInfo { get; private set; }
        internal PanelHeroInfo panelHeroInfo { get; private set; }
        internal PanelMonsterInfo panelMonsterInfo { get; private set; }
        internal PanelLocationInfo panelLocationInfo { get; private set; }

        internal VCMenuCell[,] CellsMenu { get; }


        //
        private void DrawHeroes()
        {
            panelHeroes = new PanelWithPanelEntity(Config.HeroRows);
            pageHeroes.Page.AddControl(panelHeroes);
            panelHeroes.ShiftY = 0;

            List<Entity> list = new List<Entity>();
            for (int x = 0; x < Config.HeroRows * Config.HeroInRow; x++)
                list.Add(null);

            panelHeroes.ApplyList(list);
            panelHeroes.Height = panelHeroes.MaxSize().Height;
        }

        private void DrawPageTournament()
        {
            //private readonly VCCell[] pageTournamentPlayers;
            // Ячейки игроков
        }

        private void DrawPageRealMap()
        {
            if (pageRealMap != null)
            {
                mapArdania = new VCMap(pageRealMap.Page, 0, 0);
                mapArdania.Click += MapArdania_Click;
            }

            //descriptorMap = new DescriptorMap(mapArdania.Bitmap.Width, mapArdania.Bitmap.Height, mapArdania.Bitmap);
        }

        private void MapArdania_Click(object sender, EventArgs e)
        {
            Point p = mapArdania.MousePosToCoord(Program.formMain.MousePosToControl(mapArdania));
            descriptorMap.SearchBorder(p);
        }

        private void AdjustPageTournament()
        {
            if (pageTournamentPlayers is null)
            {
                int nextTop = 56;
                pageTournamentPlayers = new VCCell[lobby.Players.Length];
                for (int i = 0; i < pageTournamentPlayers.Length; i++)
                {
                    pageTournamentPlayers[i] = new VCCell(pageTournament.Page, 0, nextTop);
                    nextTop += 56;
                }

                pageTournament.Page.ArrangeControls();
            }

            foreach (VCResultRound rr in listResultRound)
                rr.Visible = false;

            while (listResultRound.Count < lobby.BattlesPlayers.Count)
            {
                listResultRound.Add(new VCResultRound(pageTournament.Page, pageTournamentPlayers[0].NextLeft() + (listResultRound.Count * 56), 0, lobby.Players.Length));
                pageTournament.Page.ArrangeControls();
            }

            foreach (Player lp in lobby.Players.OrderBy(lp => lp.PositionInLobby))
            {
                pageTournamentPlayers[lp.PositionInLobby - 1].Entity = lp;
            }

            for (int i = 0; i < lobby.BattlesPlayers.Count; i++)
            {
                listResultRound[i].ShowPlayers(lobby.Players, lobby.BattlesPlayers[i]);
            }
        }

        private void DrawWarehouse()
        {
            panelWarehouse = new PanelWithPanelEntity(Config.WarehouseWidth);
            pageHeroes.Page.AddControl(panelWarehouse);
            panelWarehouse.ShiftY = panelHeroes.NextTop();
        }

        internal void ShowWarehouse()
        {
            panelWarehouse.ApplyList(lobby.CurrentPlayer.Warehouse.ToList<Entity>());
        }

        internal void ActivatePageResultTurn()
        {
            pageControl.ActivatePage(pageResultTurn);
        }

        private void DrawPageFinance()
        {
            VCLabel l = new VCLabel(pageFinance.Page, 0, 0, Program.formMain.fontParagraph, Color.White, 16, "Уровень налогов:");
            l.Width = l.Font.WidthText(l.Text);

            int nextLeft = 0;
            foreach (DescriptorLevelTax lt in Descriptors.LevelTaxes)
            {
                VCIconButton48 btn = new VCIconButton48(pageFinance.Page, nextLeft, l.NextTop(), Config.Gui48_Money);
                btn.HighText = lt.Percent.ToString() + "%";
                btn.Hint = lt.Name;
                btn.Click += BtnLevelTax_Click;
                btn.Tag = lt.Index;
                listBtnLevelTax.Add(btn);

                nextLeft = btn.NextLeft();
            }
        }

        private void BtnLevelTax_Click(object sender, EventArgs e)
        {
            if (!((VCIconButton48)sender).ManualSelected)
            {
                listBtnLevelTax[curAppliedPlayer.CurrentLevelTax.Index].ManualSelected = false;
                curAppliedPlayer.CurrentLevelTax = Descriptors.LevelTaxes[((VCIconButton48)sender).Tag];
                ((VCIconButton48)sender).ManualSelected = true;
            }
        }

        internal void UpdateNeighborhoods()
        {
            Location l = curAppliedPlayer.CurrentLocation;
            Debug.Assert(l != null);

            pageLocation.ImageIndex = l.Settings.TypeLandscape.ImageIndex;
            pageLocation.Caption = l.Settings.Name;
            pageLocation.Hint = l.Settings.Name;

            for (int y = 0; y < lobby.TypeLobby.LairsHeight; y++)
                for (int x = 0; x < lobby.TypeLobby.LairsWidth; x++)
                {
                    constructionsOfLocation[y, x].Entity = l.Lairs[y, x];
                    constructionsOfLocation[y, x].Visible = constructionsOfLocation[y, x].Entity != null;
                }
        }

        private void ShowEvents()
        {
            Debug.Assert(curAppliedPlayer is PlayerHuman);

            PlayerHuman p = (PlayerHuman)curAppliedPlayer;
            int top = 0;

            foreach (VCEvent e in p.ListEvents)
            {
                e.SetParent(pageResultTurn.Page);
                e.ShiftX = 0;
                e.ShiftY = top;

                top = e.NextTop();
            }

            pageResultTurn.Page.ArrangeControls();
        }

        private void DrawPageConstructions()
        {
            // Создаем массив из страниц, линий и позиций
            PanelConstruction[,,] panels = new PanelConstruction[Descriptors.CapitalPages.Count, Config.ConstructionMaxLines, Config.ConstructionMaxPos];

            // Проходим по каждому зданию, создавая ему панель
            VisualControl parent;
            foreach (DescriptorConstruction tck in Descriptors.Constructions)
            {
                if (tck.IsInternalConstruction)
                {
                    parent = pagesCapital[tck.Page.Index].Page;

                    Debug.Assert(panels[tck.Page.Index, tck.CoordInPage.Y, tck.CoordInPage.X] == null);

                    if (parent != null)
                    {
                        tck.Panel = new PanelConstruction(parent, 0, 0);
                        tck.Panel.ShiftX = (tck.Panel.Width + Config.GridSize) * (tck.CoordInPage.X);
                        tck.Panel.ShiftY = (tck.Panel.Height + Config.GridSize) * (tck.CoordInPage.Y);
                        panels[tck.Page.Index, tck.CoordInPage.Y, tck.CoordInPage.X] = tck.Panel;
                    }
                }
            }
        }

        internal void LairsWithFlagChanged()
        {
            if (lobby.StateLobby == StateLobby.TurnHuman)
                AdjustPanelLairsWithFlags();
        }

        internal void LosesChanged()
        {
            if (lobby.StateLobby == StateLobby.TurnHuman)
                AdjustPanelLoses();
        }

        private void AdjustPanelLairsWithFlags()
        {
            Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);
            Debug.Assert(lobby.CurrentPlayer.ListFlags.Count > 0);

            // Приводим в соответствие количество кнопок и логов
            // Для этого скрываем все кнопки, а потом делаем их видимыми.
            // Это чтобы не создавать каждый раз заново кнопки при изменении их численности
            while (listBtnTargetLair.Count < lobby.CurrentPlayer.ListFlags.Count)
            {
                listBtnTargetLair.Add(new VCButtonTargetLair(panelLairWithFlags));
            }

            foreach (VCButtonTargetLair b in listBtnTargetLair)
                b.Visible = false;

            // Сортируем логова и переназначаем ссылки на них у кнопок
            int n = 0;
            int left = 0;
            foreach (Construction pl in lobby.CurrentPlayer.ListFlags)
            {
                listBtnTargetLair[n].ShiftX = left;
                listBtnTargetLair[n].Entity = pl;
                listBtnTargetLair[n].Visible = true;

                left = listBtnTargetLair[n].NextLeft();
                n++;
            }

            panelLairWithFlags.ShiftX = MainControl.Width - left;
            panelLairWithFlags.Width = left;
            MainControl.ArrangeControl(panelLairWithFlags);

            Program.formMain.SetNeedRedrawFrame();
        }

        private void AdjustNeighborhood()
        {
            Debug.Assert(lobby != null);

            for (int y = 0; y < lobby.TypeLobby.MapHeight; y++)
                for (int x = 0; x < lobby.TypeLobby.MapWidth; x++)
                {
                    imgLocations[y, x].ImageIndex = curAppliedPlayer.Locations[y, x].Settings.TypeLandscape.ImageIndex;
                    imgLocations[y, x].Entity = curAppliedPlayer.Locations[y, x];
                }
        }

        private void AdjustPanelLoses()
        {
            Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);

            // Приводим в соответствие количество кнопок и логов
            // Для этого скрываем все кнопки, а потом делаем их видимыми.
            // Это чтобы не создавать каждый раз заново кнопки при изменении их численности
            while (listBtnLoses.Count < lobby.CurrentPlayer.LoseInfo.Count)
            {
                listBtnLoses.Add(new VCImageLose(bmpPreparedToolbar, 0, 6));
            }

            foreach (VCImageLose b in listBtnLoses)
                b.Visible = false;

            // Сортируем логова и переназначаем ссылки на них у кнопок
            int n = 0;
            int left = bmpPreparedToolbar.Width - listBtnLoses[0].Width - Config.GridSize;
            foreach (LoseInfo li in lobby.CurrentPlayer.LoseInfo)
            {
                listBtnLoses[n].ShiftX = left;
                listBtnLoses[n].Info = li;
                listBtnLoses[n].Visible = true;

                left -= listBtnLoses[n].Width + Config.GridSize;
                n++;
            }

            bmpPreparedToolbar.ArrangeControls();
            Program.formMain.SetNeedRedrawFrame();
        }

        internal void ListHeroesChanged()
        {
            if (lobby != null)
            {
                Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);

                panelCombatHeroes.Visible = curAppliedPlayer.CombatHeroes.Count > 0;
                panelCombatHeroes.ApplyList(curAppliedPlayer.CombatHeroes);

                Program.formMain.SetNeedRedrawFrame();
            }
        }

        private Bitmap PrepareToolbar()
        {
            Bitmap bmp = new Bitmap(MainControl.Width, Program.formMain.bmpToolbar.Height);

            Graphics g = Graphics.FromImage(bmp);

            DrawBitmap(0, Program.formMain.bmpToolbar);
            DrawBitmap(0, Program.formMain.bmpToolbarBorder);
            DrawBitmap(bmp.Height - Program.formMain.bmpToolbarBorder.Height, Program.formMain.bmpToolbarBorder);

            g.Dispose();
            return bmp;

            void DrawBitmap(int top, Bitmap b)
            {
                int repeats = bmp.Width / b.Width;
                int restBorder = bmp.Width - (b.Width * repeats);

                for (int i = 0; i < repeats; i++)
                {
                    g.DrawImageUnscaled(b, i * b.Width, top);
                }

                g.DrawImageUnscaledAndClipped(b, new Rectangle(repeats * b.Width, top, restBorder, b.Height));
            }
        }

        internal void SelectPlayerObject(BigEntity po, int selectPage = -1)
        {
            if (selectedPlayerObject != po)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;

                if (selectedPlayerObject != null)
                {
                    selectedPlayerObject.HideInfo();
                }

                selectedPlayerObject = po;
                if ((po == null) && (pageControl.CurrentPage.Location != null))
                    selectedPlayerObject = pageControl.CurrentPage.Location;

                if (selectedPlayerObject != null)
                {
                    selectedPlayerObject.ShowInfo();
                }
                else
                    panelEmptyInfo.Visible = true;

                UpdateMenu();
                Program.formMain.SetNeedRedrawFrame();
            }
        }

        internal bool PlayerObjectIsSelected(Entity po)
        {
            Debug.Assert(po != null);

            return po == selectedPlayerObject;
        }

        internal void ObjectDestroyed(BigEntity entity)
        {
            Debug.Assert(entity != null);

            foreach (VCPageButton button in pageControl.Pages)
            {
                if (button.SelectedPlayerObject == entity)
                {
                    button.SelectedPlayerObject = null;
                }
            }
        }

        internal void SelectConstruction(Construction construction, int selectPage = -1)
        {
            if (construction.TypeConstruction.Page != null)
            {
                pageControl.ActivatePage(pagesCapital[construction.TypeConstruction.Page.Index]);
            }
            else
            {
                SetPageLocation(construction.Location, false);
                pageControl.ActivatePage(pageLocation);
            }

            SelectPlayerObject(construction, selectPage);
        }

        internal void SetPageLocation(Location l, bool showInfo)
        {
            curAppliedPlayer.CurrentLocation = l;
            UpdateNeighborhoods();
        }

        internal void UpdateMenu()
        {
            ClearMenu();

            if (selectedPlayerObject != null)
                selectedPlayerObject.MakeMenu(CellsMenu);

            for (int y = 0; y < FormMain.PANEL_MENU_CELLS.Height; y++)
                for (int x = 0; x < FormMain.PANEL_MENU_CELLS.Width; x++)
                    if (!CellsMenu[y, x].Used)
                        CellsMenu[y, x].Research = null;
        }

        internal void ClearMenu()
        {
            for (int y = 0; y < FormMain.PANEL_MENU_CELLS.Height; y++)
                for (int x = 0; x < FormMain.PANEL_MENU_CELLS.Width; x++)
                    CellsMenu[y, x].Used = false;
        }

        private void ShowLobby()
        {
            /*int top = 0;
            foreach (Player p in lobby.Players.OrderBy(p => p.PositionInLobby))
            {
                Debug.Assert(p.PositionInLobby >= 1);
                Debug.Assert(p.PositionInLobby <= lobby.TypeLobby.QuantityPlayers);

                p.Panel.ShiftY = top;
                top += p.Panel.Height + Config.GridSize;
            }*/

            //panelPlayers.ArrangeControls();

            // Показываем сооружения
            foreach (Construction pb in lobby.CurrentPlayer.Constructions)
            {
                if (pb.TypeConstruction.IsInternalConstruction && (pb.TypeConstruction.Category != CategoryConstruction.Temple))
                    pb.TypeConstruction.Panel.Entity = pb;
            }

            // Показываем логова
            UpdateNeighborhoods();

            foreach (VCIconButton48 b in listBtnLevelTax)
            {
                b.ManualSelected = b.Tag == curAppliedPlayer.CurrentLevelTax.Index;
            }

            // Показываем героев
            ShowEvents();
            AdjustPanelLoses();
            AdjustPanelLairsWithFlags();
            ListHeroesChanged();
        }

        internal void StartNewLobby()
        {
            Debug.Assert(lobby == null);

            lobby = new Lobby(Descriptors.TypeLobbies[0], this);

            for (int i = 0; i < panelPlayers.Controls.Count; i++)
            {
                Debug.Assert(panelPlayers.Controls[i] is CellPlayer);
                ((CellPlayer)panelPlayers.Controls[i]).Entity = lobby.Players[i];
            }

            if (Program.formMain.currentLayer != this)
            {
                Program.formMain.playerMusic.PlayMusic();
                Program.formMain.ExchangeLayer(Program.formMain.layerMainMenu, this);
            }

            if (imgLocations is null)
            {
                imgLocations = new VCImage128[lobby.TypeLobby.MapHeight, lobby.TypeLobby.MapWidth];

                for (int y = 0; y < lobby.TypeLobby.MapHeight; y++)
                    for (int x = 0; x < lobby.TypeLobby.MapWidth; x++)
                    {
                        imgLocations[y, x] = new VCImage128(pageMap.Page, x * (Program.formMain.imListObjects128.Size.Width + Config.GridSize), y * (Program.formMain.imListObjects128.Size.Height + Config.GridSize));
                        imgLocations[y, x].ShowHint += Location_ShowHint;
                        imgLocations[y, x].Click += Map_Click;
                    }

                pageMap.Page.ArrangeControls();
            }

            pageControl.ActivatePage(pageResultTurn);
            PageControl_PageChanged(null, new EventArgs());
            ShowCurrentPlayerLobby();

            lobby.Start();
        }

        private void Map_Click(object sender, EventArgs e)
        {
            Location l = (sender as VCImage128).Entity as Location;

            //Utils.Assert(imgLocations[curAppliedPlayer.CurrentLocation.Settings.Coord.Y, curAppliedPlayer.CurrentLocation.Settings.Coord.X].ManualSelected);
            imgLocations[curAppliedPlayer.CurrentLocation.Settings.Coord.Y, curAppliedPlayer.CurrentLocation.Settings.Coord.X].ManualSelected = false;
            SelectPlayerObject(l);
            SetPageLocation(l, true);
            imgLocations[curAppliedPlayer.CurrentLocation.Settings.Coord.Y, curAppliedPlayer.CurrentLocation.Settings.Coord.X].ManualSelected = true;
        }

        private void Location_ShowHint(object sender, EventArgs e)
        {
            Location l = (sender as VCImage128).Entity as Location;
            PanelHint.AddStep2Header(l.Settings.Name);
            PanelHint.AddStep5Description(l.Settings.TypeLandscape.Description);
        }

        internal void RestartLobby()
        {
            Debug.Assert(lobby != null);
            lobby.ExitFromLobby();
            lobby = null;

            StartNewLobby();
        }

        internal void EndLobby()
        {
            Debug.Assert(lobby != null);
            lobby.ExitFromLobby();

            ReturnFromLobby();
        }

        internal void ReturnFromLobby()
        {
            Debug.Assert(lobby != null);
            lobby = null;

            if (Program.formMain.ProgramState != ProgramState.NeedQuit)
            {
                Program.formMain.ExchangeLayer(this, Program.formMain.layerMainMenu);
                Program.formMain.ShowFrame(true);
                Program.formMain.playerMusic.PlayMainTheme();
            }
        }

        internal void ShowCurrentPlayerLobby()
        {
            if (lobby.CurrentPlayer == null)
            {
                MainControl.Visible = false;
            }
            else
            {
                if (lobby.CurrentPlayer.GetTypePlayer() == TypePlayer.Human)
                {
                    while (pageResultTurn.Page.Controls.Count > 0)
                        pageResultTurn.Page.RemoveControl(pageResultTurn.Page.Controls[0]);

                    btnEndTurn.ImageIsEnabled = true;
                    labelDay.Visible = true;
                    labelBuilders.Visible = true;
                    ShowResoures(true);
                    labelGreatness.Visible = true;
                    labelHeroes.Visible = true;
                    labelCorruption.Visible = true;
                    MainControl.Visible = true;
                    ShowDataPlayer();
                }
                else
                {
                    btnEndTurn.ImageIsEnabled = false;
                    labelDay.Visible = false;
                    labelBuilders.Visible = false;
                    ShowResoures(false);
                    labelGreatness.Visible = false;
                    labelHeroes.Visible = false;
                    labelCorruption.Visible = false;
                    MainControl.Visible = false;
                    foreach (VCImageLose il in listBtnLoses)
                        il.Visible = false;

                    ShowNamePlayer(lobby.CurrentPlayer.Descriptor.Name);
                }
            }

            Program.formMain.ShowFrame(true);

            void ShowResoures(bool visible)
            {
                foreach (VCToolLabelResource l in labelsResources)
                {
                    l.Visible = visible;
                }
            }
        }

        internal void ShowNamePlayer(string name)
        {
            Debug.Assert(name.Length > 0);

            if (labelNamePlayer.Text != name)
            {
                labelNamePlayer.Text = name;
                labelNamePlayer.Width = labelNamePlayer.Font.WidthText(labelNamePlayer.Text);
                labelNamePlayer.ShiftX = (bmpPreparedToolbar.Width - labelNamePlayer.Width) / 2;
                bmpPreparedToolbar.ArrangeControl(labelNamePlayer);
            }
        }

        internal void ShowDataPlayer()
        {
            Debug.Assert(lobby.CurrentPlayer.GetTypePlayer() == TypePlayer.Human);

            labelDay.Text = $"{lobby.Month}.{lobby.Week}.{lobby.Day}";

            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayer)
            {
                curAppliedPlayer = lobby.CurrentPlayer;
            }

            ShowLobby();

            LairsWithFlagChanged();
            LosesChanged();
            UpdateListHeroes();
            ShowWarehouse();
            AdjustPageTournament();
            UpdateNeighborhoods();
            AdjustNeighborhood();
            ShowPlayerNotices();
        }

        internal void ShowPlayerNotices()
        {
            pageResultTurn.LowText = curAppliedPlayer.ListNoticesForPlayer.Count > 0 ? curAppliedPlayer.ListNoticesForPlayer.Count.ToString() : "";

            if (curAppliedPlayer.ListNoticesForPlayer.Count > 0)
            {
                int nextY = 0;

                foreach (VCNoticeForPlayer ep in curAppliedPlayer.ListNoticesForPlayer)
                {
                    ep.ShiftY = nextY;
                    ep.Visible = true;
                    if ((ep.Parent is null) || (ep.Parent != pageResultTurn.Page))
                        pageResultTurn.Page.AddControl(ep);

                    pageResultTurn.Page.ArrangeControl(ep);
                    nextY = ep.NextTop();
                }
            }
        }

        internal void UpdateListHeroes()
        {
            //List<ICell> listHeroes = new List<ICell>();
            //for (int y = 0; y < lobby.CurrentPlayer.CellHeroes.GetLength(0); y++)
            //    for (int x = 0; x < lobby.CurrentPlayer.CellHeroes.GetLength(1); x++)
            //        listHeroes.Add(lobby.CurrentPlayer.CellHeroes[y, x]);

            //panelHeroes.ApplyList(listHeroes);
        }

        private void BtnInGameMenu_Click(object sender, EventArgs e)
        {
            ShowInGameMenu();
        }

        private void BtnEndTurn_Click(object sender, EventArgs e)
        {
            Program.formMain.StopSoundSelect();
            PanelHint.HideHint();

            curAppliedPlayer.EndTurn();
        }
        private void DrawPageLocation()
        {
            constructionsOfLocation = new PanelConstruction[FormMain.MAX_LAIRS_HEIGHT, FormMain.MAX_LAIRS_WIDTH];

            int top = 0;
            int left;
            int height = 0;

            for (int y = 0; y < FormMain.MAX_LAIRS_HEIGHT; y++)
            {
                left = 0;
                for (int x = 0; x < FormMain.MAX_LAIRS_WIDTH; x++)
                {
                    Debug.Assert(constructionsOfLocation[y, x] == null);
                    constructionsOfLocation[y, x] = new PanelConstruction(pageLocation.Page, left, top);

                    left += constructionsOfLocation[y, x].Width + Config.GridSize;
                    height = constructionsOfLocation[y, x].Height;
                }

                top += height + Config.GridSize;
            }

            pageLocation.Page.ArrangeControls();
        }

        private Bitmap MainControlbackground(string nameTexture)
        {
            return GuiUtils.MakeCustomBackground(Config.GetTexture(nameTexture), MainControl);
        }

        private void BtnCheating_Click(object sender, EventArgs e)
        {
            WindowCheating w = new WindowCheating(curAppliedPlayer);
            w.ShowDialog();
        }

        private void LabelCorruption_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Воровство");
            PanelHint.AddStep5Description($"Всего процент: {curAppliedPlayer.PercentCorruption}" + Environment.NewLine
                + $"Изменение за день: {curAppliedPlayer.ChangeCorruption}");
        }

        private void LabelHeroes_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Герои");
            PanelHint.AddStep5Description($"Нанято героев: {curAppliedPlayer.CombatHeroes.Count}" + Environment.NewLine
                + $"Максимум героев: {curAppliedPlayer.Lobby.TypeLobby.MaxHeroes}");
        }

        private void PageControl_PageChanged(object sender, EventArgs e)
        {
            if (Program.formMain.currentLayer == this)
            {
                ShowNamePlayer(pageControl.CurrentPage.Caption);
                //if (winAdvice is null)
                //    winAdvice = new WindowAdvice();
                //winAdvice.ShowAdvice(pageControl.CurrentPage.Advice);
            }

            MainControl.BackgroundImage = pageControl.CurrentPage.PageImage;
        }

        private void LabelBuilders_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Строители");
            PanelHint.AddStep5Description("Всего строителей: " + curAppliedPlayer.Builders.ToString()
                + Environment.NewLine + "Свободно строителей: " + curAppliedPlayer.FreeBuilders.ToString());
        }

        private void PanelCombatHeroes_Click(object sender, EventArgs e)
        {
            SelectPlayerObject(null);
        }

        private void MainControl_Click(object sender, EventArgs e)
        {
            SelectPlayerObject(null);
        }

        private void LabelGreatness_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Уровень величия: " + curAppliedPlayer.LevelGreatness.ToString());
            PanelHint.AddStep5Description($"Очков набрано: {curAppliedPlayer.PointGreatness} из {curAppliedPlayer.PointGreatnessForNextLevel}"
                + Environment.NewLine
                + "До следующего уровня: " + (curAppliedPlayer.PointGreatnessForNextLevel - curAppliedPlayer.PointGreatness).ToString()
                + Environment.NewLine
                + "Прибавление в день: " + curAppliedPlayer.PointGreatnessPerDay().ToString());
        }

        private void LabelDay_Click(object sender, EventArgs e)
        {
            debugMode = !debugMode;
            labelTimeDrawFrame.Visible = debugMode;
            labelLayers.Visible = debugMode;
            Program.formMain.ShowFrame(true);
        }

        private void PageHeroes_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Герои");
            PanelHint.AddStep5Description("Нанято героев: " + lobby.CurrentPlayer.CombatHeroes.Count.ToString());
        }

        private void PageTournament_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header("Турнир");
            PanelHint.AddStep5Description(lobby.DaysLeftForBattle > 0 ? "Битва с другим игроком начнется через " + lobby.DaysLeftForBattle.ToString() + " дн." :
                    curAppliedPlayer.SkipBattle ? "Битва пропускается" : "Сегодня битва с другим игроком");
        }

        private void LabelDay_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Header($"Ход игры: {lobby.Turn}");
            PanelHint.AddStep5Description($"День: {lobby.Day}{Environment.NewLine}Неделя: {lobby.Week}{Environment.NewLine}Месяц: {lobby.Day}{Environment.NewLine}");
            PanelHint.AddStep21Tooltip($"В неделе 7 дней.{Environment.NewLine}В месяце 4 недели.");
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if ((lobby != null) && (lobby.CurrentPlayer != null) && MainControl.Visible)
            {
                labelBuilders.Text = $"{curAppliedPlayer.FreeBuilders}/{curAppliedPlayer.Builders}";
                labelGreatness.Text = curAppliedPlayer.LevelGreatness.ToString()
                    + " (+" + curAppliedPlayer.PointGreatnessPerDay().ToString() + ")"
                    + ": " + curAppliedPlayer.PointGreatness.ToString() + "/"
                    + curAppliedPlayer.PointGreatnessForNextLevel.ToString();
                labelHeroes.Text = curAppliedPlayer.CombatHeroes.Count.ToString() + "/" + curAppliedPlayer.Lobby.TypeLobby.MaxHeroes.ToString();
                labelCorruption.Text = $"{curAppliedPlayer.PercentCorruption}% ({(curAppliedPlayer.ChangeCorruption > 0 ? "+" : "")}{curAppliedPlayer.ChangeCorruption}%)";

                pageTournament.LowText = lobby.DaysLeftForBattle > 0 ? lobby.DaysLeftForBattle.ToString() + " д." :
                        curAppliedPlayer.SkipBattle ? "Проп." : "Битва";

                foreach (VCToolLabelResource l in labelsResources)
                {
                    l.UpdateData(curAppliedPlayer);
                }
            }
        }

        private void ShowInGameMenu()
        {
            WindowMenuInGame w = new WindowMenuInGame(CurrentLobby);
            DialogAction dr = w.ShowDialog();
            switch (dr)
            {
                case DialogAction.None:
                    break;
                case DialogAction.Quit:
                    Program.formMain.SetProgrameState(ProgramState.NeedQuit);
                    if (!(lobby is null))
                        EndLobby();
                    Program.formMain.Close();
                    break;
                case DialogAction.MainMenu:
                    EndLobby();
                    break;
                case DialogAction.RestartGame:
                    RestartLobby();
                    break;
                default:
                    throw new Exception($"Неизвестное действие: {dr}.");
            }

            if (Program.formMain.ProgramState != ProgramState.NeedQuit)
                Program.formMain.ShowFrame(true);
        }

        internal override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);

            if (e.KeyCode == Keys.Escape)
            {
                ShowInGameMenu();
            }

        }
    }
}