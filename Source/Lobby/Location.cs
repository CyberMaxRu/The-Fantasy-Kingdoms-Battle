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
        private int percentScoutedArea;
        private int percentNonScoutedArea;

        private readonly DescriptorCellMenu descScout;
        private readonly CellMenuLocationScout cmScout;
        private readonly DescriptorCellMenu descAddScoutHero;
        private readonly CellMenuLocationAddScoutHero cmAddScoutHero;
        private readonly DescriptorCellMenu descCancelScout;
        private readonly CellMenuLocationCancelScout cmCancelScout;
        private readonly DescriptorCellMenu descReturnFromScout;
        private readonly CellMenuLocationReturn cmReturnFromScout;

        public Location(Player player, TypeLobbyLocationSettings settings) : base(settings, player.Lobby, player)
        {
            Player = player;
            Settings = settings;
            Visible = settings.Visible;
            PercentScoutedArea = settings.PercentScoutedArea;
            Danger = 333;
            ComponentObjectOfMap = new ComponentObjectOfMap(this, settings.Visible);

            // Создание сооружений согласно настройкам
            foreach (TypeLobbyLairSettings ls in settings.LairsSettings)
            {
                Construction c = new Construction(this, ls);
                Lairs.Add(c);

                if (!ls.Visible)
                    c.PercentScoutForFound = player.Lobby.Rnd.Next(ls.MinPercentScout, ls.MaxPercentScout + 1);
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

        internal Player Player { get; }
        internal TypeLobbyLocationSettings Settings { get; }
        internal List<Construction> Lairs { get; } = new List<Construction>();
        internal bool Visible { get; set; }
        internal int PercentScoutedArea// Процент разведанной части локации
        {
            get => percentScoutedArea;
            private set
            { 
                percentScoutedArea = value;
                percentNonScoutedArea = 1000 - percentScoutedArea;

                Debug.Assert(percentScoutedArea <= 1_000, $"percentNonScoutedArea : {percentNonScoutedArea}");
                Debug.Assert(percentNonScoutedArea >= 0, $"percentNonScoutedArea : {percentNonScoutedArea}");
            }
        }
        internal int PercentNonScoutedArea { get => percentNonScoutedArea; }// Процент неразведанной части локации
        internal int PercentScoutAreaToday { get; set; }// Сколько проценто локации будет разведано сегодня
        internal List<Creature> ListCreaturesForScout { get; } = new List<Creature>();// Какие существа разведуют локацию

        internal int Danger { get; private set; }// Процент опасности локации
        internal int StateMenu { get; set; }//
        internal int PayForHire { get; set; }// Сколько было потрачено на найм
        internal List<CellMenuLocationSpell> MenuSpells { get; } = new List<CellMenuLocationSpell>();

        internal override string GetTypeEntity() => "Локация";

        internal override int GetImageIndex()
        {
            return Settings.TypeLandscape.ImageIndex;
        }

        internal override string GetName()
        {
            return Settings.Name;
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

                    foreach (ConstructionSpell cs in Player.ConstructionSpells)
                    {
                        if (cs.DescriptorSpell.TypeEntity == TypeEntity.Location) 
                            if ((!Visible && !cs.DescriptorSpell.Scouted) || (Visible && cs.DescriptorSpell.Scouted))
                            {
                                CellMenuLocationSpell cmcs = SearchCellMenuSpell(cs);

                                if (cmcs is null)
                                {
                                    cmcs = new CellMenuLocationSpell(this, cs);
                                    MenuSpells.Add(cmcs);
                                }
                                //Assert(!menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Used);                        

                                menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Research = cmcs;
                                menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Used = true;
                            }
                    }
                    break;
                case 1:
                    if ((cmPageCreatures is null) || !cmPageCreatures.ChangePage)
                        ShowHeroesInMenu(menu, ComponentObjectOfMap.ListHeroesForFlag, HeroForScoutClick, ModeTextForCreature.Scout);
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
                        ShowHeroesInMenu(menu, Player.FreeHeroes, AddHeroToScout, ModeTextForCreature.Hire);
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

        internal void DoScout(int percentArea)
        {
            Debug.Assert(percentArea > 0);

            PercentScoutedArea += Math.Min(percentNonScoutedArea, percentArea);
        }

        internal void FindScoutedConstructions()
        {
            foreach (Construction c in Lairs)
                if (!c.ComponentObjectOfMap.Visible)
                    if (c.PercentScoutForFound <= PercentScoutedArea)
                        c.Unhide(true);
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

            Debug.Assert(ComponentObjectOfMap.ListHeroesForFlag.IndexOf(cmc.Creature) == -1);
            ComponentObjectOfMap.ListHeroesForFlag.Add(cmc.Creature);
            Player.SetScoutForHero(cmc.Creature, this);

            if (Player.FreeHeroes.Count == 0)
                cmReturnFromScout.Click();
            else
                Program.formMain.layerGame.UpdateMenu();
        }

        internal void DropFlagScout()
        {
            foreach (Creature c in ComponentObjectOfMap.ListHeroesForFlag)
            {
                Debug.Assert(ComponentObjectOfMap.ListHeroesForFlag.IndexOf(c) != -1);
                Player.SetScoutForHero(c, null);
            }

            ComponentObjectOfMap.ListHeroesForFlag.Clear();
        }

        internal override void PlayDefaultSoundSelect()
        {
            if (Settings.TypeLandscape.UriSoundSelect != null)
                Program.formMain.PlaySoundSelect(Settings.TypeLandscape.UriSoundSelect);
        }

        private CellMenuLocationSpell SearchCellMenuSpell(ConstructionSpell spell)
        {
            foreach (CellMenuLocationSpell cs in MenuSpells)
            {
                if (cs.Spell == spell)
                    return cs;
            }

            return null;
        }

        internal void PrepareNewDay()
        {
            foreach (CellMenuLocationSpell cm in MenuSpells)
            {
                cm.PrepareNewDay();
            }
        }

        internal void CalcPercentScoutToday()
        {
            PercentScoutAreaToday = 0;
            foreach (Creature c in ListCreaturesForScout)
                PercentScoutAreaToday += c.PercentLocationForScout;

            if (PercentScoutAreaToday > PercentNonScoutedArea)
                PercentScoutAreaToday = PercentNonScoutedArea;
        }

        internal override string GetIDEntity(DescriptorEntity descriptor) => (descriptor as TypeLobbyLocationSettings).ID + Number.ToString();// Убрать Number.ToString(), когда будет 1 игрок;

        internal Bitmap GetBitmapBackground()
        {
            return Program.formMain.layerGame.MainControlbackground(Settings.Background);
        }
    }
}
