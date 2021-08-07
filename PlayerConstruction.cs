using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс здания игрока
    internal sealed class PlayerConstruction : PlayerMapObject
    {
        private int gold;

        public PlayerConstruction(LobbyPlayer p, TypeConstruction b) : base(p, b)
        {
            TypeConstruction = b;

            Level = b.DefaultLevel;

            // Восстановить
            //if (Construction.HasTreasury)
            //    Gold = Construction.GoldByConstruction;
        }

        internal TypeConstruction TypeConstruction { get; }
        internal int Level { get; private set; }
        internal int Gold { get => gold; set { Debug.Assert(TypeConstruction.HasTreasury); gold = value; } }
        internal List<PlayerHero> Heroes { get; } = new List<PlayerHero>();
        internal int ResearchesAvailabled { get; private set; }// Сколько еще исследований доступно на этом ходу
        internal List<Entity> Items { get; } = new List<Entity>();// Товары, доступные в строении
        internal List<PlayerItem> Warehouse { get; } = new List<PlayerItem>();// Склад здания

        internal bool BuyOrUpgrade()
        {
            if ((Level < TypeConstruction.MaxLevel) && (Player.Gold >= CostBuyOrUpgrade()) && (CheckRequirements() == true))
            {
                Debug.Assert(Player.Gold >= CostBuyOrUpgrade());

                Player.Constructed(this);
                Level++;
                ValidateHeroes();
                PrepareTurn();

                return true;
            }

            return false;
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
            if ((TypeConstruction.Levels[Level + 1].Builders > Player.FreeBuilders)
                || (TypeConstruction.Levels[Level + 1].PointConstructionTemple > Player.PointConstructionTemple)
                || (TypeConstruction.Levels[Level + 1].PointConstructionTradePost > Player.PointConstructionTradePost))
                return false;

            // Проверяем требования к зданиям
            return Player.CheckRequirements(TypeConstruction.Levels[Level + 1].Requirements);
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            if (Level == TypeConstruction.MaxLevel)
                return null;

            List<TextRequirement> list = new List<TextRequirement>();

            if (TypeConstruction.Levels[Level + 1].PointConstructionTemple > 0)
            {
                if (TypeConstruction.Levels[Level + 1].PointConstructionTemple <= Player.PointConstructionTemple)
                    list.Add(new TextRequirement(true, "Есть свободная Святая земля"));
                else
                    list.Add(new TextRequirement(false, "Нет свободных Святых земель"));
            }

            if (TypeConstruction.Levels[Level + 1].PointConstructionTradePost > 0)
            {
                if (TypeConstruction.Levels[Level + 1].PointConstructionTradePost <= Player.PointConstructionTradePost)
                    list.Add(new TextRequirement(true, "Есть свободное торговое место"));
                else
                    list.Add(new TextRequirement(false, "Нет свободных торговых мест"));
            }

            Player.TextRequirements(TypeConstruction.Levels[Level + 1].Requirements, list);

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

        internal PlayerHero HireHero(TypeHero th)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= TypeConstruction.TrainedHero.Cost);

            PlayerHero h = new PlayerHero(this, Player, th);

            if (TypeConstruction.TrainedHero.Cost > 0)
            {
                Player.SpendGold(TypeConstruction.TrainedHero.Cost);
                if (Player.Player.TypePlayer == TypePlayer.Human)
                    Program.formMain.SetNeedRedrawFrame();
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

        internal int MaxHeroes()
        {
            return Level > 0 ? TypeConstruction.Levels[Level].MaxHeroes : 0;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeConstruction.Name, Level > 0 ? (TypeConstruction.LevelAsQuantity ? "Количество: " : "Уровень ") + Level.ToString() : "", TypeConstruction.Description + ((Level > 0) && (TypeConstruction.TrainedHero != null) ? Environment.NewLine + Environment.NewLine                
                + (!(TypeConstruction is TypeEconomicConstruction) ? "Героев: " + Heroes.Count.ToString() + "/" + MaxHeroes().ToString() : "") : ""));
            Program.formMain.formHint.AddStep2Income(Income());
            Program.formMain.formHint.AddStep3Greatness(0, GreatnessPerDay());
            Program.formMain.formHint.AddStep35PlusBuilders(BuildersPerDay());
        }

        internal override void HideInfo()
        {
            Program.formMain.panelConstructionInfo.Visible = false;
        }

        internal override void ShowInfo()
        {
            Program.formMain.panelConstructionInfo.Visible = true;
            Program.formMain.panelConstructionInfo.PlayerObject = this;
        }

        internal void ResearchCompleted()
        {
            Debug.Assert(ResearchesAvailabled > 0);

            ResearchesAvailabled--;
        }

        internal void PrepareTurn()
        {
            Debug.Assert(Level > 0);

            ResearchesAvailabled = TypeConstruction.ResearchesPerDay;
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

        internal override bool CheckRequirementsForResearch(PlayerResearch research)
        {
            // Сначала проверяем, построено ли здание
            if (Level == 0)
                return false;

            // Потом проверяем наличие золота
            if (Player.Gold < research.Cost())
                return false;

            // Проверяем, что еще можно делать исследования
            if (!CanResearch())
                return false;

            // Проверяем требования к исследованию
            return Player.CheckRequirements(research.Research.Requirements);
        }

        internal override List<TextRequirement> GetTextRequirements(PlayerResearch research)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Level == 0)
                list.Add(new TextRequirement(false, "Здание не построено"));
            else
            {
                Player.TextRequirements(research.Research.Requirements, list);

                if (!CanResearch())
                    list.Add(new TextRequirement(false, "Больше нельзя выполнять исследований в этот день"));
            }

            return list;
        }

        internal override void ResearchCompleted(PlayerResearch research)
        {
            base.ResearchCompleted(research);

            Debug.Assert(research.Research.Entity != null);
            Items.Add(research.Research.Entity);
        }
    }
}