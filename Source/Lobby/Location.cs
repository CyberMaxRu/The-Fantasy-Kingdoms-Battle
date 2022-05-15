using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс локации
    internal sealed class Location : BigEntity
    {
        private readonly DescriptorCellMenu descScout;
        private readonly CellMenuLocationScout cmScout;
        private readonly DescriptorCellMenu descAddScoutHero;
        private readonly CellMenuLocationAddScoutHero cmAddScoutHero;
        private readonly DescriptorCellMenu descCancelScout;
        private readonly CellMenuLocationCancelScout cmCancelScout;
        private readonly DescriptorCellMenu descReturnFromScout;
        private readonly CellMenuLocationReturn cmReturnFromScout;

        public Location(Player player, TypeLobbyLocationSettings settings) : base(player.Descriptor, player.Lobby)
        {
            Player = player;
            Settings = settings;
            Visible = settings.VisibleByDefault;
            ScoutedArea = settings.ScoutedArea;
            Danger = 333;

            UpdatePercentScoutedArea();

            // Создание сооружений согласно настройкам
            foreach (TypeLobbyLairSettings ls in settings.LairsSettings)
            {
                Construction c = new Construction(player, ls.TypeLair, this, ls.Visible, ls.Own, ls.CanOwn, ls.IsEnemy);
                Lairs.Add(c);
            }

            // Создание меню
            descScout = new DescriptorCellMenu(new Point(0, 0));
            cmScout = new CellMenuLocationScout(this, descScout);
            descAddScoutHero = new DescriptorCellMenu(new Point(1, 2));
            cmAddScoutHero = new CellMenuLocationAddScoutHero(this, descAddScoutHero);
            descCancelScout = new DescriptorCellMenu(new Point(2, 2));
            cmCancelScout = new CellMenuLocationCancelScout(this, descCancelScout);
            descReturnFromScout = new DescriptorCellMenu(new Point(3, 2));
            cmReturnFromScout = new CellMenuLocationReturn(this, descReturnFromScout);

            // Создание рандомных логов монстров согласно настроек типа лобби
            // Для этого сначала создаем логова по минимальному списку, а оставшиеся ячейки - из оставшихся по максимуму
            /*int idxCell;
            int idxTypeLair;

            List<DescriptorConstruction> lairs = new List<DescriptorConstruction>();
            lairs.AddRange(Player.Lobby.Lairs[settings.Coord.Y, settings.Coord.X]);
            List<Point> cells = GetCells();
            Debug.Assert(cells.Count <= lairs.Count);

            while (cells.Count > 0)
            {
                // Берем случайную ячейку
                idxCell = Player.Lobby.Rnd.Next(cells.Count);
                // Берем случайное логово
                idxTypeLair = Player.Lobby.Rnd.Next(lairs.Count);

                // Помещаем в нее логово
                Debug.Assert(Lairs[cells[idxCell].Y, cells[idxCell].X] == null);

                Lairs[cells[idxCell].Y, cells[idxCell].X] = new Construction(Player, lairs[idxTypeLair], lairs[idxTypeLair].DefaultLevel, cells[idxCell].X, cells[idxCell].Y, this, TypeNoticeForPlayer.None);

                cells.RemoveAt(idxCell);// Убираем ячейку из списка доступных
                lairs.RemoveAt(idxTypeLair);// Убираем тип логова из списка доступных
            }

            List<Point> GetCells()
            {
                List<Point> l = new List<Point>();
                for (int y = 0; y < Player.Lobby.TypeLobby.LairsHeight; y++)
                    for (int x = 0; x < Player.Lobby.TypeLobby.LairsWidth; x++)
                        l.Add(new Point(x, y));

                return l;
            }*/
        }
        public Location(Player player) : base(player.Descriptor, player.Lobby)
        {
            Player = player;
            Visible = false;
        }

        internal Player Player { get; }
        internal TypeLobbyLocationSettings Settings { get; }
        internal List<Construction> Lairs { get; } = new List<Construction>();
        internal bool Visible { get; set; }
        internal int ScoutedArea { get; private set; }// Сколько площади локации разведано
        internal int PercentScoutedArea { get; private set; }// Процент разведанной территории
        internal int Danger { get; private set; }// Процент опасности локации
        internal int StateMenu { get; set; }//
        internal List<Creature> HeroesForScout { get; } = new List<Creature>();

        internal override int GetImageIndex()
        {
            return Settings.TypeLandscape.ImageIndex;
        }

        internal override bool GetNormalImage() => true;

        internal override void MakeMenu(VCMenuCell[,] menu)
        {
            switch (StateMenu)
            {
                case 0:
                    StopShowHeroesInMenu();
                    menu[cmScout.Descriptor.Coord.Y, cmScout.Descriptor.Coord.X].Research = cmScout;
                    menu[cmScout.Descriptor.Coord.Y, cmScout.Descriptor.Coord.X].Used = true;
                    break;
                case 1:
                    if ((cmPageCreatures is null) || !cmPageCreatures.ChangePage)
                        ShowHeroesInMenu(menu, HeroesForScout, HeroForScoutClick);
                    cmPageCreatures.ChangePage = false;

                    menu[cmAddScoutHero.Descriptor.Coord.Y, cmAddScoutHero.Descriptor.Coord.X].Research = cmAddScoutHero;
                    menu[cmAddScoutHero.Descriptor.Coord.Y, cmAddScoutHero.Descriptor.Coord.X].Used = true;
                    menu[cmCancelScout.Descriptor.Coord.Y, cmCancelScout.Descriptor.Coord.X].Research = cmCancelScout;
                    menu[cmCancelScout.Descriptor.Coord.Y, cmCancelScout.Descriptor.Coord.X].Used = true;
                    menu[cmReturnFromScout.Descriptor.Coord.Y, cmReturnFromScout.Descriptor.Coord.X].Research = cmReturnFromScout;
                    menu[cmReturnFromScout.Descriptor.Coord.Y, cmReturnFromScout.Descriptor.Coord.X].Used = true;

                    break;
                case 2:
                    if ((cmPageCreatures is null) || !cmPageCreatures.ChangePage)
                        ShowHeroesInMenu(menu, Player.FreeHeroes, AddHeroToScout);
                    cmPageCreatures.ChangePage = false;

                    menu[cmReturnFromScout.Descriptor.Coord.Y, cmReturnFromScout.Descriptor.Coord.X].Research = cmReturnFromScout;
                    menu[cmReturnFromScout.Descriptor.Coord.Y, cmReturnFromScout.Descriptor.Coord.X].Used = true;

                    break;
            }

            base.MakeMenu(menu);
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Lobby.Layer.panelLocationInfo.Entity = this;
            Lobby.Layer.panelLocationInfo.Visible = true;
        }

        internal override void HideInfo()
        {
            base.HideInfo();

            StateMenu = 0;
            Lobby.Layer.panelLocationInfo.Visible = false;
        }

        internal void DoScout(int area)
        {
            Debug.Assert(area > 0);

            ScoutedArea += area;
            if (ScoutedArea > Settings.Area)
                ScoutedArea = Settings.Area;

            UpdatePercentScoutedArea();
        }

        private void UpdatePercentScoutedArea()
        {
            PercentScoutedArea = ScoutedArea / Settings.Area * 1000;
        }        

        private void HeroForScoutClick(object sender, EventArgs e)
        {
            CellMenuCreature cmc = sender as CellMenuCreature;
            Debug.Assert(cmc != null);
            Debug.Assert(cmc.Creature != null);

            Program.formMain.layerGame.SelectPlayerObject(cmc.Creature);
        }

        private void AddHeroToScout(object sender, EventArgs e)
        {
            CellMenuCreature cmc = sender as CellMenuCreature;

            Debug.Assert(HeroesForScout.IndexOf(cmc.Creature) == -1);
            HeroesForScout.Add(cmc.Creature);
            Player.SetScoutForHero(cmc.Creature, this);

            if (Player.FreeHeroes.Count == 0)
                cmReturnFromScout.Click();
            else
                Program.formMain.layerGame.UpdateMenu();
        }
    }
}
