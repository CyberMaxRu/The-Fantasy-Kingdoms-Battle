using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс сооружения у игрока
    internal sealed class Construction : BattleParticipant
    {
        private int gold;

        public Construction(Player p, DescriptorConstruction b, Location location, bool visible, bool own, bool canOwn, bool isEnemy, string pathToLocation) : base(b, p.Lobby)
        {
            Player = p;
            TypeConstruction = b;
            Location = location;
            DaysBuilded = 0;
            PlayerIsOwner = own;
            PlayerCanOwn = canOwn;
            IsEnemy = isEnemy;

            IDPathToLocation = pathToLocation;

            // Настраиваем исследования 
            foreach (DescriptorCellMenu d in TypeConstruction.CellsMenu)
                Researches.Add(CellMenuConstruction.Create(this, d));

            Visible = visible;// && !location.Ownership;

            Level = b.DefaultLevel;
            if (Level > 0)
            {
                AddPerksToPlayer();
                CreateProducts();
            }

            // Убрать эту проверку после настройки всех логов
            if (TypeConstruction.Monsters.Count > 0)
                CreateMonsters();

            p.Constructions.Add(this);
            // Восстановить
            //if (Construction.HasTreasury)
            //    Gold = Construction.GoldByConstruction;

            TuneCellMenuBuildOrUpgrade();
            UpdateSelectedColor();
        }

        public Construction(Player p, DescriptorConstruction l, int level, int x, int y, Location location, bool visible, bool own, bool canOwn, bool isEnemy, TypeNoticeForPlayer typeNotice) : base(l, p.Lobby)
        {
            Player = p;
            TypeConstruction = l;
            X = x;
            Y = y;
            Location = location;
            Visible = visible;
            DaysBuilded = 0;
            PlayerIsOwner = own;
            PlayerCanOwn = canOwn;
            IsEnemy = isEnemy;

            Debug.Assert((TypeConstruction.Category == CategoryConstruction.Lair) || (TypeConstruction.Category == CategoryConstruction.External) || (TypeConstruction.Category == CategoryConstruction.Temple)
                || (TypeConstruction.Category == CategoryConstruction.Place) || (TypeConstruction.Category == CategoryConstruction.BasePlace) || (TypeConstruction.Category == CategoryConstruction.ElementLandscape));

            Debug.Assert(level <= 1);
            if (level == 1)
            {
                Build(false);
                if (TypeConstruction.Levels[1].GetCreating() != null)
                    DaysBuilded = TypeConstruction.Levels[1].GetCreating().DaysProcessing;
                else
                    DaysBuilded = 0;
            }

            // Настраиваем исследования 
            foreach (DescriptorCellMenu d in TypeConstruction.CellsMenu)
                Researches.Add(CellMenuConstruction.Create(this, d));

            // Убрать эту проверку после настройки всех логов
            if (TypeConstruction.Monsters.Count > 0)
                CreateMonsters();

            p.Constructions.Add(this);

            if (typeNotice != TypeNoticeForPlayer.None)
                Player.AddNoticeForPlayer(this, typeNotice);

            TuneCellMenuBuildOrUpgrade();
            UpdateSelectedColor();
        }

        public Construction(Location l, TypeLobbyLairSettings ls) : base(ls.TypeLair, l.Lobby)
        {
            Player = l.Player;
            TypeConstruction = ls.TypeLair;
            Location = l;
            DaysBuilded = 0;
            PlayerIsOwner = ls.Own;
            PlayerCanOwn = ls.CanOwn;
            IsEnemy = ls.IsEnemy;
            Visible = ls.Visible;// && !location.Ownership;
            IDPathToLocation = ls.PathToLocation;

            // Настраиваем исследования 
            foreach (DescriptorCellMenu d in TypeConstruction.CellsMenu)
                Researches.Add(CellMenuConstruction.Create(this, d));

            Level = ls.TypeLair.DefaultLevel;
            if (Level > 0)
            {
                AddPerksToPlayer();
                CreateProducts();
            }

            // Убрать эту проверку после настройки всех логов
            if (TypeConstruction.Monsters.Count > 0)
                CreateMonsters();

            Player.Constructions.Add(this);

            if (ls.Resources != null)
            {
                BaseMiningResources = new ListBaseResources(ls.Resources);
                CurrentMiningResources = new ListBaseResources();
                UpdateCurrentIncomeResources();
            }

            // Восстановить
            //if (Construction.HasTreasury)
            //    Gold = Construction.GoldByConstruction;

            TuneCellMenuBuildOrUpgrade();
            UpdateSelectedColor();
        }

        internal DescriptorConstruction TypeConstruction { get; }
        internal bool PlayerIsOwner { get; private set; }// Игрок - владелец сооружения
        internal bool PlayerCanOwn { get; private set; }// Игрок может владеть сооружением
        internal bool IsEnemy { get; private set; }// Это сооружение враждебно
        internal int Level { get; private set; }
        internal int DaysBuilded { get; private set; }// Сколько дней строится сооружение
        internal int Gold { get => gold; set { Debug.Assert(TypeConstruction.HasTreasury); gold = value; } }// Казна гильдии
        internal List<Hero> Heroes { get; } = new List<Hero>();
        internal Player Player { get; }

        // Свойства для внешних сооружений
        internal Location Location { get; set; }// Локация, на которой находится сооружение
        internal int X { get; set; }// Позиция по X в слое
        internal int Y { get; set; }// Позиция по Y в слое
        internal bool Visible { get; private set; }// Сооружение видимо игроку
        internal int PercentScoutForFound { get; set; }// Процент разведки локации, чтобы найти сооружение
        internal Color SelectedColor { get; private set; }// Цвет рамки при выделении
        internal string IDPathToLocation { get; }//
        internal Location NextLocation { get; private set; }// Дескриптор пути в другую локацию

        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня
        internal bool Destroyed { get; private set; } = false;// Логово уничтожено, работа с ним запрещена

        // Поддержка флага
        internal TypeFlag TypeFlag { get; private set; } = TypeFlag.None;// Тип установленного флага
        internal int DaySetFlag { get; private set; }// День установки флага
        internal ListBaseResources SpendedGoldForSetFlag { get; private set; }// Сколько ресурсов было потрачено на установку флага
        internal PriorityExecution PriorityFlag { get; private set; } = PriorityExecution.None;// Приоритет разведки/атаки
        internal List<Hero> listAttackedHero { get; } = new List<Hero>();// Список героев, откликнувшихся на флаг

        // Small-сущности в сооружении
        internal List<EntityForConstruction> ListEntities { get; } = new List<EntityForConstruction>();// Все сущности в сооружении
        internal ConstructionVisitSimple CurrentVisit { get; private set; }// Текущее активное посещение сооружения
        internal ConstructionEvent CurrentMassEvent { get; set; }// Текущее мероприятие
        internal ConstructionTournament CurrentTournament { get; set; }// Текущий турнир
        internal List<ConstructionVisit> Visits { get; } = new List<ConstructionVisit>();//
        internal List<ConstructionExtension> Extensions { get; } = new List<ConstructionExtension>();// Дополнения
        internal List<ConstructionImprovement> Improvements { get; } = new List<ConstructionImprovement>();// Улучшения
        internal List<ConstructionResource> Resources { get; } = new List<ConstructionResource>();// Ресурсы
        internal List<ConstructionService> Services { get; } = new List<ConstructionService>();// Услуги, доступные в строении
        internal List<ConstructionProduct> Goods { get; } = new List<ConstructionProduct>();// Товары, доступные в строении
        internal List<ConstructionAbility> Abilities { get; } = new List<ConstructionAbility>();// Умения, доступные в строении
        internal List<ConstructionSpell> Spells { get; } = new List<ConstructionSpell>();// Заклинания, доступные в строении
        internal CellMenuConstructionLevelUp CellMenuBuildOrLevelUp { get; private set; }// Действие для постройки/улучшения сооружения
        internal int[] SatisfactionNeeds { get; private set; }// Удовлетворяемые потребности
        internal List<CellMenuConstructionSpell> MenuSpells { get; } = new List<CellMenuConstructionSpell>();

        internal List<CellMenuConstruction> ListQueueProcessing { get; } = new List<CellMenuConstruction>();// Очередь обработки ячеек меню

        // 
        internal ListBaseResources BaseMiningResources { get; }// Базовые значения добываемых ресурсов
        internal ListBaseResources CurrentMiningResources { get; }// Текущие значения добываемых ресурсов
        internal bool MiningBaseResources { get; private set; }// Сооружение добывает ресурсы

        private void TuneCellMenuBuildOrUpgrade()
        {
            CellMenuBuildOrLevelUp = null;

            // Сооружение не построено, ищем действие для постройки
            foreach (CellMenuConstruction cm in Researches)
            {
                if ((cm is CellMenuConstructionLevelUp cml) && (cml.Descriptor.Number == Level + 1))
                {
                    CellMenuBuildOrLevelUp = cml;
                    break;
                }
            }
        }

        private void UpdateCurrentIncomeResources()
        {
            MiningBaseResources = TypeConstruction.Levels[Level].Mining != null;
            if (MiningBaseResources)
            {
                for (int i = 0; i < BaseMiningResources.Count; i++)
                    CurrentMiningResources[i].Quantity = Convert.ToInt32(BaseMiningResources[i].Quantity * TypeConstruction.Levels[Level].Mining[i] / 10);
            }
        }

        internal void Build(bool needNotice)
        {
            if ((TypeConstruction.Category != CategoryConstruction.Lair) && (TypeConstruction.Category != CategoryConstruction.ElementLandscape))
            {
                Debug.Assert(Level < TypeConstruction.MaxLevel);
                //Debug.Assert(CheckRequirements());
                //Debug.Assert(Player.BaseResources.ResourcesEnough(CostBuyOrUpgrade()));

                Player.Constructed(this);

                if (Level > 0)
                {
                    // Убираем перки от сооружения
                    foreach (DescriptorPerk dp in TypeConstruction.Levels[Level].ListPerks)
                        Player.RemovePerkFromConstruction(this, dp);

                    // Убираем товар посещения
                    if (TypeConstruction.Levels[Level].DescriptorVisit != null)
                    {
                        RemoveProduct(TypeConstruction.Levels[Level].DescriptorVisit);
                    }
                }
            }

            Level++;

            if (Level == 1)
            {
                ValidateHeroes();
                //PrepareTurn();
            }

            //
            CreateProducts();

            if ((TypeConstruction.Category != CategoryConstruction.Lair) && (TypeConstruction.Category != CategoryConstruction.ElementLandscape))
            {
                // Убираем операцию постройки из меню
                CellMenuConstruction cmBuild = null;
                foreach (CellMenuConstruction cm in Researches)
                {
                    if (cm is CellMenuConstructionLevelUp cml)
                        if (cml.Descriptor.Number == Level)
                        {
                            cmBuild = cml;
                            break;
                        }
                }

                if (cmBuild != null)
                {
                    Researches.Remove(cmBuild);
                    Lobby.Layer.UpdateMenu();
                }
            }

            // Обновляем список перков от сооружения
            AddPerksToPlayer();

            // Добавляем товар посещения
            Debug.Assert(TypeConstruction.Levels[Level].DescriptorVisit != null);
            ConstructionVisitSimple cpVisit = new ConstructionVisitSimple(this, TypeConstruction.Levels[Level].DescriptorVisit);
            CurrentVisit = cpVisit;
            AddVisit(cpVisit);

            // Инициализируем удовлетворяемые потребности
            SatisfactionNeeds = new int[FormMain.Descriptors.NeedsCreature.Count];
            if (TypeConstruction.Levels[Level].DescriptorVisit != null)
            {
                foreach ((DescriptorNeed, int) need in TypeConstruction.Levels[Level].DescriptorVisit.ListNeeds)
                {
                    SatisfactionNeeds[need.Item1.Index] = need.Item2;
                }
            }

            //
            Properties = new EntityProperties(this, TypeConstruction.Levels[Level].Properties);
            if (TypeConstruction.Levels[Level].Properties != null)
            {
                MainPerk = new Perk(this, TypeConstruction.Levels[Level].Properties);
                Perks.Add(MainPerk);
            }
            
            UpdateCurrentIncomeResources();// Настраиваем добычу базовых ресурсов

            Initialize();

            if (needNotice)
                Player.AddNoticeForPlayer(this, Level == 1 ? TypeNoticeForPlayer.Build : TypeNoticeForPlayer.LevelUp);

            TuneCellMenuBuildOrUpgrade();
        }

        private void CreateProducts()
        {
            foreach (DescriptorSmallEntity se in TypeConstruction.Levels[Level].Extensions)
            {
                if (se is DescriptorConstructionExtension dce)
                    AddExtension(new ConstructionExtension(this, dce));
                else if (se is DescriptorResource dr)
                    AddResource(new ConstructionResource(this, dr)); 
                //else if (se is DescriptorItem di)
                //    AddProduct(new ConstructionProduct(this, di));
                else
                    throw new Exception($"Неизвестный товар: {se.ID}");
            }
        }

        internal int GetInterest()
        {
            return CurrentVisit != null ? CurrentVisit.Interest : 0;
        }

        internal void AddPerksToPlayer()
        {
            foreach (DescriptorPerk dp in TypeConstruction.Levels[Level].ListPerks)
                Player.AddPerkFromConstruction(this, dp);

            Player.RecalcPerksHeroes();
        }

        internal void ValidateHeroes()
        {
            // Восстановить
            /*if ((Construction.TrainedHero != null) && (Construction.TrainedHero.Cost == 0))
            {
                if (Heroes.Count() < MaxHeroes())
                {
                    for (; Heroes.Count() < MaxHeroes();)
                    {
                        HireHero();
                    }
                }
            }*/
        }

        internal override void MakeMenu(VCMenuCell[,] menu)
        {
            // Рисуем содержимое ячеек
            if (Visible)
            {
                Debug.Assert(TypeConstruction != null);

                ValidateResearches();
                FillResearches(menu);
            }
            else
            {
                foreach (ConstructionSpell cs in Player.ConstructionSpells)
                {
                    if (!cs.DescriptorSpell.Scouted)
                    {
                        CellMenuConstructionSpell cmcs = SearchCellMenuSpell(cs);

                        if (cmcs is null)
                        {
                            cmcs = new CellMenuConstructionSpell(this, cs);
                            MenuSpells.Add(cmcs);
                        }
                        //Assert(!menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Used);                        

                        menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Research = cmcs;
                        menu[cs.DescriptorSpell.Coord.Y, cs.DescriptorSpell.Coord.X].Used = true;
                    }
                }
            }

            CellMenuConstructionSpell SearchCellMenuSpell(ConstructionSpell spell)
            {
                foreach (CellMenuConstructionSpell cs in MenuSpells)
                {
                    if (cs.Spell == spell)
                        return cs;
                }

                return null;
            }
        }

        internal void ValidateResearches()
        {
            Debug.Assert(Researches != null);

            /*List<ConstructionCellMenu> forRemove = new List<ConstructionCellMenu>();

            foreach (ConstructionCellMenu mc in Researches)
            {
                if (mc.Research.TypeConstruction != null)
                    if (mc.ConstructionForBuild != null)
                    {
                        if (mc.ConstructionForBuild.Level> 0)
                        {
                            forRemove.Add(mc);
                        }
                    }
            }

            foreach (ConstructionCellMenu mc in forRemove)
            {
                Researches.Remove(mc);
            }*/
        }

        internal bool CanLevelUp()
        {
            return Level < TypeConstruction.MaxLevel;
        }

        internal ListBaseResources CostBuyOrUpgrade()
        {
            return CanLevelUp() == true ? TypeConstruction.Levels[Level + 1].GetCreating().CostResources : null;
        }

        internal bool CheckLevelRequirements(int level)
        {
            // При постройке храма из меню Святой земли, сюда прилетает 2 уровень
            if (TypeConstruction.MaxLevel < level)
                return false;

            // Сначала проверяем наличие золота
            if (!Player.CheckRequiredResources(TypeConstruction.Levels[level].GetCreating().CostResources))
                return false;

            // Проверяем наличие очков строительства
            if (!Player.CheckRequireBuilders(TypeConstruction.Levels[level].GetCreating().Builders))
                return false;

            // Проверяем, что нет события или турнира
            if (CurrentMassEvent != null)
                return false;
            if (CurrentTournament != null)
                return false;

            // Проверяем требования к зданиям
            return Player.CheckRequirements(TypeConstruction.Levels[level].GetCreating().Requirements);
        }

        internal List<TextRequirement> GetTextRequirements(int level)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            Player.TextRequirements(TypeConstruction.Levels[level].GetCreating().Requirements, list);

            if (CurrentMassEvent != null)
                list.Add(new TextRequirement(false, "В сооружении идет мероприятие"));

            if (CurrentTournament != null)
                list.Add(new TextRequirement(false, "В сооружении идет турнир"));

            return list;
        }

        internal int Income()
        {
            return 0;// Level > 0 ? TypeConstruction.Levels[Level].Income : 0;
        }

        internal bool DoIncome()
        {
            return false;// TypeConstruction.Levels[1].Income > 0;
        }

        internal int IncomeForLevel(int level)
        {
            return 0;// TypeConstruction.Levels[level].Income;
        }

        internal int DayBuildingForLevel(int level)
        {
            return TypeConstruction.Levels[level].GetCreating().DaysProcessing;
        }

        internal int GreatnesAddForLevel(int level)
        {
            return TypeConstruction.Levels[level].GreatnessByConstruction;
        }

        internal int GreatnesPerDayForLevel(int level)
        {
            return TypeConstruction.Levels[level].GreatnessPerDay;
        }

        internal int BuildersPerDayForLevel(int level)
        {
            return TypeConstruction.Levels[level].BuildersPerDay;
        }

        internal int IncomeNextLevel()
        {
            return Level < TypeConstruction.MaxLevel ? IncomeForLevel(Level + 1) : 0;
        }

        internal int GreatnessPerDay()
        {
            return Level > 0 ? TypeConstruction.Levels[Level].GreatnessPerDay : 0;
        }

        internal int BuildersPerDay()
        {
            return Level > 0 ? TypeConstruction.Levels[Level].BuildersPerDay : 0;
        }

        internal int GreatnessAddNextLevel()
        {
            return Level < TypeConstruction.MaxLevel ? GreatnesAddForLevel(Level + 1) : 0;
        }

        internal int GreatnessPerDayNextLevel()
        {
            return Level < TypeConstruction.MaxLevel ? GreatnesPerDayForLevel(Level + 1) : 0;
        }

        internal int BuildersPerDayNextLevel()
        {
            return Level < TypeConstruction.MaxLevel ? BuildersPerDayForLevel(Level + 1) : 0;
        }

        internal List<TextRequirement> GetTextRequirementsHire()
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Level == 0)
                list.Add(new TextRequirement(false, TypeConstruction.GetTextConstructionNotBuilded()));

            if ((Level > 0) && (Heroes.Count == MaxHeroes()))
                list.Add(new TextRequirement(false, TypeConstruction.GetTextConstructionIsFull()));

            if (MaxHeroesAtPlayer())
                list.Add(new TextRequirement(false, "Достигнуто максимальное количество героев в королевстве"));

            return list;
        }

        internal Hero HireHero(DescriptorCreature th, ListBaseResources cost)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= TypeConstruction.TrainedHero.Cost);

            Hero h = new Hero(this, Player, th);

            if (th.CategoryCreature != CategoryCreature.Citizen)
            {
                if (gold > 0)
                {
                    Player.SpendResource(cost);
                    if (Player.Descriptor.TypePlayer == TypePlayer.Human)
                        Program.formMain.SetNeedRedrawFrame();
                }
            }

            AddHero(h);

            return h;
        }

        internal void AddHero(Hero ph)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);

            Heroes.Add(ph);
            Player.AddHero(ph);
        }

        internal bool MaxHeroesAtPlayer()
        {
            return Player.CombatHeroes.Count == Player.Lobby.TypeLobby.MaxHeroes;
        }

        internal int MaxHeroes()
        {
            return Level > 0 ? TypeConstruction.Levels[Level].MaxInhabitant : 0;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            Debug.Assert(!Destroyed);

            if (Player == Player.Lobby.CurrentPlayer)
            {

                if (TypeConstruction.IsOurConstruction)
                {
                    panelHint.AddStep2Header(GetName(), TypeConstruction.ImageIndex);
                    panelHint.AddStep3Type(TypeConstruction.TypeConstruction.Name);
                    if (!((Level == 1) && (TypeConstruction.MaxLevel == 1)))
                        panelHint.AddStep4Level(Level > 0 ? "Уровень " + Level.ToString(): "");
                    panelHint.AddStep5Description(TypeConstruction.Description + ((Level > 0) && (Heroes.Count > 0) ? Environment.NewLine + Environment.NewLine
                        + (Heroes.Count > 0 ? "Героев: " + Heroes.Count.ToString() + "/" + MaxHeroes().ToString() : "") : ""));
                    panelHint.AddStep6Income(Income());
                    panelHint.AddStep8Greatness(0, GreatnessPerDay());
                    panelHint.AddStep9PlusBuilders(BuildersPerDay());
                    panelHint.AddStep9Interest(GetInterest(), false);
                    panelHint.AddStep9ListNeeds(SatisfactionNeeds);
                }
                else
                {
                    if (!Visible)
                    {
                        panelHint.AddStep2Header("Неизвестное место");
                        panelHint.AddStep4Level("Место не разведано");
                        panelHint.AddStep5Description("Установите флаг разведки для отправки героев к месту");
                    }
                    else
                    {
                        panelHint.AddStep2Header(GetName(), TypeConstruction.ImageIndex);
                        panelHint.AddStep3Type(TypeConstruction.TypeConstruction.Name);
                        panelHint.AddStep5Description(TypeConstruction.Description);

                        if (TypeConstruction.Reward != null)
                        {
                            panelHint.AddStep7Reward(TypeConstruction.Reward.Cost.ValueGold());
                            panelHint.AddStep8Greatness(TypeConstruction.Reward.Greatness, 0);
                        }
                    }
                }
            }
        }

        internal override void HideInfo()
        {
            base.HideInfo();

            Debug.Assert(!Destroyed);

            Lobby.Layer.panelConstructionInfo.Visible = false;
            Lobby.Layer.panelLairInfo.Visible = false;
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Debug.Assert(!Destroyed);

            if (TypeConstruction.IsOurConstruction)
            {
                Lobby.Layer.panelConstructionInfo.Visible = true;
                Lobby.Layer.panelConstructionInfo.Entity = this;
                if (selectPage >= 0)
                    Lobby.Layer.panelConstructionInfo.SelectPage(selectPage);
            }
            else
            {
                Lobby.Layer.panelConstructionInfo.Visible = true;
                Lobby.Layer.panelConstructionInfo.Entity = this;
//                Program.formMain.panelLairInfo.Visible = true;
//                Program.formMain.panelLairInfo.Entity = this;
            }

        }

        internal void PrepareTurn()
        {
            if (Level > 0)
            {
                Initialize();

                if (Lobby.Turn > 1)
                {
                    if (TypeConstruction.Levels[Level].GreatnessPerDay > 0)
                        Player.AddGreatness(GreatnessPerDay());
                }

                if (CurrentMassEvent != null)
                {
                    CurrentMassEvent.Counter--;
                    if (CurrentMassEvent.Counter == 0)
                        RemoveProduct(CurrentMassEvent.Descriptor);
                }

                if (CurrentTournament != null)
                {
                    CurrentTournament.Counter--;
                    if (CurrentTournament.Counter == 0)
                        RemoveProduct(CurrentTournament.Descriptor);
                }


                foreach (CellMenuConstruction cm in Researches)
                {
                    cm.PrepareTurn();
                }

                foreach (CellMenuConstructionSpell cm in MenuSpells)
                {
                    cm.PrepareTurn();
                }

                if (TypeConstruction.ID != FormMain.Config.IDCityGraveyard)
                {
                    foreach (Hero h in Heroes)
                    {
                        h.PrepareTurn();
                    }
                }
            }

            if (ListQueueProcessing.Count > 0)
            {
                CellMenuConstruction cm = ListQueueProcessing[0];
                Debug.Assert(cm.DaysLeft > 0);

                cm.DaysProcessed++;
                cm.DaysLeft--;

                if (cm.DaysLeft == 0)
                {
                    cm.Execute();

                    RemoveEntityFromQueueProcessing(cm);
                }
            }
        }

        internal void PrepareQueueShopping(List<UnitOfQueueForBuy> queue)
        {
            Debug.Assert(Level > 0);

            foreach (Hero h in Heroes)
            {
                if (h.IsLive)
                    h.PrepareQueueShopping(queue);
            }
        }

        internal List<TextRequirement> GetResearchTextRequirements(CellMenuConstruction research)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (TypeConstruction.IsInternalConstruction)
            {
                // Если нет требований, то по умолчанию остается только одно - сооружение должно быть построено
                // Если есть, то не надо писать, что сооружение не построено - иначе не видно, какие там требования
                if (Level == 0)
                    list.Add(new TextRequirement(false, "Сооружение не построено"));

                Player.TextRequirements(research.Descriptor.CreatedEntity.GetCreating().Requirements, list);
            }

            return list;
        }

        internal override int GetQuantity()
        {
            return 0;
        }

        private void CreateMonsters()
        {
            AssertNotDestroyed();
            //Debug.Assert(TypeLair.Monsters.Count > 0);

            Monster lm;
            foreach (DescriptorConstructionLevelLair mll in TypeConstruction.Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new Monster(mll.Monster, mll.Level, this);
                    Monsters.Add(lm);
                    AddCombatHero(lm);
                }
            }
        }

        internal ListBaseResources CostScout()
        {
            Debug.Assert(!Visible);
            AssertNotDestroyed();

            return new ListBaseResources(0);
           //return PriorityFlag < PriorityExecution.Exclusive ?
           //     new ListBaseResources(Location.Settings.CostScout * Player.Lobby.TypeLobby.CoefFlagScout[(int)PriorityFlag + 1] / 100) : new ListBaseResources();
        }

        private void AssertNotHidden()
        {
            Debug.Assert(Visible, $"Логово {TypeConstruction.ID} игрока {Player.GetName()} скрыто.");
        }

        internal void AssertNotDestroyed()
        {
            Debug.Assert(!Destroyed, $"Логово {TypeConstruction.ID} игрока {Player.GetName()} уничтожено.");
        }

        internal ListBaseResources CostAttack()
        {
            AssertNotHidden();
            AssertNotDestroyed();

            return new ListBaseResources(0);
            //return PriorityFlag < PriorityExecution.Exclusive ?
            //    new ListBaseResources(Location.Settings.CostAttack * Player.Lobby.TypeLobby.CoefFlagAttack[(int)PriorityFlag + 1] / 100) : new ListBaseResources();
        }

        internal ListBaseResources CostDefense()
        {
            AssertNotHidden();
            AssertNotDestroyed();

            return new ListBaseResources(0);
            //return PriorityFlag < PriorityExecution.Exclusive ?
            //    new ListBaseResources(Location.Settings.CostDefense * Player.Lobby.TypeLobby.CoefFlagDefense[(int)PriorityFlag + 1] / 100) : new ListBaseResources();
        }

        internal string NameLair()
        {
            AssertNotDestroyed();
            return Visible ? GetName() : "Неизвестное место";
        }

        internal int ImageIndexLair()
        {
            AssertNotDestroyed();

            return Visible ? TypeConstruction.ImageIndex : FormMain.IMAGE_INDEX_UNKNOWN;
        }

        internal bool ImageEnabled()
        {
            return (Level > 0) || (TypeConstruction.MaxLevel == 0);
        }

        internal Color GetColorCaption()
        {
            if (PriorityFlag == PriorityExecution.None)
                return Visible ? Color.MediumAquamarine : FormMain.Config.ColorMapObjectCaption(false);

            switch (TypeAction())
            {
                case TypeFlag.Scout:
                    return Color.LimeGreen;
                case TypeFlag.Attack:
                    return Color.OrangeRed;
                case TypeFlag.Defense:
                    return Color.DodgerBlue;
                default:
                    throw new Exception($"Неизвестный тип действия: {TypeAction()}");
            }
        }

        internal ListBaseResources RequiredGold()
        {
            AssertNotDestroyed();

            switch (TypeAction())
            {
                case TypeFlag.Scout:
                    return CostScout();
                case TypeFlag.Attack:
                    return CostAttack();
                case TypeFlag.Defense:
                    return CostDefense();
                default:
                    throw new Exception($"Неизвестный тип действия: {TypeAction()}");
            }
        }

        internal bool CheckFlagRequirements()
        {
            AssertNotDestroyed();

            if (!Player.CheckRequiredResources(RequiredGold()))
                return false;

            switch (PriorityFlag)
            {
                case PriorityExecution.None:
                    return Player.QuantityFlags[PriorityExecution.None] > 0;
                case PriorityExecution.Normal:
                    return true;
                case PriorityExecution.Warning:
                    return Player.QuantityFlags[PriorityExecution.High] <= 1;
                case PriorityExecution.High:
                    return Player.QuantityFlags[PriorityExecution.Exclusive] == 0;
                case PriorityExecution.Exclusive:
                    return false;
                default:
                    throw new Exception("Неизвестный приоритет.");
            }
        }

        internal List<TextRequirement> GetRequirements()
        {
            AssertNotDestroyed();

            List<TextRequirement> list = new List<TextRequirement>();

            switch (PriorityFlag)
            {
                case PriorityExecution.None:
                    if (!Player.ExistsFreeFlag())
                        list.Add(new TextRequirement(false, "Нет свободных флагов"));
                    break;
                case PriorityExecution.Normal:
                    break;
                case PriorityExecution.Warning:
                    if (Player.QuantityFlags[PriorityExecution.High] >= 2)
                        list.Add(new TextRequirement(false, "Флагов с высоким приоритетом может быть не более двух"));
                    break;
                case PriorityExecution.High:
                    if (Player.QuantityFlags[PriorityExecution.Exclusive] >= 1)
                        list.Add(new TextRequirement(false, "Флагов с максимальным приоритетом может быть не более одного"));
                    break;
                case PriorityExecution.Exclusive:
                    break;
                default:
                    throw new Exception("Неизвестный приоритет.");
            }

            return list;
        }

        internal TypeFlag TypeAction()
        {
            AssertNotDestroyed();

            if (!Visible)
                return TypeFlag.Scout;
            if (TypeConstruction.Category == CategoryConstruction.Lair)
                return TypeFlag.Attack;
            if (TypeConstruction.Category == CategoryConstruction.External)
                return TypeFlag.Defense;
            if (TypeConstruction.ID == FormMain.Config.IDConstructionCastle)
                return TypeFlag.Battle;
            return TypeFlag.None;
        }

        internal void AttackToCastle()
        {
            Debug.Assert(TypeConstruction.ID == FormMain.Config.IDConstructionCastle);
            TypeFlag = TypeFlag.Battle;
            DaySetFlag = Player.Lobby.Turn;
        }

        internal void IncPriority()
        {
            Debug.Assert(PriorityFlag < PriorityExecution.Exclusive);
            AssertNotDestroyed();

            // 

            ListBaseResources cost = RequiredGold();// На всякий случай запоминаем точное значение. вдруг потом при трате что-нибудь поменяется
            Player.SpendResource(cost);
            SpendedGoldForSetFlag.AddResources(cost);

            if (DaySetFlag == 0)
            {
                Debug.Assert(TypeFlag == TypeFlag.None);
                TypeFlag = TypeAction();
                DaySetFlag = Player.Lobby.Turn;
            }
            else
            {
                Debug.Assert(TypeFlag == TypeAction());
            }

            PriorityFlag++;
            if (PriorityFlag == PriorityExecution.Normal)
                Player.AddFlag(this);
            else
                Player.UpPriorityFlag(this);

            Lobby.Layer.LairsWithFlagChanged();
        }

        internal ListBaseResources Cashback()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag.ExistsResources());
            Debug.Assert(DaySetFlag > 0);
            Debug.Assert(TypeFlag != TypeFlag.None);
            AssertNotDestroyed();

            return DaySetFlag == Player.Lobby.Turn ? SpendedGoldForSetFlag : new ListBaseResources();
        }

        internal void CancelFlag()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag.ExistsResources());
            Debug.Assert(DaySetFlag > 0);
            Debug.Assert(TypeFlag != TypeFlag.None);
            AssertNotDestroyed();

            Player.ReturnResource(Cashback());
            DropFlag();
        }

        internal string PriorityFlatToText()
        {
            AssertNotDestroyed();

            switch (PriorityFlag)
            {
                case PriorityExecution.None:
                    return "Отсутствует";
                case PriorityExecution.Normal:
                    return "Обычный";
                case PriorityExecution.Warning:
                    return "Повышенный";
                case PriorityExecution.High:
                    return "Высокий";
                case PriorityExecution.Exclusive:
                    return "Максимальный";
                default:
                    throw new Exception("Неизвестный приоритет: " + PriorityFlag.ToString());
            }
        }

        internal int MaxHeroesForFlag()
        {
            switch (TypeAction())
            {
                case TypeFlag.Scout:
                    return Player.Lobby.TypeLobby.MaxHeroesForScoutFlag;
                case TypeFlag.Attack:
                case TypeFlag.Defense:
                case TypeFlag.Battle:
                    return Player.Lobby.TypeLobby.MaxHeroesForBattle;
                default:
                    throw new Exception($"Неизвестный тип действия: {TypeAction()}");
            }
        }

        internal void AddAttackingHero(Hero ph)
        {
            Debug.Assert(ph != null);
            Debug.Assert(listAttackedHero.IndexOf(ph) == -1);
            Debug.Assert(ph.StateCreature.ID == NameStateCreature.Nothing.ToString());
            Debug.Assert(ph.TargetByFlag == null);
            AssertNotDestroyed();
            Debug.Assert(listAttackedHero.Count < MaxHeroesForFlag());

            listAttackedHero.Add(ph);
            ph.TargetByFlag = this;
            ph.SetState(ph.StateForFlag(TypeFlag));
        }

        internal void RemoveAttackingHero(Hero ph)
        {
            Debug.Assert(listAttackedHero.IndexOf(ph) != -1);
            Debug.Assert(ph.TargetByFlag == this);
            AssertNotDestroyed();

            ph.TargetByFlag = null;
            listAttackedHero.Remove(ph);
            ph.SetState(NameStateCreature.Nothing);
        }

        private void DropFlag()
        {
            Debug.Assert(TypeFlag != TypeFlag.None);
            AssertNotDestroyed();

            Player.RemoveFlag(this);

            TypeFlag = TypeFlag.None;
            SpendedGoldForSetFlag = null;
            DaySetFlag = 0;
            TypeFlag = TypeFlag.None;
            PriorityFlag = PriorityExecution.None;

            while (listAttackedHero.Count > 0)
                RemoveAttackingHero(listAttackedHero[0]);

            Lobby.Layer.LairsWithFlagChanged();
        }


        internal void Unhide(bool needNotice)
        {
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Guild);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Economic);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Temple);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Military);
            //Debug.Assert(TypeConstruction.Category != CategoryConstruction.External);
            Debug.Assert(!Visible);
            Debug.Assert(TypeFlag == TypeFlag.None);
            Debug.Assert(!Destroyed);

            Visible = true;
            if (needNotice)
                Player.AddNoticeForPlayer(this, TypeNoticeForPlayer.Explore);

            if (NextLocation != null)
            {
                NextLocation.Visible = true;

                if (needNotice)
                    Player.AddNoticeForPlayer(NextLocation, TypeNoticeForPlayer.FoundLocation);
            }
        }

        // Место разведано
        internal void DoScout()
        {
            Debug.Assert(!Visible);
            Debug.Assert(TypeFlag == TypeFlag.Scout);
            AssertNotDestroyed();

            Visible = true;

            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            HandOutGoldHeroes();

            DropFlag();
        }

        // Сооружение уничтожено 
        internal void Destroy()
        {
            AssertNotDestroyed();

            // Убираем себя из списка логов игрока
            Player.RemoveLair(this);

            // Если сооружение было выбрано, очищаем ссылку
            Lobby.Layer.ObjectDestroyed(this);
            Destroyed = true;
        }

        // Логово захвачено
        internal void DoCapture()
        {
            AssertNotHidden();
            Debug.Assert(TypeFlag == TypeFlag.Attack);
            Debug.Assert(listAttackedHero.Count > 0);

            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            HandOutGoldHeroes();

            DropFlag();

            Player.ApplyReward(this);
            Destroy();

            // Ставим тип места, который должен быть после зачистки
            Debug.Assert(!(TypeConstruction.TypePlaceForConstruct is null));

            Construction pl = new Construction(Player, TypeConstruction.TypePlaceForConstruct, TypeConstruction.DefaultLevel, X, Y, Location, true, true, true, false, TypeNoticeForPlayer.None);
            pl.Visible = true;
            Location.Lairs.Add(pl);
        }

        internal void DoDefense()
        {
            AssertNotHidden();
            AssertNotDestroyed();
            Debug.Assert(TypeFlag == TypeFlag.Defense);
            Debug.Assert(listAttackedHero.Count > 0);

            // Раздаем награду
            HandOutGoldHeroes();

            DropFlag();

            // Убираем себя из списка логов игрока
            Player.RemoveLair(this);

            Destroyed = true;
        }

        internal void MonsterIsDead(Monster m)
        {
            Debug.Assert(m != null);
            Debug.Assert(m.BattleParticipant == this);
            Debug.Assert(Monsters.IndexOf(m) != -1);

            m.SetIsDead(FormMain.Descriptors.ReasonOfDeathInBattle);
            CombatHeroes.Remove(m);
            Monsters.Remove(m);

            if (Lobby.Layer.PlayerObjectIsSelected(m))
                Lobby.Layer.SelectPlayerObject(null);
        }

        // Раздаем деньги за флаг героям
        private void HandOutGoldHeroes()
        {
            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            if (listAttackedHero.Count > 0)
            {
                Debug.Assert(SpendedGoldForSetFlag != null);

                // Определяем, по сколько денег достается каждому герою
                int goldPerHero = SpendedGoldForSetFlag.ValueGold() / listAttackedHero.Count;
                int delta = SpendedGoldForSetFlag.ValueGold() - goldPerHero * listAttackedHero.Count;
                Debug.Assert(goldPerHero * listAttackedHero.Count + delta == SpendedGoldForSetFlag.ValueGold());

                foreach (Hero h in listAttackedHero)
                    h.AddGold(goldPerHero);

                // Остаток отдаем первому герою
                if (delta > 0)
                    listAttackedHero[0].AddGold(delta);
            }
        }

        internal string ListMonstersForHint()
        {
            if (Visible)
            {
                if (Monsters.Count == 0)
                    return "Нет существ";

                string list = "";
                int pos = 1;
                foreach (Monster m in Monsters)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{pos}. {m.TypeCreature.Name} ({m.Level})";
                    pos++;
                }

                return list;
            }
            else
                return "Пока место не разведано, существа в нем неизвестны";
        }

        internal string ListHeroesForHint()
        {
            if (listAttackedHero.Count == 0)
                return "Нет героев";
            else
            {
                string list = "";
                int pos = 1;
                foreach (Hero h in listAttackedHero)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{pos}. {h.GetNameHero()} ({h.Level})";
                    pos++;
                }

                return list;
            }
        }

        internal void PrepareHintForBuildOrUpgrade(PanelHint panelHint, int requiredLevel)
        {
            if (requiredLevel > TypeConstruction.MaxLevel)
                return;// Убрать это
            Debug.Assert(requiredLevel > 0);
            Debug.Assert(requiredLevel <= TypeConstruction.MaxLevel);

            panelHint.AddStep2Header(TypeConstruction.Name, TypeConstruction.ImageIndex);
            panelHint.AddStep3Type(TypeConstruction.TypeConstruction.Name);
            panelHint.AddStep4Level(requiredLevel == 1 ? "Уровень 1" : $"Улучшить строение ({requiredLevel} ур.)");
            panelHint.AddStep5Description(requiredLevel == 1 ? TypeConstruction.Description : "");
            panelHint.AddStep6Income(IncomeForLevel(requiredLevel));
            panelHint.AddStep8Greatness(GreatnesAddForLevel(requiredLevel), GreatnesPerDayForLevel(requiredLevel));
            panelHint.AddStep9PlusBuilders(BuildersPerDayForLevel(requiredLevel));
            if (TypeConstruction.Levels[requiredLevel].DescriptorVisit != null)
            {
                panelHint.AddStep9Interest(TypeConstruction.Levels[requiredLevel].DescriptorVisit.Interest, false);
                panelHint.AddStep9ListNeeds(TypeConstruction.Levels[requiredLevel].DescriptorVisit.ListNeeds, false);
            }
            panelHint.AddStep10DaysBuilding(-1, DayBuildingForLevel(requiredLevel));
            panelHint.AddStep11Requirement(GetTextRequirements(requiredLevel));
            panelHint.AddStep12Gold(Player.BaseResources, TypeConstruction.Levels[requiredLevel].GetCreating().CostResources);
            panelHint.AddStep13Builders(TypeConstruction.Levels[requiredLevel].GetCreating().Builders, Player.FreeBuilders >= TypeConstruction.Levels[requiredLevel].GetCreating().Builders);
        }

        internal void PrepareHintForInhabitantCreatures(PanelHint panelHint)
        {
            if (Heroes.Count > 0)
            {

                string list = "";
                int pos = 1;
                foreach (Hero h in Heroes)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{pos}. {h.GetNameHero()} ({h.Level})";
                    pos++;
                }

                panelHint.AddStep2Header(TypeConstruction.IsOurConstruction ? "Жители" : "Существа");
                panelHint.AddStep5Description(list);
            }
            else
                panelHint.AddSimpleHint("Обитателей нет");
        }

        internal override int GetImageIndex()
        {
            if (NextLocation != null)
                return NextLocation.GetImageIndex();
            else if ((Player.Lobby.CurrentPlayer is null) || (Player == Player.Lobby.CurrentPlayer))
                return ImageIndexLair();
            else
                return FormMain.Config.Gui48_Battle;
        }

        internal override string GetText() => "";

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override string GetLevel()
        {
            AssertNotDestroyed();

            return !Visible ? "" : Level == 0 ? "" : (Level == 1) && (TypeConstruction.MaxLevel == 1) ? "" : Level < TypeConstruction.MaxLevel ? $"{Level}/{TypeConstruction.MaxLevel}" : Level.ToString();
        }

        internal override void Click(VCCell pe)
        {
            base.Click(pe);
            Lobby.Layer.SelectPlayerObject(this);
        }

        internal override string GetName()
        {
            if (NextLocation is null)
                return TypeConstruction.Name;
            else
                return "Путь в " + NextLocation.Settings.Name;
        }

        internal override TypePlayer GetTypePlayer()
        {
            return TypePlayer.Lair;
        }

        internal override Player GetPlayer()
        {
            return Player;
        }

        internal override int GetImageIndexAvatar()
        {
            return TypeConstruction.ImageIndex;
        }

        internal List<ConstructionProduct> GetProducts(DescriptorCreature dc)
        {
            List<ConstructionProduct> list = new List<ConstructionProduct>();

            foreach (ConstructionProduct cp in ListEntities)
            {
                if (cp.IsAvailableForCreature(dc))
                {
                    list.Add(cp);
                }
            }

            return list;
        }

        internal Ability PurchaseAbility(Creature creature, ConstructionAbility ca)
        {
            Debug.Assert(Abilities.IndexOf(ca) >= 0);

            Ability a = new Ability(creature, ca.DescriptorAbility);
            return a;
        }

        private void AddEntity(EntityForConstruction entity)
        {
            foreach (EntityForConstruction i in ListEntities)
            {
                Debug.Assert(i.Descriptor.ID != entity.Descriptor.ID);
            }

            ListEntities.Add(entity);
        }

        internal void AddVisit(ConstructionVisit cv)
        {
            AddEntity(cv);
            Visits.Add(cv);
        }

        internal void AddExtension(ConstructionExtension extension)
        {
            AddEntity(extension);

            // Прибавляем ее удовлетворение потребностей к текущим
            Extensions.Add(extension);

            foreach ((DescriptorNeed, int) need in extension.Descriptor.ListNeeds)
            {
                ChangeNeed(need.Item1, need.Item2);
            }

            if (CurrentVisit != null)
                UpdateInterestMainVisit();
        }

        internal void AddImprovement(ConstructionImprovement improvement)
        {
            AddEntity(improvement);

            // Прибавляем ее удовлетворение потребностей к текущим
            Improvements.Add(improvement);
        }

        internal void AddResource(ConstructionResource resource)
        {
            AddEntity(resource);
            Resources.Add(resource);
        }

        internal void AddAbility(ConstructionAbility ca)
        {
            AddEntity(ca);
            Abilities.Add(ca);
        }

        internal void AddSpell(ConstructionSpell cs)
        {
            AddEntity(cs);
            Spells.Add(cs);
            Player.ConstructionSpells.Add(cs);
        }

        internal void AddMassEvent(ConstructionEvent ce)
        {
            AddEntity(ce);

            Debug.Assert(CurrentMassEvent is null);
            Debug.Assert(CurrentTournament is null);

            Visits.Add(ce);
        }

        internal void AddService(ConstructionService cs)
        {
            AddEntity(cs);
            Services.Add(cs);
        }

        internal void AddProduct(ConstructionProduct cp)
        {
            AddEntity(cp);

            if ((cp.DescriptorItem != null) || (cp.DescriptorGroupItem != null))
            {
                Goods.Add(cp);
            }
        }

        internal void RemoveProduct(DescriptorSmallEntity de)
        {
            EntityForConstruction productFromRemove = null;

            foreach (EntityForConstruction cp in ListEntities)
            {
                if (cp.Descriptor.ID == de.ID)
                {
                    productFromRemove = cp;
                    break;
                }
            }

            Debug.Assert(productFromRemove != null);

            if (CurrentVisit == productFromRemove)
                CurrentVisit = null;
            if (CurrentMassEvent == productFromRemove)
                CurrentMassEvent = null;
            if (CurrentTournament == productFromRemove)
                CurrentTournament = null;

            RemoveEntity(productFromRemove);
        }

        internal void RemoveEntity(EntityForConstruction entity)
        {
            Debug.Assert(entity != null);

            if (!ListEntities.Remove(entity))
                Debug.Fail($"Не смог удалить сущность {entity.Descriptor.ID} из сооружения {TypeConstruction.ID}");

            if (entity is ConstructionExtension ce)
            {
                if (!Extensions.Remove(ce))
                    Debug.Fail($"Не смог удалить доп. сооружение {entity.Descriptor.ID} из сооружения {TypeConstruction.ID}");
            }
            else if (entity is ConstructionResource cr)
            {
                if (!Resources.Remove(cr))
                    Debug.Fail($"Не смог удалить ресурс {entity.Descriptor.ID} из сооружения {TypeConstruction.ID}");
            }
            else if (entity is ConstructionEvent cev)
            {
                Debug.Assert(CurrentMassEvent != null);
                Debug.Assert(CurrentMassEvent == cev);
                CurrentMassEvent = null;
            }
            else if (entity is ConstructionTournament ct)
            {
                Debug.Assert(CurrentTournament != null);
                Debug.Assert(CurrentTournament == ct);
                CurrentTournament = null;
            }
            else if (entity is ConstructionAbility ca)
            {
                if (!Abilities.Remove(ca))
                    Debug.Fail($"Не смог удалить умение {entity.Descriptor.ID} из сооружения {TypeConstruction.ID}");
            }
            else if (entity is ConstructionSpell csp)
            {
                if (!Spells.Remove(csp))
                    Debug.Fail($"Не смог удалить заклинание {entity.Descriptor.ID} из сооружения {TypeConstruction.ID}");

                if (!Player.ConstructionSpells.Remove(csp))
                    Debug.Fail($"Не смог удалить заклинание {entity.Descriptor.ID} у игрока");
            }
            else if (entity is ConstructionService cs)
            {
                if (!Services.Remove(cs))
                    Debug.Fail($"Не смог удалить услугу {entity.Descriptor.ID} из сооружения {TypeConstruction.ID}");
            }
            else if (entity is ConstructionProduct cp)
            {
                Goods.Remove(cp);
            }
            else if (entity is ConstructionVisit cv)
            {
                Visits.Remove(cv);
            }
            else
                throw new Exception($"Неизвестная сущность {entity.Descriptor.ID}.");
        }

        internal void UpdateInterestMainVisit()
        {
            CurrentVisit.Interest = CurrentVisit.DescriptorConstructionVisit.Interest;

            foreach (ConstructionExtension cp in Extensions)
                CurrentVisit.Interest += cp.Descriptor.ModifyInterest;
        }

        private void ChangeNeed(DescriptorNeed need, int value)
        {
            SatisfactionNeeds[need.Index] += value;
        }

        internal bool GoodsExists(DescriptorItem item)
        {
            foreach (CellMenuConstruction cm in Researches)
            {
                if (cm is CellMenuConstructionResearch cmr)
                    if (cmr.Entity.ID == item.ID)
                        return true;
            }

            return false;
        }

        internal bool GoodsAvailabled(DescriptorProduct item)
        {
            foreach (ConstructionProduct cp in Goods)
            {
                if (cp.Descriptor.ID == item.ID)
                    return true;
            }

            return false;
        }

        internal bool ExtensionAvailabled(DescriptorConstructionExtension extension)
        {
            foreach (ConstructionExtension cp in Extensions)
            {
                if (cp.Descriptor.ID == extension.ID)
                    return true;
            }

            return false;
        }

        internal string HintDescriptionInterest()
        {
            Debug.Assert(Level > 0);

            if (GetInterest() == 0)
                return "";

            string text = "Сооружение: " + Utils.DecIntegerBy10(TypeConstruction.Levels[Level].DescriptorVisit.Interest, false);

            foreach (ConstructionExtension cp in Extensions)
            {
                if (cp.Descriptor.ModifyInterest > 0)
                    text += Environment.NewLine + cp.Descriptor.Name + ": " + Utils.DecIntegerBy10(cp.Descriptor.ModifyInterest, true);
            }

            return text;
        }

        internal void AddEntityToQueueProcessing(CellMenuConstruction cell)
        {
            Debug.Assert(ListQueueProcessing.IndexOf(cell) == -1);
            Debug.Assert(cell.DaysProcessed == 0);
            Debug.Assert(cell.PosInQueue == 0);
            Debug.Assert(cell.PurchaseValue is null);
            Debug.Assert(Player.FreeBuilders >= cell.Descriptor.CreatedEntity.GetCreating().Builders);

            cell.PurchaseValue = new ListBaseResources(cell.GetCost());
            Player.SpendResource(cell.PurchaseValue);
            Player.UseFreeBuilder(cell.Descriptor.CreatedEntity.GetCreating().Builders);
            ListQueueProcessing.Add(cell);
            //Player.AddEntityToQueueBuilding()
            cell.PosInQueue = ListQueueProcessing.Count;
        }

        internal void RemoveEntityFromQueueProcessing(CellMenuConstruction cell)
        {
            Debug.Assert(ListQueueProcessing.IndexOf(cell) != -1);
            Debug.Assert((cell.DaysLeft == 0) || (cell.DaysProcessed == 0));
            Debug.Assert(cell.PosInQueue > 0);
            Debug.Assert(cell.PurchaseValue != null);

            cell.PosInQueue = 0;
            Player.ReturnResource(cell.PurchaseValue);
            Player.UnuseFreeBuilders(cell.Descriptor.CreatedEntity.GetCreating().Builders);
            cell.PurchaseValue = null;
            ListQueueProcessing.Remove(cell);

            for (int i = 0; i < ListQueueProcessing.Count; i++)
            {
                ListQueueProcessing[i].PosInQueue = i + 1;
            }
        }

        private void UpdateSelectedColor()
        {
            if (PlayerIsOwner)
                SelectedColor = Color.White;
            else if (PlayerCanOwn)
                SelectedColor = Color.LimeGreen;
            else if (!IsEnemy)
                SelectedColor = Color.Yellow;
            else
                SelectedColor = Color.Red;
        }

        internal void ChangeGold(int gold)
        {
            Gold += gold;

            Debug.Assert(Gold >= 0);
        }

        internal int CalcTax(int gold)
        {
            Debug.Assert(gold > 0);
            Debug.Assert(Level > 0);

            return gold * TypeConstruction.Levels[Level].Tax / 100;
        }

        internal void TuneLinks()
        {
            if (IDPathToLocation.Length > 0)
            {
                foreach (Location l in Player.Locations)
                {
                    if (IDPathToLocation == l.Settings.ID)
                    {
                        NextLocation = l;
                        break;
                    }
                }

                Debug.Assert(NextLocation != null);
            }
        }

        internal override Color GetSelectedColor() => SelectedColor;
    }
}