using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Threading;
using Fantasy_Kingdoms_Battle.Source;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class LayerGameSingle : LayerScene
    {
        // Главные страницы игры
        private readonly VCPageControl pageControl;        
        private readonly VCPageButton pageFinance;
        private readonly VCPageButton pageHeroes;
        private readonly VCPageButton pageTournament;
        private readonly VCPageButton pageTraditions;
        private readonly VCPageButton pageSpell;
        private readonly List<VCAcceptedTradition> listAcceptedTraditions = new List<VCAcceptedTradition>();
        private readonly List<VCPageButton> pagesCapital;
        //private readonly VCPageButton pageLocation;
        //private readonly VCPageButton pageTemples;
        private readonly VCLabel labelCaptionPage;

        private PanelWithPanelEntity panelWarehouse;
        private PanelWithPanelEntity panelHeroes;
        private readonly VisualControl vcRightPanel;
        private PanelWithPanelEntity panelCombatHeroes;

        // Поддержка реального времени игры
        private Stopwatch internalTimer;

        // Поддержка режима отладки
        internal bool debugMode = false;
        internal Pen penDebugBorder = new Pen(Color.Red);
        internal VisualControl vcDebugInfo;
        internal VCLabel labelTimeDrawFrame;
        internal VCLabel labelLayers;
        internal DateTime startDebugAction;
        internal TimeSpan durationDrawFrame;
        internal DateTime firstFrameOfSecond;
        internal int countFrames;
        internal int countTicks;
        internal int framesPerSecond;
        internal int ticksPerSecond;

        internal readonly VisualControl MainControl;
        internal readonly VisualControl panelNotices;// Панель извещений

        private readonly VisualControl panelPlayers;// Панель, на которой находятся панели игроков лобби

        // Контролы над тулбаром
        private readonly VCLabelValue labelDay;
        private readonly VCLabelValue labelTraditions;
        private readonly VCProgressBar pbTraditions;
        private readonly VCLabelValue labelMana;

        private readonly VCIconButton48 btnInGameMenu;
        private readonly VCIconButton48 btnCheating;

        // Контролы тулбара
        private readonly VCToolLabel labelKnowledge;
        //private readonly VCToolLabel labelPeople;
        private readonly VCToolLabel labelGreatness;
        private readonly VCLabel labelNamePlayer;

        private readonly VisualControl panelLairWithFlags;
        //private readonly List<VCImageLose> listBtnLoses = new List<VCImageLose>();

        private readonly PanelConstruction[,,] panels;
        private readonly VCBitmap bmpObjectMenu;
        private readonly VCMenuCell cellObjectMenu;
        private readonly VCBitmap bmpTopPanel;
        private readonly VCBitmap bmpPreparedToolbar;

        internal BigEntity selectedPlayerObject;

        private WindowAdvice winAdvice;

        private VCCell[] pageTournamentPlayers;
        private readonly List<VCResultRound> listResultRound = new List<VCResultRound>();
        //private PanelConstruction[,] constructionsOfLocation;

        private Lobby lobby;
        private Player curAppliedPlayer;

        int horInterval;
        int verInterval;

        internal Lobby CurrentLobby { get { return lobby; } }


        public LayerGameSingle() : base()
        {
            horInterval = Config.GridSize;
            verInterval = Config.GridSize;

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

            /*labelsResources = new VCToolLabelResource[Descriptors.BaseResources.Count];
            foreach (DescriptorBaseResource br in Descriptors.BaseResources)
            {
                VCToolLabelResource lblRes = new VCToolLabelResource(bmpPreparedToolbar, 0, 6, br);
                labelsResources[br.Number] = lblRes;
            }*/

            labelNamePlayer = new VCLabel(bmpPreparedToolbar, 0, 0, Program.formMain.FontMedCaptionC, Color.White, Program.formMain.FontMedCaptionC.MaxHeightSymbol, "");
            labelNamePlayer.StringFormat.LineAlignment = StringAlignment.Center;
            labelNamePlayer.Width = 16;

            // Контролы над тулбаром
            labelDay = new VCLabelValue(bmpTopPanel, Config.GridSize, Config.GridSize, Color.White, true);
            labelDay.StringFormat.Alignment = StringAlignment.Far;
            labelDay.Click += LabelDay_Click;
            labelDay.ShowHint += LabelDay_ShowHint;
            labelDay.Width = 72;
            labelDay.RightMargin = 6;

            labelTraditions = new VCLabelValue(bmpTopPanel, labelDay.ShiftX, labelDay.NextTop() - Config.GridSize, Color.White, true);
            labelTraditions.Image.ImageIndex = FormMain.GUI_16_TRADITIONS;
            labelTraditions.StringFormat.Alignment = StringAlignment.Far;
            labelTraditions.RightMargin = 6;
            //labelTraditions.ShowHint += LabelKnowledge_ShowHint;
            labelTraditions.Width = labelDay.Width;
            pbTraditions = new VCProgressBar(bmpTopPanel, labelDay.ShiftX, labelTraditions.ShiftY);
            pbTraditions.Width = 160;

            labelMana = new VCLabelValue(bmpTopPanel, labelDay.NextLeft(), labelDay.ShiftY, Color.White, true);
            labelMana.Image.ImageIndex = FormMain.GUI_16_MANA;
            labelMana.Width = 112;
            labelGreatness = new VCToolLabel(bmpPreparedToolbar, pbTraditions.NextLeft() - Config.GridSizeHalf, pbTraditions.ShiftY, "", FormMain.GUI_16_GREATNESS);
            labelGreatness.ShowHint += LabelGreatness_ShowHint;
            labelGreatness.Width = 112;



            btnInGameMenu = CreateButton(bmpTopPanel, Config.Gui48_Settings, Config.GridSize, Config.GridSize, BtnInGameMenu_Click, null);
            btnInGameMenu.HighlightUnderMouse = true;
            btnInGameMenu.ShowBorder = false;
            btnInGameMenu.Hint = "Меню";
            btnInGameMenu.HintDescription = "Показать внутриигровое меню";
            btnCheating = CreateButton(bmpTopPanel, Config.Gui48_Cheating, btnInGameMenu.NextLeft(), btnInGameMenu.ShiftY, BtnCheating_Click, null);
            btnCheating.HighlightUnderMouse = true;
            btnCheating.Hint = "Читинг";
            btnCheating.HintDescription = "Открыть настройки читинга";

            panelLairWithFlags = new VisualControl(MainControl, 0, Config.GridSize);
            panelLairWithFlags.Width = Program.formMain.BmpListObjects48.Size.Width;
            panelLairWithFlags.Height = Program.formMain.BmpListObjects48.Size.Height;

            // Отладочная информация
            vcDebugInfo = new VisualControl();
            labelTimeDrawFrame = new VCLabel(vcDebugInfo, btnCheating.NextLeft(), Config.GridSize, Program.formMain.FontParagraph, Color.White, 16, "");
            labelTimeDrawFrame.StringFormat.Alignment = StringAlignment.Near;
            labelTimeDrawFrame.Width = 300;
            labelLayers = new VCLabel(vcDebugInfo, labelTimeDrawFrame.ShiftX, labelTimeDrawFrame.NextTop(), Program.formMain.FontParagraph, Color.White, 16, "Layers");
            labelLayers.StringFormat.Alignment = StringAlignment.Near;
            labelLayers.Width = 300;
            vcDebugInfo.ApplyMaxSize();
            vcDebugInfo.ArrangeControls();

            // Правая панель с героями и меню
            vcRightPanel = new VisualControl(MainControl, 0, Config.GridSize);
            vcRightPanel.IsActiveControl = false;

            // Создаем меню
            bmpObjectMenu = new VCBitmap(vcRightPanel, 0, 0, LoadBitmap("Menu.png"));
            //Debug.Assert(panelHeroInfo.Width >= bitmapMenu.Width);

            int addShift = bmpObjectMenu.Width - FormMain.Config.ObjectMenuWidth + 22;
            CellsMenu = new VCMenuCell[FormMain.PANEL_MENU_CELLS.Height, FormMain.PANEL_MENU_CELLS.Width];
            for (int y = 0; y < FormMain.PANEL_MENU_CELLS.Height; y++)
                for (int x = 0; x < FormMain.PANEL_MENU_CELLS.Width; x++)
                    CellsMenu[y, x] = new VCMenuCell(bmpObjectMenu, addShift + (x * (Program.formMain.BmpListObjects48.Size.Width + FormMain.DISTANCE_BETWEEN_CELLS)), 95 + (y * (Program.formMain.BmpListObjects48.Size.Height + FormMain.DISTANCE_BETWEEN_CELLS)));

            cellObjectMenu = new VCMenuCell(bmpObjectMenu, addShift + 4, 40);
            cellObjectMenu.ManualDraw = true;
            cellObjectMenu.ShowHint += CellObjectMenu_ShowHint;

            // Панель со всеми героями
            panelCombatHeroes = new PanelWithPanelEntity(5, false, 12, 12);
            panelCombatHeroes.Width += FormMain.Config.GridSize * 2;
            panelCombatHeroes.Click += PanelCombatHeroes_Click;
            vcRightPanel.AddControl(panelCombatHeroes);

            vcRightPanel.Width = Math.Max(FormMain.Config.ObjectMenuWidth, panelCombatHeroes.Width + Config.GridSize);

            // Панели информации об объектахs
            panelHeroInfo = new PanelHeroInfo(MainControl, Config.GridSize, panelLairWithFlags.ShiftY);
            panelHeroInfo.Width = vcRightPanel.Width - Config.GridSize;
            panelHeroInfo.ApplyMaxSize();
            panelHeroInfo.Width = panelHeroInfo.Width + Config.GridSize;
            vcRightPanel.Width = Math.Max(vcRightPanel.Width, panelHeroInfo.Width);
            panelHeroInfo.Width = vcRightPanel.Width;

            panelConstructionInfo = new PanelConstructionInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
            panelConstructionInfo.Width = panelHeroInfo.Width;
            panelConstructionInfo.ApplyMaxSize();

            panelMonsterInfo = new PanelMonsterInfo(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY);
            panelMonsterInfo.Width = panelHeroInfo.Width;
            panelMonsterInfo.ApplyMaxSize();

            panelEmptyInfo = new VisualControl(MainControl, panelHeroInfo.ShiftX, panelHeroInfo.ShiftY)
            {
                Width = panelHeroInfo.Width,
                Height = panelHeroInfo.Height,
                ShowBorder = true
            };

            // Страницы игры
            pageControl = new VCPageControl(MainControl, 0, panelLairWithFlags.ShiftY);
            pageControl.PageChanged += PageControl_PageChanged;
            //pageFinance = pageControl.AddPage(Config.Gui48_Finance, "Финансы", "Информация о финансах", null);
            //pageFinance.Hint = "Финансовая информация";
            pageHeroes = pageControl.AddPage(Config.Gui48_Heroes, "Герои", "Здесь можно посмотреть своих героев", PageHeroes_ShowHint);
            pageTournament = pageControl.AddPage(Config.Gui48_Tournament, "Турнир", "Здесь можно увидеть положение всех игроков на турнире", PageTournament_ShowHint);
            pageTraditions = pageControl.AddPage(Config.Gui48_Tradition, "Традиции", "Здесь традиции", null);
            pageSpell = pageControl.AddPage(Config.Gui48_CastSpell, "Заклинания", "Здесь колдуют заклинания", null);
            //pageRealMap = pageControl.AddPage(Config.Gui48_Map, "Карта Ардании", "Просмотр провинций Ардании", null);
            //pageRealMap.Hint = "Карта Ардании";
            pageControl.Separate();

            pagesCapital = new List<VCPageButton>();

            foreach (CapitalPage cp in Descriptors.CapitalPages)
            {
                VCPageButton pageCapital = pageControl.AddPage(cp.ImageIndex, cp.Name, "", null);
                pageCapital.Hint = cp.Name;
                pageCapital.HintDescription = cp.Description;
                pagesCapital.Add(pageCapital);
            }

            //pageTemples = pageControl.AddPage(Config.Gui48_Temple, "Храмы", "Храмы позволяют нанимать самых сильных героев", PageTemples_ShowHint);
            //pageControl.Separate();

            labelCaptionPage = new VCLabel(bmpPreparedToolbar, 0, 0, Program.formMain.FontMedCaptionC, Color.White, 48, "");
            labelCaptionPage.StringFormat.Alignment = StringAlignment.Center;
            labelCaptionPage.StringFormat.LineAlignment = StringAlignment.Center;
            labelCaptionPage.Width = 280;

            // Создаем массив из страниц, линий и позиций
            panels = new PanelConstruction[Descriptors.CapitalPages.Count, Config.ConstructionMaxLines, Config.ConstructionMaxPos];

            DrawPageConstructions();
            //DrawPageFinance();
            DrawHeroes();
            DrawWarehouse();
            DrawPageTournament();
            DrawPageLocation();

            panelNotices = new VisualControl(vcRightPanel, 0, 0);
            panelNotices.Width = vcRightPanel.Width - Config.GridSize;
            panelNotices.Height = vcRightPanel.Height;
            panelNotices.IsActiveControl = false;
            panelNotices.ShowBorder = false;

            // Вычисляем максимальный размер страниц
            pageControl.ApplyMaxSize();
            pageControl.ShiftX = panelEmptyInfo.NextLeft();

            vcRightPanel.ShiftX = pageControl.NextLeft();
            vcRightPanel.ShiftY = panelLairWithFlags.NextTop();

            //
            Debug.Assert(panelConstructionInfo.Height > 0);
            Debug.Assert(panelHeroInfo.Height > 0);
            Debug.Assert(panelMonsterInfo.Height > 0);

            int maxHeightPanelInfo = Math.Max(panelConstructionInfo.Height, panelHeroInfo.Height);
            maxHeightPanelInfo = Math.Max(panelMonsterInfo.Height, maxHeightPanelInfo);
            int maxHeightControls = Math.Max(pageControl.Height, maxHeightPanelInfo);

            // Все контролы созданы, устанавливаем размеры bitmapMenu
            MainControl.Width = vcRightPanel.ShiftX + vcRightPanel.Width;
            MainControl.Height = pageHeroes.ShiftY + maxHeightControls + (Config.GridSize * 2);

            Adjust2();

            labelCaptionPage.ShiftX = (labelCaptionPage.Parent.Width - labelCaptionPage.Width) / 2;
            labelCaptionPage.Height = labelCaptionPage.Parent.Height;

            PreferencesChanged();

            // Теперь когда известна ширина окна, можно создавать картинку тулбара
            Program.formMain.sizeGamespace = new Size(MainControl.Width, MainControl.ShiftY + MainControl.Height);
            Program.formMain.MinSizeGamespace = Program.formMain.sizeGamespace;



            bmpPreparedToolbar.ShiftX = 0;
            MainControl.ShiftX = 0;

            Width = Program.formMain.sizeGamespace.Width;
            Height = Program.formMain.sizeGamespace.Height;

            MakePagesBackground();

            pageControl.ActivatePage(pagesCapital[0]);
            UpdateNameCurrentPage();

            // Сразу создаем контролы под традиции. Они все равно обязательно пригодятся
            int nextLeft = 0;
            int nextTop = 0;
            for (int i = 1; i <= FormMain.Config.MaxTraditions; i++)
            {
                VCAcceptedTradition at = new VCAcceptedTradition(pageTraditions.Page, nextLeft, nextTop);
                at.Visible = false;
                listAcceptedTraditions.Add(at);

                if (i % FormMain.Config.TraditionsPerColumn > 0)
                {
                    nextTop = at.NextTop();
                }
                else
                {
                    nextLeft = at.NextLeft();
                    nextTop = 0;
                }
            }
        }

        // Сейчас будет рисоваться кадр. Делаем расчеты тактов игры
        internal override void BeforeDrawFrame()
        {
            base.BeforeDrawFrame();

        }

        private VCIconButton48 CreateButton(VisualControl parent, int imageIndex, int left, int top, EventHandler click, EventHandler showHint)
        {
            VCIconButton48 b = new VCIconButton48(parent, left, top, imageIndex);
            b.Click += click;
            b.ShowHint += showHint;

            return b;
        }


        private void LabelKnowledge_ShowHint(object sender, EventArgs e)
        {
        }

        private void CellObjectMenu_ShowHint(object sender, EventArgs e)
        {
            selectedPlayerObject?.PrepareHint(PanelHint);
        }

        private void MakePagesBackground()
        {
            //pageFinance.PageImage = MainControlbackground("Finance");
            pageHeroes.PageImage = MainControlbackground("Heroes");
            pageTournament.PageImage = MainControlbackground("Tournament");
            pageTraditions.PageImage = MainControlbackground("Traditions");
            pageSpell.PageImage = MainControlbackground("Spell");

            for (int i = 0; i < Descriptors.CapitalPages.Count; i++)
            {
                pagesCapital[i].PageImage = MainControlbackground(Descriptors.CapitalPages[i].NameTexture);
            }
        }

        private readonly VisualControl panelEmptyInfo;
        internal PanelConstructionInfo panelConstructionInfo { get; private set; }
        internal PanelHeroInfo panelHeroInfo { get; private set; }
        internal PanelMonsterInfo panelMonsterInfo { get; private set; }

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

        private void ShowEvents()
        {
            Debug.Assert(curAppliedPlayer is PlayerHuman);

            PlayerHuman p = (PlayerHuman)curAppliedPlayer;
            int top = 0;

            foreach (VCEvent e in p.ListEvents)
            {
                e.SetParent(panelNotices);
                e.ShiftX = 0;
                e.ShiftY = top;

                top = e.NextTop();
            }

            panelNotices.ArrangeControls();
        }

        private void DrawPageConstructions()
        {
            // Проходим по каждому зданию, создавая ему панель
            VisualControl parent;
            foreach (DescriptorConstruction tck in Descriptors.Constructions)
            {
                if (tck.IsInternalConstruction)
                {
                    parent = pagesCapital[tck.Page.Index].Page;

                    Assert(panels[tck.Page.Index, tck.CoordInPage.Y, tck.CoordInPage.X] == null);

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

        internal void LosesChanged()
        {
            if (lobby.StateLobby == StateLobby.TurnHuman)
                AdjustPanelLoses();
        }

        private void AdjustPanelLoses()
        {
            /*
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
            Program.formMain.SetNeedRedrawFrame();*/
        }

        internal void ListHeroesChanged()
        {
            if (lobby != null)
            {
                Debug.Assert(curAppliedPlayer == lobby.CurrentPlayer);

                panelCombatHeroes.Visible = curAppliedPlayer.CombatHeroes.Count > 0;
                panelCombatHeroes.ApplyList(curAppliedPlayer.CombatHeroes);
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

        internal void SelectPlayerObject(BigEntity po, int selectPage = -1, bool playSoundSelect = false)
        {
            if (selectedPlayerObject != po)
            {
                if (panelEmptyInfo.Visible)
                    panelEmptyInfo.Visible = false;

                if (selectedPlayerObject != null)
                {
                    selectedPlayerObject.HideInfo();
                }

                UpdateBackgroundImage();

                selectedPlayerObject = po;

                if (selectedPlayerObject != null)
                {
                    if (playSoundSelect)
                        selectedPlayerObject.PlaySoundSelect();

                    selectedPlayerObject.ShowInfo();
                }
                else
                    panelEmptyInfo.Visible = true;

                if (selectedPlayerObject != null)
                {
                    cellObjectMenu.Visible = true;
                }
                else
                {
                    cellObjectMenu.ImageIndex = -1;
                    cellObjectMenu.Visible = false;
                }

                UpdateMenu();
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
            pageControl.ActivatePage(pagesCapital[construction.Descriptor.Page.Index]);
            SelectPlayerObject(construction, selectPage);
        }

        internal void UpdateMenu()
        {
            ClearMenu();

            if (selectedPlayerObject != null)
            {
                cellObjectMenu.ImageIndex = selectedPlayerObject.GetImageIndex();
                cellObjectMenu.ImageIsEnabled = selectedPlayerObject.GetNormalImage();
            }

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
                if (pb.Descriptor.IsInternalConstruction && (pb.Descriptor.Category != CategoryConstruction.Temple))
                    pb.Descriptor.Panel.Entity = pb;
            }

            // Показываем героев
            ShowEvents();
            AdjustPanelLoses();
            ListHeroesChanged();
        }

        internal void StartNewLobby()
        {
            Debug.Assert(lobby == null);

            lobby = new Lobby(Descriptors.TypeLobbies[0], Program.formMain.CurrentHumanPlayer.TournamentSettings[0], this, FormMain.Descriptors);

            for (int i = 0; i < panelPlayers.Controls.Count; i++)
            {
                Debug.Assert(panelPlayers.Controls[i] is CellPlayer);
                ((CellPlayer)panelPlayers.Controls[i]).Entity = lobby.Players[i];
            }

            if (Program.formMain.currentLayer != this)
            {
                Program.formMain.PlayerMusic.PlayMusic();
                Program.formMain.ExchangeLayer(Program.formMain.layerMainMenu, this);
            }

            pageControl.ActivatePage(pagesCapital[0]);
            PageControl_PageChanged(null, new EventArgs());
            ShowCurrentPlayerLobby();

            lobby.Start();
            firstFrameOfSecond = DateTime.Now;
            internalTimer = new Stopwatch();
            internalTimer.Start();
        }

        internal override void PrepareFrame()
        {
            base.PrepareFrame();

            if (internalTimer.IsRunning)
            {

                DateTime curTime = DateTime.Now;
                TimeSpan delta1 = curTime - firstFrameOfSecond;
                if (delta1.TotalMilliseconds >= 1000)
                {
                    firstFrameOfSecond = DateTime.Now;
                    framesPerSecond = countFrames;
                    ticksPerSecond = countTicks;
                    countFrames = 0;
                    countTicks = 0;
                }

                countFrames++;
                if (lobby is null)
                    Program.formMain.ExchangeLayer(this, Program.formMain.layerMainMenu);
            }
        }

        internal void RestartLobby()
        {
            Debug.Assert(lobby != null);
            pageControl.ClearSelectedObjects();
            SelectPlayerObject(null);
            lobby.ExitFromLobby();
            lobby = null;

            StartNewLobby();
        }

        internal void EndLobby()
        {
            Debug.Assert(lobby != null);
            pageControl.ClearSelectedObjects();
            SelectPlayerObject(null);
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
                Program.formMain.PlayerMusic.PlayMainTheme();
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
                    while (panelNotices.Controls.Count > 0)
                        panelNotices.RemoveControl(panelNotices.Controls[0]);

                    labelDay.Visible = true;
                    //labelKnowledge.Visible = true;
                    labelTraditions.Visible = true;
                    //labelPeople.Visible = true;
                    labelGreatness.Visible = false;
                    MainControl.Visible = true;
                    ShowDataPlayer();
                }
                else
                {
                    labelDay.Visible = false;
                    //labelKnowledge.Visible = false;
                    labelTraditions.Visible = false;
                    //labelPeople.Visible = false;
                    labelGreatness.Visible = false;
                    MainControl.Visible = false;
                    //foreach (VCImageLose il in listBtnLoses)
                    //    il.Visible = false;

                    ShowNamePlayer(lobby.CurrentPlayer.Descriptor.Name);
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
                AdjustNamePlayer();
            }
        }

        internal void ShowDataPlayer()
        {
            Debug.Assert(lobby.CurrentPlayer.GetTypePlayer() == TypePlayer.Human);

            // Если этого игрока не отрисовывали, формируем заново вкладки
            if (curAppliedPlayer != lobby.CurrentPlayer)
            {
                curAppliedPlayer = lobby.CurrentPlayer;
                //if (curAppliedPlayer.CurrentLocation != null)
                //    pageLocation.PageImage = curAppliedPlayer.CurrentLocation.Settings.TypeLandscape.GetBackgroundImage();
            }

            ShowLobby();

            LosesChanged();
            UpdateListHeroes();
            ShowWarehouse();
            AdjustPageTournament();
            //AdjustNeighborhood();
            //ShowPlayerNotices();
        }

        internal void ShowPlayerNotices()
        {
            if (curAppliedPlayer != null)
            if (curAppliedPlayer.ListNoticesForPlayer.Count > 0)
            {
                panelNotices.Visible = true;
                int nextY = 0;

                foreach (VCCustomNotice ep in curAppliedPlayer.ListNoticesForPlayer)
                {
                    ep.ShiftY = nextY;
                    ep.Visible = true;
                    if ((ep.Parent is null) || (ep.Parent != panelNotices))
                        panelNotices.AddControl(ep);

                    panelNotices.ArrangeControl(ep);
                    nextY = ep.NextTop();
                }

                panelNotices.ApplyMaxSize();
            }
            else
                panelNotices.Visible = false;
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

        private void DrawPageLocation()
        {

            /*
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

            pageLocation.Page.ArrangeControls();*/
        }

        internal Bitmap MainControlbackground(string nameTexture)
        {
            return Program.formMain.CollectionBackgroundImage.GetBitmap(nameTexture, new Size(MainControl.Width, MainControl.Height));
        }

        private void BtnCheating_Click(object sender, EventArgs e)
        {
            WindowCheating w = new WindowCheating(curAppliedPlayer);
            w.Show();
        }

        private void UpdateNameCurrentPage()
        {
            labelCaptionPage.Text = pageControl.CurrentPage.Caption;
        }

        private void PageControl_PageChanged(object sender, EventArgs e)
        {
            if (Program.formMain.currentLayer == this)
            {
                UpdateNameCurrentPage();
                //if (winAdvice is null)
                //    winAdvice = new WindowAdvice();
                //winAdvice.ShowAdvice(pageControl.CurrentPage.Advice);
            }

            UpdateBackgroundImage();
        }

        private void UpdateBackgroundImage()
        {
            MainControl.BackgroundImage = pageControl.CurrentPage.PageImage;
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
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if ((lobby != null) && (lobby.CurrentPlayer != null) && MainControl.Visible)
            {
                labelDay.Text = $"{lobby.Turn}";
                labelTraditions.Text = $"{curAppliedPlayer.PointsForNextTradition}";
                pbTraditions.Max = curAppliedPlayer.PointsForNextTradition;
                pbTraditions.Position = Math.Min((int)curAppliedPlayer.PointsTraditions, curAppliedPlayer.PointsForNextTradition);
                pbTraditions.Text = curAppliedPlayer.NextTradition is null ? "Не выбрана" : "";
                labelGreatness.Text = curAppliedPlayer.LevelGreatness.ToString()
                    + " (+" + curAppliedPlayer.PointGreatnessPerDay().ToString() + ")";
                    //+ ": " + curAppliedPlayer.PointGreatness.ToString() + "/"
                    //+ curAppliedPlayer.PointGreatnessForNextLevel.ToString();

                pageTournament.LowText = lobby.DaysLeftForBattle > 0 ? lobby.DaysLeftForBattle.ToString() + " д." :
                        curAppliedPlayer.SkipBattle ? "Проп." : "Битва";
                //pageTraditions.RestTimeExecuting = curAppliedPlayer.RestTimeForNextTradition >= 0 ? curAppliedPlayer.RestTimeForNextTradition.ToString() : "";
                pageTraditions.Quantity = curAppliedPlayer.ListTraditions.Count;

                /*foreach (VCToolLabelResource l in labelsResources)
                {
                    l.UpdateData(curAppliedPlayer);
                }*/

                ShowPlayerNotices();

                // Показываем страницу традиций
                if (pageControl.CurrentPage == pageTraditions)
                {
                    int i = 0;

                    foreach (KeyValuePair<DescriptorTradition, int> t in curAppliedPlayer.ListTraditions)
                    {
                        listAcceptedTraditions[i].CellTypeTradition.ImageIndex = t.Key.TypeTradition.ImageIndex;
                        listAcceptedTraditions[i].TextName.Text = t.Key.Name;
                        listAcceptedTraditions[i].LblLevel.Text = t.Value.ToString();
                        listAcceptedTraditions[i].Visible = true;

                        i++;
                    }
                }
            }
        }

        private void Pause()
        {
            Assert(internalTimer.IsRunning);

            internalTimer.Stop();
        }

        private void Continue()
        {
            Assert(!internalTimer.IsRunning);

            internalTimer.Start();
        }

        private void ShowInGameMenu()
        {
            WindowMenuInGame w = new WindowMenuInGame(this, CurrentLobby);
            w.Show();
        }

        internal override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);

            if (e.KeyCode == Keys.Escape)
            {
                ShowInGameMenu();
            }
        }

        internal override void ArrangeControls()
        {
            base.ArrangeControls();
        }

        internal override void Deactivated()
        {
            base.Deactivated();

            Pause();
        }

        internal override void Activated()
        {
            base.Activated();

            Continue();
        }

        internal override void ApplyCurrentWindowSize(Size size)
        {
            base.ApplyCurrentWindowSize(size);

            if ((MainControl.Width != size.Width) || (MainControl.Height != size.Height - MainControl.ShiftY))
            {
                MainControl.Width = size.Width;
                MainControl.Height = size.Height - MainControl.ShiftY;

                Adjust2();
                ArrangeControls();
            }
        }

        private void Adjust2()
        {
            bmpPreparedToolbar.Bitmap = PrepareToolbar();
            bmpPreparedToolbar.ShiftY = panelPlayers.NextTop();
            MainControl.ShiftY = bmpPreparedToolbar.NextTop() - Config.GridSize;

            bmpTopPanel.Bitmap = GuiUtils.MakeBackground(new Size(MainControl.Width, bmpPreparedToolbar.ShiftY));
            bmpTopPanel.Width = bmpTopPanel.Bitmap.Width;
            bmpTopPanel.Height = bmpTopPanel.Bitmap.Height;

            labelNamePlayer.Height = bmpPreparedToolbar.Height;
            panelPlayers.ShiftX = (MainControl.Width - panelPlayers.Width) / 2;
            vcRightPanel.Height = MainControl.Height - panelLairWithFlags.NextTop();
            vcRightPanel.ShiftX = MainControl.Width - vcRightPanel.Width;

            panelNotices.Height = vcRightPanel.Height;

            bmpObjectMenu.ShiftX = vcRightPanel.Width - bmpObjectMenu.Width;
            bmpObjectMenu.ShiftY = vcRightPanel.Height - bmpObjectMenu.Height;
            panelCombatHeroes.ShiftX = vcRightPanel.Width - panelCombatHeroes.Width - Config.GridSize;

            int shift0 = MainControl.Width - Config.GridSizeHalf;
            /*foreach (DescriptorBaseResource br in Descriptors.BaseResources)
            {
                labelsResources[br.Number].ShiftX = shift0 - (labelsResources[br.Number].Width + Config.GridSizeHalf) * (Descriptors.BaseResources.Count - br.Number);
            }*/

            panelConstructionInfo.Height = MainControl.Height - panelConstructionInfo.ShiftY - Config.GridSize;
            panelHeroInfo.Height = panelConstructionInfo.Height;
            panelMonsterInfo.Height = panelConstructionInfo.Height;
            panelEmptyInfo.Height = panelConstructionInfo.Height;

            AdjustNamePlayer();
            MakePagesBackground();

            btnInGameMenu.ShiftX = btnInGameMenu.Parent.Width - btnInGameMenu.Width - Config.GridSize;
            btnCheating.ShiftX = btnInGameMenu.ShiftX - btnCheating.Width - Config.GridSize;

            // Выравниваем страницы столицы
            // Мы достоверно знаем, что на страницах столицы 3 промежутка между сооружениями и надо еще 2 по краям по горизонтали
            // По вертикали 2 расстояния
            // Вообще надо переделать на константы из конфиги
            horInterval = (MainControl.Width - panelEmptyInfo.ShiftX - panelEmptyInfo.Width - vcRightPanel.Width - (panels[0, 0, 0].Width * FormMain.Config.ConstructionMaxPos)) / (FormMain.Config.ConstructionMaxPos + 1);
            verInterval = (MainControl.Height - pageHeroes.Page.ShiftY - (panels[0, 0, 0].Height * FormMain.Config.ConstructionMaxLines) - (Config.GridSize * 2)) / (FormMain.Config.ConstructionMaxLines - 1);

            for (int z = 0; z < panels.GetLength(0); z++)
                for (int y = 0; y < panels.GetLength(1); y++)
                    for (int x = 0; x < panels.GetLength(2); x++)
                    {
                        if (panels[z, y, x] != null)
                        {
                            panels[z, y, x].ShiftX = (panels[z, y, x].Width + horInterval) * x;
                            panels[z, y, x].ShiftY = (panels[z, y, x].Height + verInterval) * y;
                        }
                    }

            pageControl.ShiftX = panelEmptyInfo.ShiftX + panelEmptyInfo.Width + horInterval;

            foreach (VCPageButton p in pageControl.Pages)
            {
                p.Page.Width = CalcWidthPage();
            }
        }

        private int CalcWidthPage()
        {
            return MainControl.Width - panelEmptyInfo.ShiftX - panelEmptyInfo.Width - vcRightPanel.Width - (horInterval * 2);
        }

        private void AdjustNamePlayer()
        {
            labelNamePlayer.ShiftX = (bmpPreparedToolbar.Width - labelNamePlayer.Width) / 2;
            bmpPreparedToolbar.ArrangeControl(labelNamePlayer);

        }

        internal override void PreferencesChanged()
        {
            base.PreferencesChanged();

            btnCheating.Visible = Program.formMain.Settings.AllowCheating;
        }
    }
}