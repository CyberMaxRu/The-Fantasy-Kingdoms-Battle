using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class BigEntity : Entity
    {
        private List<CellMenuCreature> listCellMenuCreatures;
        private List<Creature> listCreature;
        private EventHandler creatureEventHandler;
        private ModeTextForCreature modeTextForCreature;
        private static Dictionary<string, int> dictNextNumber;

        protected CellMenuCreaturePage cmPageCreatures;

        public BigEntity(DescriptorEntity descriptor, Lobby lobby, Player player) : base()
        {
            Descriptor = descriptor;
            Lobby = lobby;
            Player = player;

            if (!dictNextNumber.ContainsKey(descriptor.ID))
                dictNextNumber.Add(descriptor.ID, 0);

            Number = ++dictNextNumber[descriptor.ID];
            IDEntity = GetIDEntity(Descriptor);
            lobby.AddEntity(this);
        }

        internal int Number { get; }// Последовательный номер сущностиы
        internal string IDEntity { get; }// Уникальное имя сущности в пределах миссии
        internal DescriptorEntity Descriptor { get; }
        internal Player Player { get; }
        internal Lobby Lobby { get; }
        internal bool Destroyed { get; set; } = false;// Сущность уничтожена, работа с ней запрещена
        internal List<CellMenuConstruction> Researches { get; } = new List<CellMenuConstruction>();

        internal Perk MainPerk { get; set; }// Основной перк существа 
        internal List<Perk> Perks { get; } = new List<Perk>();// Перки
        internal EntityProperties Properties { get; set; }// Характеристики
        internal ComponentObjectOfMap ComponentObjectOfMap { get; set; }

        internal virtual string GetIDEntity(DescriptorEntity descriptor) => Descriptor.ID + Number.ToString();

        internal abstract void ShowInfo(int selectPage = -1);

        internal void AssertNotDestroyed()
        {
            //Debug.Assert(!Destroyed, $"Сущность {Descriptor.ID} уничтожена.");
        }

        internal virtual void PlayDefaultSoundSelect()
        {
            if (Descriptor.UriSoundSelect != null)
                Program.formMain.PlaySoundSelect(Descriptor.UriSoundSelect);
        }

        internal virtual void PlaySoundSelect()
        {
            PlayDefaultSoundSelect();
        }

        internal virtual void HideInfo()
        {
            listCreature = null;

            if (listCellMenuCreatures != null)
                foreach (CellMenuCreature cmc in listCellMenuCreatures)
                    cmc.Creature = null;
        }

        internal virtual void MakeMenu(VCMenuCell[,] menu)
        {
            if (listCreature != null)
                UpdateListCreatures(menu, listCreature);
        }

        protected void FillResearches(VCMenuCell[,] menu)
        {
            foreach (CellMenuConstruction pr in Researches)
            {
                if (!menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Used)
                {
                    menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research = pr;
                    menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Used = true;
                }
                else if (((CellMenuConstruction)menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research).Construction == pr.Construction)
                    menu[pr.Descriptor.Coord.Y, pr.Descriptor.Coord.X].Research = pr;
            }
        }

        internal virtual void PerksChanged()
        {
            CalcProperties();
        }

        protected void CalcProperties()
        {
            if (Properties != null)
                for (int i = 0; i < Properties.Count; i++)
                    if (Properties[i] != null)
                        CalcProperty(Properties[i]);
        }

        protected void CalcProperty(CreatureProperty cp)
        {
            cp.ListSource.Clear();
            cp.Value = 0;
            int value;

            foreach (Perk p in Perks)
            {
                value = p.ListProperty[cp.Property.Index];
                if (value != 0)
                {
                    cp.ListSource.Add(p);
                    cp.Value += value;
                }
            }

            if (cp.Value > FormMain.Config.MaxValueProperty)
                cp.Value = FormMain.Config.MaxValueProperty;
            else if (cp.Value < -FormMain.Config.MaxValueProperty)
                cp.Value = -FormMain.Config.MaxValueProperty;
        }

        internal virtual void Initialize()
        {

            // 
            PerksChanged();
        }

        internal void ShowHeroesInMenu(VCMenuCell[,] menu, List<Creature> list, EventHandler onClick, ModeTextForCreature modeTextForCreature)
        {
            // Создаем действия меню
            if (listCellMenuCreatures is null)
            {
                listCellMenuCreatures = new List<CellMenuCreature>();
                DescriptorCellMenu dcm;
                for (int y = 0; y < menu.GetLength(0) - 1; y++)
                    for (int x = 0; x < menu.GetLength(1); x++)
                    {
                        dcm = new DescriptorCellMenu(new Point(x, y));
                        listCellMenuCreatures.Add(new CellMenuCreature(this, dcm));
                    }

                dcm = new DescriptorCellMenu(new Point(0, menu.GetLength(0) - 1));
                cmPageCreatures = new CellMenuCreaturePage(this, dcm);
            }

            cmPageCreatures.SetQuantity(list.Count);
            listCreature = list;
            creatureEventHandler = onClick;
            this.modeTextForCreature = modeTextForCreature;

            UpdateListCreatures(menu, list);
        }

        internal void StopShowHeroesInMenu()
        {
            if (listCellMenuCreatures != null)
            {
                listCreature = null;
                creatureEventHandler = null;
                foreach (CellMenuCreature c in listCellMenuCreatures)
                {
                    c.Creature = null;
                    c.OnClick = null;
                }
            }
        }

        private void UpdateListCreatures(VCMenuCell[,] menu, List<Creature> list)
        {
            Debug.Assert(creatureEventHandler != null);

            int shift = cmPageCreatures.CurrentPage * listCellMenuCreatures.Count;
            for (int i = 0; i < listCellMenuCreatures.Count; i++)
            {
                if (i + shift >= list.Count)
                    break;
                listCellMenuCreatures[i].Creature = list[i + shift];
                listCellMenuCreatures[i].ModeText = modeTextForCreature;
                Debug.Assert(listCellMenuCreatures[i].Creature != null);
                listCellMenuCreatures[i].OnClick = creatureEventHandler;
                menu[listCellMenuCreatures[i].Descriptor.Coord.Y, listCellMenuCreatures[i].Descriptor.Coord.X].Research = listCellMenuCreatures[i];
                menu[listCellMenuCreatures[i].Descriptor.Coord.Y, listCellMenuCreatures[i].Descriptor.Coord.X].Used = true;
            }

            menu[cmPageCreatures.Descriptor.Coord.Y, cmPageCreatures.Descriptor.Coord.X].Research = cmPageCreatures;
            menu[cmPageCreatures.Descriptor.Coord.Y, cmPageCreatures.Descriptor.Coord.X].Used = true;
        }

        internal void EntityAssert(bool condition, string text)
        {
            Utils.Assert(condition, $"{IDEntity}.{Environment.NewLine}{text}");
        }

        internal void EntityDoException(string text)
        {
            Utils.DoException($"{IDEntity}.{Environment.NewLine}{text}");
        }

        internal static void ResetNumerate()
        {
            dictNextNumber = new Dictionary<string, int>();
        }
    }
}
