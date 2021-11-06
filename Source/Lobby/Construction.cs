using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс сооружения у игрока
    internal sealed class Construction : BattleParticipant
    {
        private int gold;

        public Construction(Player p, DescriptorConstruction b, Location location) : base(p.Lobby)
        {
            Player = p;
            TypeConstruction = b;
            Location = location;
            DaysBuilded = 0;

            // Настраиваем исследования 
            foreach (DescriptorCellMenuForConstruction d in TypeConstruction.ListResearches)
                Researches.Add(ConstructionCellMenu.Create(this, d));

            Hidden = !TypeConstruction.IsInternalConstruction && !location.Ownership;

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
        }

        public Construction(Player p, DescriptorConstruction l, int level, int x, int y, Location location, TypeNoticeForPlayer typeNotice) : base(p.Lobby)
        {
            Player = p;
            TypeConstruction = l;
            X = x;
            Y = y;
            Location = location;
            Hidden = !location.Ownership;
            DaysBuilded = 0;

            Debug.Assert((TypeConstruction.Category == CategoryConstruction.Lair) || (TypeConstruction.Category == CategoryConstruction.External)
                || (TypeConstruction.Category == CategoryConstruction.Place) || (TypeConstruction.Category == CategoryConstruction.BasePlace) || (TypeConstruction.Category == CategoryConstruction.ElementLandscape));

            Debug.Assert(level <= 1);
            if (level == 1)
            {
                Build(false);
                DaysBuilded = TypeConstruction.Levels[1].DaysProcessing;
            }

            // Настраиваем исследования 
            foreach (DescriptorCellMenuForConstruction d in TypeConstruction.ListResearches)
                Researches.Add(ConstructionCellMenu.Create(this, d));

            // Убрать эту проверку после настройки всех логов
            if (TypeConstruction.Monsters.Count > 0)
                CreateMonsters();

            p.Constructions.Add(this);

            if (typeNotice != TypeNoticeForPlayer.None)
                Player.AddNoticeForPlayer(this, typeNotice);
        }

        internal DescriptorConstruction TypeConstruction { get; }
        internal int Level { get; private set; }
        internal int DaysBuilded { get; private set; }// Сколько дней строится сооружение
        internal int Gold { get => gold; set { Debug.Assert(TypeConstruction.HasTreasury); gold = value; } }
        internal List<Hero> Heroes { get; } = new List<Hero>();
        internal Player Player { get; }

        // Свойства для внешних сооружений
        internal Location Location { get; set; }// Локация, на которой находится сооружение
        internal int X { get; set; }// Позиция по X в слое
        internal int Y { get; set; }// Позиция по Y в слое
        internal bool Hidden { get; private set; }// Логово не разведано

        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня
        internal bool Destroyed { get; private set; } = false;// Логово уничтожено, работа с ним запрещена

        // Поддержка флага
        internal TypeFlag TypeFlag { get; private set; } = TypeFlag.None;// Тип установленного флага
        internal int DaySetFlag { get; private set; }// День установки флага
        internal int SpendedGoldForSetFlag { get; private set; }// Сколько золота было потрачено на установку флага
        internal PriorityExecution PriorityFlag { get; private set; } = PriorityExecution.None;// Приоритет разведки/атаки
        internal List<Hero> listAttackedHero { get; } = new List<Hero>();// Список героев, откликнувшихся на флаг

        // 
        internal List<ConstructionCellMenu> ListQueueProcessing { get; } = new List<ConstructionCellMenu>();// Очередь обработки ячеек меню
        internal List<ConstructionProduct> AllProducts { get; } = new List<ConstructionProduct>();// Все сущности в сооружении
        internal List<ConstructionProduct> Visits { get; } = new List<ConstructionProduct>();// Посещения, события, турниры
        internal List<ConstructionProduct> Extensions { get; } = new List<ConstructionProduct>();// Дополнения
        internal List<ConstructionProduct> Resources { get; } = new List<ConstructionProduct>();// Ресурсы
        internal List<ConstructionProduct> Goods { get; } = new List<ConstructionProduct>();// Товары, доступные в строении
        internal List<ConstructionProduct> Abilities { get; } = new List<ConstructionProduct>();// Умения, доступные в строении
        internal ConstructionProduct MainVisit { get; private set; }// Основное посещение сооружения
        internal ConstructionProduct CurrentVisit { get; private set; }// Текущее активное посещение сооружения
        internal int[] SatisfactionNeeds { get; private set; }// Удовлетворяемые потребности

        internal void Build(bool needNotice)
        {
            if ((TypeConstruction.Category != CategoryConstruction.Lair) && (TypeConstruction.Category != CategoryConstruction.ElementLandscape))
            {
                Debug.Assert(Level < TypeConstruction.MaxLevel);
                Debug.Assert(CheckRequirements());
                Debug.Assert(Player.Gold >= CostBuyOrUpgrade());

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
                ConstructionCellMenu cmBuild = null;
                foreach (ConstructionCellMenu cm in Researches)
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
                    Program.formMain.UpdateMenu();
                }
            }

            // Обновляем список перков от сооружения
            AddPerksToPlayer();

            // Добавляем товар посещения
            if (TypeConstruction.Levels[Level].DescriptorVisit != null)
            {
                ConstructionProduct cpVisit = new ConstructionProduct(this, TypeConstruction.Levels[Level].DescriptorVisit);
                AddProduct(cpVisit);
            }

            // Инициализируем удовлетворяемые потребности
            SatisfactionNeeds = new int[FormMain.Config.NeedsCreature.Count];
            if (TypeConstruction.Levels[Level].DescriptorVisit != null)
            {
                foreach ((DescriptorNeed, int) need in TypeConstruction.Levels[Level].DescriptorVisit.ListNeeds)
                {
                    SatisfactionNeeds[need.Item1.Index] = need.Item2;
                }
            }

            if (needNotice)
                Player.AddNoticeForPlayer(this, Level == 1 ? TypeNoticeForPlayer.Build : TypeNoticeForPlayer.LevelUp);
        }

        private void CreateProducts()
        {
            foreach (DescriptorSmallEntity se in TypeConstruction.Levels[Level].Extensions)
            {
                if (se is DescriptorConstructionExtension dce)
                    AddProduct(new ConstructionProduct(this, dce));
                else if (se is DescriptorItem di)
                    AddProduct(new ConstructionProduct(this, di));
                else if (se is DescriptorResource dr)
                    AddProduct(new ConstructionProduct(this, dr));
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
            if (!Hidden)
            {
                Debug.Assert(TypeConstruction != null);

                ValidateResearches();
                FillResearches(menu);
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

        internal int CostBuyOrUpgradeForLevel(int level)
        {
            return TypeConstruction.Levels[level].Cost;
        }

        internal int CostBuyOrUpgrade()
        {
            return CanLevelUp() == true ? TypeConstruction.Levels[Level + 1].Cost : 0;
        }

        internal bool CheckRequirements()
        {
            // При постройке храма из меню Святой земли, сюда прилетает 2 уровень
            if (TypeConstruction.MaxLevel < Level + 1)
                return false;

            // Сначала проверяем наличие золота
            if (!Player.CheckRequireGold(TypeConstruction.Levels[Level + 1].Cost))
                return false;

            // Проверяем наличие очков строительства
            if (!Player.CheckRequireBuilders(TypeConstruction.Levels[Level + 1].Builders))
                return false;

            // Проверяем, что нет события или турнира
            foreach (ConstructionProduct cp in Visits)
            {
                if (cp.DescriptorConstructionEvent != null)
                    return false;
            }

            // Проверяем требования к зданиям
            return Player.CheckRequirements(TypeConstruction.Levels[Level + 1].Requirements);
        }

        internal List<TextRequirement> GetTextRequirements(int level)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            Player.TextRequirements(TypeConstruction.Levels[level].Requirements, list);

            foreach (ConstructionProduct cp in Visits)
            {
                if (cp.DescriptorConstructionEvent != null)
                {
                    list.Add(new TextRequirement(false, "В сооружении идет событие"));
                    break;
                }
            }

            return list;
        }

        internal int Income()
        {
            return Level > 0 ? TypeConstruction.Levels[Level].Income : 0;
        }

        internal bool DoIncome()
        {
            return TypeConstruction.Levels[1].Income > 0;
        }

        internal int IncomeForLevel(int level)
        {
            return TypeConstruction.Levels[level].Income;
        }

        internal int DayBuildingForLevel(int level)
        {
            return TypeConstruction.Levels[level].DaysProcessing;
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

        internal Hero HireHero(DescriptorCreature th, int gold)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= TypeConstruction.TrainedHero.Cost);

            Hero h = new Hero(this, Player, th);

            if (th.CategoryCreature != CategoryCreature.Citizen)
            {
                if (gold > 0)
                {
                    Player.SpendResource(FormMain.Config.Gold, gold);
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

        internal override void PrepareHint()
        {
            Debug.Assert(!Destroyed);

            if (Player == Player.Lobby.CurrentPlayer)
            {

                if (TypeConstruction.IsOurConstruction)
                {
                    Program.formMain.formHint.AddStep2Header(TypeConstruction.Name);
                    Program.formMain.formHint.AddStep4Level(Level > 0 ? "Уровень " + Level.ToString() + Environment.NewLine : "" + TypeConstruction.TypeConstruction.Name);
                    Program.formMain.formHint.AddStep5Description(TypeConstruction.Description + ((Level > 0) && (Heroes.Count > 0) ? Environment.NewLine + Environment.NewLine
                        + (Heroes.Count > 0 ? "Героев: " + Heroes.Count.ToString() + "/" + MaxHeroes().ToString() : "") : ""));
                    Program.formMain.formHint.AddStep6Income(Income());
                    Program.formMain.formHint.AddStep8Greatness(0, GreatnessPerDay());
                    Program.formMain.formHint.AddStep9PlusBuilders(BuildersPerDay());
                    Program.formMain.formHint.AddStep9Interest(GetInterest(), false);
                    Program.formMain.formHint.AddStep9ListNeeds(SatisfactionNeeds);
                }
                else
                {
                    if (Hidden)
                    {
                        Program.formMain.formHint.AddStep2Header("Неизвестное место");
                        Program.formMain.formHint.AddStep4Level("Место не разведано");
                        Program.formMain.formHint.AddStep5Description("Установите флаг разведки для отправки героев к месту");
                    }
                    else
                    {
                        Program.formMain.formHint.AddStep2Header(TypeConstruction.Name);
                        Program.formMain.formHint.AddStep4Level(TypeConstruction.TypeConstruction.Name);
                        Program.formMain.formHint.AddStep5Description(TypeConstruction.Description);

                        if (TypeConstruction.Reward != null)
                        {
                            Program.formMain.formHint.AddStep7Reward(TypeConstruction.Reward.Gold);
                            Program.formMain.formHint.AddStep8Greatness(TypeConstruction.Reward.Greatness, 0);
                        }
                    }
                }
            }
        }

        internal override void HideInfo()
        {
            Debug.Assert(!Destroyed);

            Program.formMain.panelConstructionInfo.Visible = false;
            Program.formMain.panelLairInfo.Visible = false;
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Debug.Assert(!Destroyed);

            if (TypeConstruction.IsOurConstruction)
            {
                Program.formMain.panelConstructionInfo.Visible = true;
                Program.formMain.panelConstructionInfo.Entity = this;
                if (selectPage >= 0)
                    Program.formMain.panelConstructionInfo.SelectPage(selectPage);
            }
            else
            {
                Program.formMain.panelConstructionInfo.Visible = true;
                Program.formMain.panelConstructionInfo.Entity = this;
//                Program.formMain.panelLairInfo.Visible = true;
//                Program.formMain.panelLairInfo.Entity = this;
            }

        }

        internal void PrepareTurn()
        {
            if (Level > 0)
            {
                if (Lobby.Turn > 1)
                {
                    if (TypeConstruction.Levels[Level].GreatnessPerDay > 0)
                        Player.AddGreatness(GreatnessPerDay());
                }

                ConstructionProduct cp;
                for (int i = 0; i < Visits.Count;)
                {
                    cp = Visits[i];
                    if (cp.Duration > 0)
                    {
                        cp.Counter--;
                        if (cp.Counter == 0)
                            RemoveProduct(cp.Descriptor);
                        else
                            i++;
                    }
                    else
                        i++;
                }

                foreach (ConstructionCellMenu cm in Researches)
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
                ConstructionCellMenu cm = ListQueueProcessing[0];
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

        internal List<TextRequirement> GetResearchTextRequirements(ConstructionCellMenu research)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (TypeConstruction.IsInternalConstruction)
            {
                // Если нет требований, то по умолчанию остается только одно - сооружение должно быть построено
                // Если есть, то не надо писать, что сооружение не построено - иначе не видно, какие там требования
                if (Level == 0)
                    list.Add(new TextRequirement(false, "Сооружение не построено"));

                Player.TextRequirements(research.Descriptor.Requirements, list);
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
            foreach (MonsterLevelLair mll in TypeConstruction.Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new Monster(mll.Monster, mll.Level, this);
                    Monsters.Add(lm);
                    AddCombatHero(lm);
                }
            }
        }

        internal int CostScout()
        {
            Debug.Assert(Hidden);
            AssertNotDestroyed();

            return PriorityFlag < PriorityExecution.Exclusive ?
                Location.Settings.CostScout * Player.Lobby.TypeLobby.CoefFlagScout[(int)PriorityFlag + 1] / 100 : 0;
        }

        private void AssertNotHidden()
        {
            Debug.Assert(!Hidden, $"Логово {TypeConstruction.ID} игрока {Player.GetName()} скрыто.");
        }

        internal void AssertNotDestroyed()
        {
            Debug.Assert(!Destroyed, $"Логово {TypeConstruction.ID} игрока {Player.GetName()} уничтожено.");
        }

        internal int CostAttack()
        {
            AssertNotHidden();
            AssertNotDestroyed();

            return PriorityFlag < PriorityExecution.Exclusive ?
                Location.Settings.CostAttack * Player.Lobby.TypeLobby.CoefFlagAttack[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal int CostDefense()
        {
            AssertNotHidden();
            AssertNotDestroyed();

            return PriorityFlag < PriorityExecution.Exclusive ?
                Location.Settings.CostDefense * Player.Lobby.TypeLobby.CoefFlagDefense[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal string NameLair()
        {
            AssertNotDestroyed();
            return Hidden ? "Неизвестное место" : TypeConstruction.Name;
        }

        internal int ImageIndexLair()
        {
            AssertNotDestroyed();

            return Hidden ? FormMain.IMAGE_INDEX_UNKNOWN : TypeConstruction.ImageIndex;
        }

        internal bool ImageEnabled()
        {
            return (Level > 0) || (TypeConstruction.MaxLevel == 0);
        }

        internal Color GetColorCaption()
        {
            if (PriorityFlag == PriorityExecution.None)
                return Hidden ? FormMain.Config.ColorMapObjectCaption(false) : Color.MediumAquamarine;

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

        internal int RequiredGold()
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

            if (Player.Gold < RequiredGold())
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

            if (Hidden)
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

            int gold = RequiredGold();// На всякий случай запоминаем точное значение. вдруг потом при трате что-нибудь поменяется
            Player.SpendResource(FormMain.Config.Gold, gold);
            SpendedGoldForSetFlag += gold;

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

            Program.formMain.LairsWithFlagChanged();
        }

        internal int Cashback()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag > 0);
            Debug.Assert(DaySetFlag > 0);
            Debug.Assert(TypeFlag != TypeFlag.None);
            AssertNotDestroyed();

            return DaySetFlag == Player.Lobby.Turn ? SpendedGoldForSetFlag : 0;
        }

        internal void CancelFlag()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag > 0);
            Debug.Assert(DaySetFlag > 0);
            Debug.Assert(TypeFlag != TypeFlag.None);
            AssertNotDestroyed();

            Player.ReturnResource(FormMain.Config.Gold, Cashback());
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
            SpendedGoldForSetFlag = 0;
            DaySetFlag = 0;
            TypeFlag = TypeFlag.None;
            PriorityFlag = PriorityExecution.None;

            while (listAttackedHero.Count > 0)
                RemoveAttackingHero(listAttackedHero[0]);

            Program.formMain.LairsWithFlagChanged();
        }


        internal void Unhide(bool needNotice)
        {
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Guild);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Economic);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Temple);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Military);
            //Debug.Assert(TypeConstruction.Category != CategoryConstruction.External);
            Debug.Assert(Hidden);
            Debug.Assert(TypeFlag == TypeFlag.None);
            Debug.Assert(!Destroyed);

            Hidden = false;

            if (needNotice)
                Player.AddNoticeForPlayer(this, TypeNoticeForPlayer.Explore);
        }

        // Место разведано
        internal void DoScout()
        {
            Debug.Assert(Hidden);
            Debug.Assert(TypeFlag == TypeFlag.Scout);
            AssertNotDestroyed();

            Hidden = false;

            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            HandOutGoldHeroes();

            DropFlag();
        }

        // Логово захвачено
        internal void DoCapture()
        {
            AssertNotHidden();
            AssertNotDestroyed();
            Debug.Assert(TypeFlag == TypeFlag.Attack);
            Debug.Assert(listAttackedHero.Count > 0);

            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            HandOutGoldHeroes();

            DropFlag();

            // Убираем себя из списка логов игрока
            Player.RemoveLair(this);
            Player.ApplyReward(this);
            Destroyed = true;

            // Если сооружение было выбрано, очищаем ссылку
            Program.formMain.ObjectDestroyed(this);

            // Ставим тип места, который должен быть после зачистки
            Debug.Assert(!(TypeConstruction.TypePlaceForConstruct is null));

            Construction pl = new Construction(Player, TypeConstruction.TypePlaceForConstruct, TypeConstruction.DefaultLevel, X, Y, Location, TypeNoticeForPlayer.None);
            pl.Hidden = false;
            Location.Lairs[Y, X] = pl;
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

            m.SetIsDead(ReasonOfDeath.InBattle);
            CombatHeroes.Remove(m);
            Monsters.Remove(m);

            if (Program.formMain.PlayerObjectIsSelected(m))
                Program.formMain.SelectPlayerObject(null);
        }

        // Раздаем деньги за флаг героям
        private void HandOutGoldHeroes()
        {
            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            if (listAttackedHero.Count > 0)
            {
                Debug.Assert(SpendedGoldForSetFlag > 0);

                // Определяем, по сколько денег достается каждому герою
                int goldPerHero = SpendedGoldForSetFlag / listAttackedHero.Count;
                int delta = SpendedGoldForSetFlag - goldPerHero * listAttackedHero.Count;
                Debug.Assert(goldPerHero * listAttackedHero.Count + delta == SpendedGoldForSetFlag);

                foreach (Hero h in listAttackedHero)
                    h.AddGold(goldPerHero);

                // Остаток отдаем первому герою
                if (delta > 0)
                    listAttackedHero[0].AddGold(delta);
            }
        }

        internal string ListMonstersForHint()
        {
            if (Hidden)
                return "Пока место не разведано, существа в нем неизвестны";
            else
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

        internal void PrepareHintForBuildOrUpgrade(int requiredLevel)
        {
            if (requiredLevel > TypeConstruction.MaxLevel)
                return;// Убрать это
            Debug.Assert(requiredLevel > 0);
            Debug.Assert(requiredLevel <= TypeConstruction.MaxLevel);

            Program.formMain.formHint.AddStep2Header(TypeConstruction.Name, TypeConstruction.ImageIndex);
            Program.formMain.formHint.AddStep4Level(requiredLevel == 1 ? "Уровень 1" + Environment.NewLine + TypeConstruction.TypeConstruction.Name :
                    $"Улучшить строение ({requiredLevel} ур.)" + Environment.NewLine + TypeConstruction.TypeConstruction.Name);
            Program.formMain.formHint.AddStep5Description(requiredLevel == 1 ? TypeConstruction.Description : "");
            Program.formMain.formHint.AddStep6Income(IncomeForLevel(requiredLevel));
            Program.formMain.formHint.AddStep8Greatness(GreatnesAddForLevel(requiredLevel), GreatnesPerDayForLevel(requiredLevel));
            Program.formMain.formHint.AddStep9PlusBuilders(BuildersPerDayForLevel(requiredLevel));
            if (TypeConstruction.Levels[requiredLevel].DescriptorVisit != null)
            {
                Program.formMain.formHint.AddStep9Interest(TypeConstruction.Levels[requiredLevel].DescriptorVisit.Interest, false);
                Program.formMain.formHint.AddStep9ListNeeds(TypeConstruction.Levels[requiredLevel].DescriptorVisit.ListNeeds, false);
            }
            Program.formMain.formHint.AddStep10DaysBuilding(-1, DayBuildingForLevel(requiredLevel));
            Program.formMain.formHint.AddStep11Requirement(GetTextRequirements(requiredLevel));
            Program.formMain.formHint.AddStep12Gold(CostBuyOrUpgradeForLevel(requiredLevel), Player.Gold >= CostBuyOrUpgradeForLevel(requiredLevel));
            Program.formMain.formHint.AddStep13Builders(TypeConstruction.Levels[requiredLevel].Builders, Player.FreeBuilders >= TypeConstruction.Levels[requiredLevel].Builders);
        }

        internal void PrepareHintForInhabitantCreatures()
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

                Program.formMain.formHint.AddStep2Header(TypeConstruction.IsOurConstruction ? "Жители" : "Существа");
                Program.formMain.formHint.AddStep5Description(list);
            }
            else
                Program.formMain.formHint.AddSimpleHint("Обитателей нет");
        }

        internal override int GetImageIndex()
        {
            if ((Player.Lobby.CurrentPlayer is null) || (Player == Player.Lobby.CurrentPlayer))
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

            return Hidden ? "" : Level == 0 ? "" : Level < TypeConstruction.MaxLevel ? $"{Level}/{TypeConstruction.MaxLevel}" : Level.ToString();
        }

        internal override void Click(VCCell pe)
        {
            base.Click(pe);
            Program.formMain.SelectPlayerObject(this);
        }

        internal override string GetName()
        {
            return TypeConstruction.Name;
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

            foreach (ConstructionProduct cp in AllProducts)
            {
                if (cp.IsAvailableForCreature(dc))
                {
                    list.Add(cp);
                }
            }

            return list;
        }

        internal Ability PurchaseAbility(Creature creature, ConstructionProduct product)
        {
            Debug.Assert(Abilities.IndexOf(product) >= 0);
            Debug.Assert(product.DescriptorAbility != null);

            Ability a = new Ability(creature, product.DescriptorAbility);
            return a;
        }

        internal void AddProduct(ConstructionProduct cp)
        {
            foreach (ConstructionProduct i in AllProducts)
            {
                Debug.Assert(i.Descriptor.ID != cp.Descriptor.ID);
            }

            AllProducts.Add(cp);

            if (cp.DescriptorAbility != null)
            {
                Abilities.Add(cp);
            }

            if ((cp.DescriptorItem != null) || (cp.DescriptorGroupItem != null))
            {
                Goods.Add(cp);
            }

            if (cp.DescriptorResource != null)
            {
                Resources.Add(cp);
            }

            if ((cp.DescriptorConstructionVisit != null) || (cp.DescriptorConstructionEvent != null))
            {
                Debug.Assert(Visits.Count <= 1);

                if (cp.DescriptorConstructionVisit != null)
                {
                    Debug.Assert(MainVisit == null);
                    Debug.Assert(CurrentVisit == null);
                }

                Visits.Add(cp);

                if (cp.DescriptorConstructionVisit != null)
                {
                    MainVisit = cp;
                    UpdateInterestMainVisit();
                }

                Debug.Assert(MainVisit != null);

                CurrentVisit = cp;
                MainVisit.Enabled = MainVisit == CurrentVisit;
            }

            // Если это пристройка, то прибавляем ее удовлетворение потребностей к текущим
            if (cp.DescriptorConstructionExtension != null)
            {
                Extensions.Add(cp);

                foreach ((DescriptorNeed, int) need in cp.DescriptorConstructionExtension.ListNeeds)
                {
                    ChangeNeed(need.Item1.NameNeed, need.Item2);
                }

                if (MainVisit != null)
                    UpdateInterestMainVisit();
            }
        }

        internal void RemoveProduct(DescriptorSmallEntity e)
        {
            ConstructionProduct productFromRemove = null;

            foreach (ConstructionProduct cp in AllProducts)
            {
                if (cp.Descriptor.ID == e.ID)
                {
                    productFromRemove = cp;
                    break;
                }
            }

            if (MainVisit == productFromRemove)
                MainVisit = null;

            if (CurrentVisit == productFromRemove)
            {
                CurrentVisit = MainVisit;
            }
            if (MainVisit != null)
                MainVisit.Enabled = MainVisit == CurrentVisit;

            RemoveElement(productFromRemove);
        }

        internal void RemoveElement(ConstructionProduct element)
        {
            Debug.Assert(element != null);
            AllProducts.Remove(element);

            Visits.Remove(element);
            Extensions.Remove(element);
            Resources.Remove(element);
            Goods.Remove(element);
            Abilities.Remove(element);
        }

        internal void UpdateInterestMainVisit()
        {
            MainVisit.Interest = MainVisit.DescriptorConstructionVisit.Interest;

            foreach (ConstructionProduct cp in Extensions)
                MainVisit.Interest += cp.DescriptorConstructionExtension.Interest;
        }

        private void ChangeNeed(NameNeedCreature nameNeed, int value)
        {
            SatisfactionNeeds[(int)nameNeed] += value;
        }

        internal bool GoodsExists(DescriptorItem item)
        {
            foreach (ConstructionCellMenu cm in Researches)
            {
                if (cm is CellMenuConstructionResearch cmr)
                    if (cmr.Entity.ID == item.ID)
                        return true;
            }

            return false;
        }

        internal bool GoodsAvailabled(DescriptorItem item)
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
            foreach (ConstructionProduct cp in Extensions)
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

            foreach (ConstructionProduct cp in Extensions)
            {
                if (cp.DescriptorConstructionExtension.Interest > 0)
                    text += Environment.NewLine + cp.DescriptorConstructionExtension.Name + ": " + Utils.DecIntegerBy10(cp.DescriptorConstructionExtension.Interest, true);
            }

            return text;
        }

        internal void AddEntityToQueueProcessing(ConstructionCellMenu cell)
        {
            Debug.Assert(ListQueueProcessing.IndexOf(cell) == -1);
            Debug.Assert(cell.DaysProcessed == 0);
            Debug.Assert(cell.PosInQueue == 0);
            Debug.Assert(cell.PurchaseValue == 0);

            cell.PurchaseValue = cell.GetCost();
            Player.SpendResource(FormMain.Config.Gold, cell.PurchaseValue);
            ListQueueProcessing.Add(cell);
            //Player.AddEntityToQueueBuilding()
            cell.PosInQueue = ListQueueProcessing.Count;
        }

        internal void RemoveEntityFromQueueProcessing(ConstructionCellMenu cell)
        {
            Debug.Assert(ListQueueProcessing.IndexOf(cell) != -1);
            Debug.Assert((cell.DaysLeft == 0) || (cell.DaysProcessed == 0));
            Debug.Assert(cell.PosInQueue > 0);
            Debug.Assert(cell.PurchaseValue > 0);

            cell.PosInQueue = 0;
            Player.ReturnResource(FormMain.Config.Gold, cell.PurchaseValue);
            cell.PurchaseValue = 0;
            ListQueueProcessing.Remove(cell);

            for (int i = 0; i < ListQueueProcessing.Count; i++)
            {
                ListQueueProcessing[i].PosInQueue = i + 1;
            }
        }
    }
}