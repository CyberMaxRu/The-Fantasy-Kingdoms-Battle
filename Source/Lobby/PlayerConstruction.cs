﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс сооружения у игрока
    internal sealed class PlayerConstruction : BattleParticipant
    {
        private int gold;

        public PlayerConstruction(LobbyPlayer p, DescriptorConstruction b) : base(p.Lobby)
        {
            Player = p;
            TypeConstruction = b;

            // Настраиваем исследования 
            if (TypeConstruction.Researches != null)
            {
                for (int z = 0; z < TypeConstruction.Researches.GetLength(0); z++)
                    for (int y = 0; y < TypeConstruction.Researches.GetLength(1); y++)
                        for (int x = 0; x < TypeConstruction.Researches.GetLength(2); x++)
                            if (TypeConstruction.Researches[z, y, x] != null)                       
                                Researches.Add(new PlayerCellMenu(this, TypeConstruction.Researches[z, y, x]));
            }

            Hidden = !TypeConstruction.IsInternalConstruction;

            Level = b.DefaultLevel;

            // Убрать эту проверку после настройки всех логов
            if (TypeConstruction.Monsters.Count > 0)
                CreateMonsters();

            p.Constructions.Add(this);
            // Восстановить
            //if (Construction.HasTreasury)
            //    Gold = Construction.GoldByConstruction;
        }

        public PlayerConstruction(LobbyPlayer p, DescriptorConstruction l, int level, int x, int y, int layer) : base(p.Lobby)
        {
            Player = p;
            TypeConstruction = l;
            X = x;
            Y = y;
            Layer = layer;
            Hidden = !((Layer == 0) || (l.Category == CategoryConstruction.External));

            Debug.Assert((TypeConstruction.Category == CategoryConstruction.Lair) || (TypeConstruction.Category == CategoryConstruction.External)
                || (TypeConstruction.Category == CategoryConstruction.Place) || (TypeConstruction.Category == CategoryConstruction.BasePlace));

            Debug.Assert(level <= 1);
            if (level == 1)
            {
                Build();
            }

            // Настраиваем исследования 
            if (TypeConstruction.Researches != null)
            {
                for (int z = 0; z < TypeConstruction.Researches.GetLength(0); z++)
                    for (int y1 = 0; y1 < TypeConstruction.Researches.GetLength(1); y1++)
                        for (int x1 = 0; x1 < TypeConstruction.Researches.GetLength(2); x1++)
                            if (TypeConstruction.Researches[z, y1, x1] != null)
                                Researches.Add(new PlayerCellMenu(this, TypeConstruction.Researches[z, y1, x1]));
            }

            // Убрать эту проверку после настройки всех логов
            if (TypeConstruction.Monsters.Count > 0)
                CreateMonsters();

            p.Constructions.Add(this);
        }

        internal DescriptorConstruction TypeConstruction { get; }
        internal int Level { get; private set; }
        internal bool BuildedOrUpgraded { get; private set; }
        internal int Gold { get => gold; set { Debug.Assert(TypeConstruction.HasTreasury); gold = value; } }
        internal List<PlayerHero> Heroes { get; } = new List<PlayerHero>();
        internal int ResearchesAvailabled { get; private set; }// Сколько еще исследований доступно на этом ходу
        internal List<Entity> Items { get; } = new List<Entity>();// Товары, доступные в строении
        internal List<PlayerItem> Warehouse { get; } = new List<PlayerItem>();// Склад здания
        internal LobbyPlayer Player { get; }
        internal List<PlayerCellMenu> Researches { get; } = new List<PlayerCellMenu>();

        // Свойства для внешних сооружений
        internal int Layer { get; private set; }// Слой, на котором находится логово
        internal int X { get; private set; }// Позиция по X в слое
        internal int Y { get; private set; }// Позиция по Y в слое
        internal bool Hidden { get; private set; }// Логово не разведано

        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня
        internal bool Destroyed { get; private set; } = false;// Логово уничтожено, работа с ним запрещена

        // Поддержка флага
        internal TypeFlag TypeFlag { get; private set; } = TypeFlag.None;// Тип установленного флага
        internal int DaySetFlag { get; private set; }// День установки флага
        internal int SpendedGoldForSetFlag { get; private set; }// Сколько золота было потрачено на установку флага
        internal PriorityExecution PriorityFlag { get; private set; } = PriorityExecution.None;// Приоритет разведки/атаки
        internal List<PlayerHero> listAttackedHero { get; } = new List<PlayerHero>();// Список героев, откликнувшихся на флаг

        internal void ResearchCompleted(PlayerCellMenu research)
        {
            Debug.Assert(CheckRequirementsForResearch(research));
            Debug.Assert(Researches.IndexOf(research) != -1);

            Researches.Remove(research);

            if (research.Research.TypeConstruction is null)
            {
                Player.SpendGold(research.Cost());

                Debug.Assert(ResearchesAvailabled > 0);

                ResearchesAvailabled--;
                Debug.Assert(research.Research.Entity != null);
                Items.Add(research.Research.Entity);
            }
            else
            {
                if (research.ConstructionForBuild != null)
                {
                    Debug.Assert(research.ConstructionForBuild.Level == 0);
                    research.ConstructionForBuild.Build();
                    research.ConstructionForBuild.X = research.ObjectOfMap.X;
                    research.ConstructionForBuild.Y = research.ObjectOfMap.Y;
                    research.ConstructionForBuild.Layer = research.ObjectOfMap.Layer;
                    Player.Lairs[research.ConstructionForBuild.Layer, research.ConstructionForBuild.Y, research.ConstructionForBuild.X] = research.ConstructionForBuild;
                }
                else
                {
                    PlayerConstruction pc = new PlayerConstruction(Player, research.Research.TypeConstruction, 1, research.ObjectOfMap.X, research.ObjectOfMap.Y, research.ObjectOfMap.Layer);
                    Player.Lairs[pc.Layer, pc.Y, pc.X] = pc;
                    Program.formMain.SelectPlayerObject(pc);
                }

                if (Player.GetTypePlayer() == TypePlayer.Human)
                    Program.formMain.UpdateNeighborhood();                        

                Program.formMain.SetNeedRedrawFrame();
                Program.formMain.PlayConstructionComplete();
            }
        }

        internal void Build()
        {
            Debug.Assert(Level == 0);
            Debug.Assert(CheckRequirements());
            Debug.Assert(Player.Gold >= CostBuyOrUpgrade());
            Debug.Assert(!BuildedOrUpgraded);

            Player.Constructed(this);
            Level++;
            ValidateHeroes();
            PrepareTurn();
            if (!Player.Initialization)
                BuildedOrUpgraded = true;
        }

        internal void Upgrade()
        {
            Debug.Assert(Level < TypeConstruction.MaxLevel);
            Debug.Assert(CheckRequirements());
            Debug.Assert(Player.Gold >= CostBuyOrUpgrade());
            Debug.Assert(!BuildedOrUpgraded);

            Player.Constructed(this);
            Level++;
            if (!Player.Initialization)
                BuildedOrUpgraded = true;
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

        internal void ValidateResearches()
        {
            Debug.Assert(Researches != null);

            List<PlayerCellMenu> forRemove = new List<PlayerCellMenu>();

            foreach (PlayerCellMenu mc in Researches)
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

            foreach (PlayerCellMenu mc in forRemove)
            {
                Researches.Remove(mc);
            }
        }

        internal bool CanLevelUp()
        {
            return Level < TypeConstruction.MaxLevel;
        }

        internal int CostBuyOrUpgrade()
        {
            return CanLevelUp() == true ? TypeConstruction.Levels[Level + 1].Cost : 0;
        }

        internal bool CheckRequirements()
        {
            // Сначала проверяем наличие золота
            if (Player.Gold < TypeConstruction.Levels[Level + 1].Cost)
                return false;

            // Проверяем наличие очков строительства
            if (TypeConstruction.Levels[Level + 1].Builders > Player.FreeBuilders)
                return false;

            // Проверяем, что на этом ходу сооружение не строили/улучшали
            if (BuildedOrUpgraded)
                return false;

            // Проверяем требования к зданиям
            return Player.CheckRequirements(TypeConstruction.Levels[Level + 1].Requirements);
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            if (Level == TypeConstruction.MaxLevel)
                return null;

            List<TextRequirement> list = new List<TextRequirement>();

            Player.TextRequirements(TypeConstruction.Levels[Level + 1].Requirements, list);

            if (BuildedOrUpgraded)
                list.Add(new TextRequirement(false, "Сооружение уже строили/улучшали в этот день"));

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

        internal int IncomeNextLevel()
        {
            return Level < TypeConstruction.MaxLevel ? TypeConstruction.Levels[Level + 1].Income : 0;
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
            return Level < TypeConstruction.MaxLevel ? TypeConstruction.Levels[Level + 1].GreatnessByConstruction : 0;
        }

        internal int GreatnessPerDayNextLevel()
        {
            return Level < TypeConstruction.MaxLevel ? TypeConstruction.Levels[Level + 1].GreatnessPerDay : 0;
        }

        internal int BuildersPerDayNextLevel()
        {
            return Level < TypeConstruction.MaxLevel ? TypeConstruction.Levels[Level + 1].BuildersPerDay : 0;
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


        internal PlayerHero HireHero()
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= Construction.TrainedHero.Cost);

            PlayerHero h = new PlayerHero(this, Player);

            if (TypeConstruction.TrainedHero.Cost > 0)
            {
                Player.SpendGold(TypeConstruction.TrainedHero.Cost);
                if (Player.Player.TypePlayer == TypePlayer.Human)
                    Program.formMain.SetNeedRedrawFrame();
            }

            AddHero(h);

            return h;
        }

        internal PlayerHero HireHero(DescriptorCreature th)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= TypeConstruction.TrainedHero.Cost);

            PlayerHero h = new PlayerHero(this, Player, th);

            if (th.CategoryCreature != CategoryCreature.Citizen)
            {
                if (TypeConstruction.TrainedHero.Cost > 0)
                {
                    Player.SpendGold(TypeConstruction.TrainedHero.Cost);
                    if (Player.Player.TypePlayer == TypePlayer.Human)
                        Program.formMain.SetNeedRedrawFrame();
                }
            }

            AddHero(h);

            return h;
        }

        internal void AddHero(PlayerHero ph)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);

            Heroes.Add(ph);
            Player.AddHero(ph);
        }

        internal bool CanTrainHero()
        {
            Debug.Assert(Heroes.Count <= MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count <= Player.Lobby.TypeLobby.MaxHeroes);

            return (Level > 0) && (Player.Gold >= TypeConstruction.TrainedHero.Cost) && (Heroes.Count < MaxHeroes()) && (Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
        }

        internal bool MaxHeroesAtPlayer()
        {
            return Player.CombatHeroes.Count == Player.Lobby.TypeLobby.MaxHeroes;
        }

        internal int MaxHeroes()
        {
            return Level > 0 ? TypeConstruction.Levels[Level].MaxHeroes : 0;
        }

        internal override void PrepareHint()
        {
            Debug.Assert(!Destroyed);

            if (Player == Player.Lobby.CurrentPlayer)
            {

                if (TypeConstruction.IsOurConstruction)
                {
                    Program.formMain.formHint.AddStep1Header(TypeConstruction.Name, Level > 0 ? "Уровень " + Level.ToString() : "", TypeConstruction.Description + ((Level > 0) && (TypeConstruction.TrainedHero != null) ? Environment.NewLine + Environment.NewLine
                        + (!(TypeConstruction.TrainedHero is null) ? "Героев: " + Heroes.Count.ToString() + "/" + MaxHeroes().ToString() : "") : ""));
                    Program.formMain.formHint.AddStep2Income(Income());
                    Program.formMain.formHint.AddStep3Greatness(0, GreatnessPerDay());
                    Program.formMain.formHint.AddStep35PlusBuilders(BuildersPerDay());
                }
                else
                {
                    if (Hidden)
                        Program.formMain.formHint.AddStep1Header("Неизвестное место", "Место не разведано", "Установите флаг разведки для отправки героев к месту");
                    else
                    {
                        Program.formMain.formHint.AddStep1Header(TypeConstruction.Name, "", TypeConstruction.Description);
                        if (TypeConstruction.TypeReward != null)
                        {
                            Program.formMain.formHint.AddStep2Reward(TypeConstruction.TypeReward.Gold);
                            Program.formMain.formHint.AddStep3Greatness(TypeConstruction.TypeReward.Greatness, 0);
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

        internal override void ShowInfo()
        {
            Debug.Assert(!Destroyed);

            if (TypeConstruction.IsOurConstruction)
            {
                Program.formMain.panelConstructionInfo.Visible = true;
                Program.formMain.panelConstructionInfo.PlayerObject = this;
            }
            else
            {
                Program.formMain.panelLairInfo.Visible = true;
                Program.formMain.panelLairInfo.PlayerObject = this;
            }

        }

        internal void PrepareTurn()
        {
            Debug.Assert(Level > 0);

            ResearchesAvailabled = TypeConstruction.ResearchesPerDay;
            BuildedOrUpgraded = false;
        }

        internal void AfterEndTurn()
        {
            Debug.Assert(Level > 0);

            if (TypeConstruction.Levels[Level].GreatnessPerDay > 0)
                Player.AddGreatness(GreatnessPerDay());
        }

        internal bool CanResearch()
        {
            return ResearchesAvailabled > 0;
        }

        internal bool CheckRequirementsForResearch(PlayerCellMenu research)
        {
            // Сначала проверяем, построено ли здание
            if (TypeConstruction.IsInternalConstruction)
                if (Level == 0)
                    return false;

            // Потом проверяем наличие золота
            if (Player.Gold < research.Cost())
                return false;

            // Проверяем, что еще можно делать исследования
            if (research.Research.Entity != null)
                if (!CanResearch())
                    return false;

            // Проверяем требования к исследованию
            if (research.Research.TypeConstruction is null)
                return Player.CheckRequirements(research.Research.Requirements);
            else
            {
                if (research.ConstructionForBuild != null)
                    return research.ConstructionForBuild.CheckRequirements();
                else
                    return research.Player.CanBuildTypeConstruction(research.Research.TypeConstruction);
            }
        }

        internal List<TextRequirement> GetResearchTextRequirements(PlayerCellMenu research)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (TypeConstruction.IsInternalConstruction)
            {
                if (Level == 0)
                    list.Add(new TextRequirement(false, "Здание не построено"));
                else
                {
                    Player.TextRequirements(research.Research.Requirements, list);

                    if (research.Research.Entity != null)
                    {
                        if (ResearchesAvailabled > 0)
                            list.Add(new TextRequirement(true, $"Доступно к изучению: {ResearchesAvailabled}"));
                        else
                            list.Add(new TextRequirement(false, "Больше нельзя изучить в этот день"));
                    }
                }
            }

            return list;
        }

        internal bool ShowMenuForPlayer() => !Hidden;

        internal override int GetLevel()
        {
            return Level;
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
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostScout * Player.Lobby.TypeLobby.CoefFlagScout[(int)PriorityFlag + 1] / 100 : 0;
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
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostAttack * Player.Lobby.TypeLobby.CoefFlagAttack[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal int CostDefense()
        {
            AssertNotHidden();
            AssertNotDestroyed();

            return PriorityFlag < PriorityExecution.Exclusive ?
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostDefense * Player.Lobby.TypeLobby.CoefFlagDefense[(int)PriorityFlag + 1] / 100 : 0;
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
            DaySetFlag = Player.Lobby.Day;
        }

        internal void IncPriority()
        {
            Debug.Assert(PriorityFlag < PriorityExecution.Exclusive);
            AssertNotDestroyed();

            // 

            int gold = RequiredGold();// На всякий случай запоминаем точное значение. вдруг потом при трате что-нибудь поменяется
            Player.SpendGold(gold);
            SpendedGoldForSetFlag += gold;

            if (DaySetFlag == 0)
            {
                Debug.Assert(TypeFlag == TypeFlag.None);
                TypeFlag = TypeAction();
                DaySetFlag = Player.Lobby.Day;
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

            return DaySetFlag == Player.Lobby.Day ? SpendedGoldForSetFlag : 0;
        }

        internal void CancelFlag()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag > 0);
            Debug.Assert(DaySetFlag > 0);
            Debug.Assert(TypeFlag != TypeFlag.None);
            AssertNotDestroyed();

            Player.ReturnGold(Cashback());
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

        internal void AddAttackingHero(PlayerHero ph)
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

        internal void RemoveAttackingHero(PlayerHero ph)
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


        internal void Unhide()
        {
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Guild);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Economic);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Temple);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.Military);
            Debug.Assert(TypeConstruction.Category != CategoryConstruction.External);
            Debug.Assert(Hidden);
            Debug.Assert(TypeFlag == TypeFlag.None);
            Debug.Assert(!Destroyed);

            Hidden = false;
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

            // Ставим тип места, который должен быть после зачистки
            Debug.Assert(!(TypeConstruction.TypePlaceForConstruct is null));

            PlayerConstruction pl = new PlayerConstruction(Player, TypeConstruction.TypePlaceForConstruct, 1, X, Y, Layer);
            pl.Hidden = false;
            Player.Lairs[Layer, Y, X] = pl;
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

            m.SetIsDead();
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

                foreach (PlayerHero h in listAttackedHero)
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
                foreach (Monster m in Monsters)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{m.TypeCreature.Name}, {m.Level} ур.";
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
                foreach (PlayerHero h in listAttackedHero)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{h.GetNameHero()}, {h.Level} ур.";
                }

                return list;
            }
        }

        internal void PrepareHintForBuildOrUpgrade()
        {
            if (Level < TypeConstruction.MaxLevel)
            {
                Program.formMain.formHint.AddStep1Header(TypeConstruction.Name, Level == 0 ? "Уровень 1" : (CanLevelUp() == true) ? $"Улучшить строение ({Level + 1} ур.)" : "", Level == 0 ? TypeConstruction.Description : "");
                Program.formMain.formHint.AddStep2Income(IncomeNextLevel());
                Program.formMain.formHint.AddStep3Greatness(GreatnessAddNextLevel(), GreatnessPerDayNextLevel());
                Program.formMain.formHint.AddStep35PlusBuilders(BuildersPerDayNextLevel());
                Program.formMain.formHint.AddStep3Requirement(GetTextRequirements());
                Program.formMain.formHint.AddStep4Gold(CostBuyOrUpgrade(), Player.Gold >= CostBuyOrUpgrade());
                Program.formMain.formHint.AddStep5Builders(TypeConstruction.Levels[Level + 1].Builders, Player.FreeBuilders >= TypeConstruction.Levels[Level + 1].Builders);
            }
        }

        internal void PrepareHintForHireHero()
        {
            if (Heroes.Count < MaxHeroes())
            {

                Program.formMain.formHint.AddStep1Header(TypeConstruction.TrainedHero.Name, "", TypeConstruction.TrainedHero.Description);
                if ((TypeConstruction.TrainedHero != null) && (TypeConstruction.TrainedHero.Cost > 0))
                    Program.formMain.formHint.AddStep3Requirement(GetTextRequirementsHire());
                Program.formMain.formHint.AddStep4Gold(TypeConstruction.TrainedHero.Cost, Player.Gold >= TypeConstruction.TrainedHero.Cost);
            }
        }

        internal void PrepareHintForInhabitantCreatures()
        {
            Debug.Assert(Heroes.Count > 0);

            string list = "";
            foreach (PlayerHero h in Heroes)
            {
                list += (list != "" ? Environment.NewLine : "") + $"{h.GetNameHero()}, {h.Level} ур.";
            }

            Program.formMain.formHint.AddStep1Header(TypeConstruction.IsOurConstruction ? "Жители" : "Существа", "", list);
        }

        internal override int GetImageIndex()
        {
            if (Player == Player.Lobby.CurrentPlayer)
                return ImageIndexLair();
            else
                return FormMain.Config.Gui48_Battle;
        }
        internal override string GetCost() => null;

        internal override bool GetNormalImage()
        {
            return true;
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

        internal override LobbyPlayer GetPlayer()
        {
            return Player;
        }

        internal override int GetImageIndexAvatar()
        {
            return TypeConstruction.ImageIndex;
        }
    }
}