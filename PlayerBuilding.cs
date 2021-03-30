using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс здания игрока
    internal sealed class PlayerBuilding : PlayerObject
    {
        private int gold;

        public PlayerBuilding(Player p, TypeConstruction b)
        {
            Player = p;
            Building = b;

            Level = b.DefaultLevel;

            // Настраиваем исследования 
            if (b.Researches != null)
            {
                for (int z = 0; z < b.Researches.GetLength(0); z++)
                    for (int y = 0; y < b.Researches.GetLength(1); y++)
                        for (int x = 0; x < b.Researches.GetLength(2); x++)
                            if (b.Researches[z, y, x] != null)
                                Researches.Add(new PlayerResearch(this, b.Researches[z, y, x]));
            }

            if (Building is TypeGuild)
                Warehouse.Add(new PlayerItem(FormMain.Config.FindItem("PotionOfHealth"), 3, true));

            // Восстановить
            //if (Building.HasTreasury)
            //    Gold = Building.GoldByConstruction;
        }

        internal Player Player { get; }
        internal TypeConstruction Building { get; }
        internal int Level { get; private set; }
        internal int Gold { get => gold; set { Debug.Assert(Building.HasTreasury); gold = value; } }
        internal List<PlayerHero> Heroes { get; } = new List<PlayerHero>();
        internal List<PlayerResearch> Researches { get; } = new List<PlayerResearch>();
        internal List<Entity> Items { get; } = new List<Entity>();// Товары, доступные в строении
        internal List<PlayerItem> Warehouse { get; } = new List<PlayerItem>();// Склад здания

        internal bool BuyOrUpgrade()
        {
            if ((Level < Building.MaxLevel) && (Player.Gold >= CostBuyOrUpgrade()) && (CheckRequirements() == true))
            {
                Debug.Assert(Player.Gold >= CostBuyOrUpgrade());

                Player.Constructed(this);
                Level++;
                ValidateHeroes();
                return true;
            }

            return false;
        }

        internal void ValidateHeroes()
        {
            // Восстановить
            /*if ((Building.TrainedHero != null) && (Building.TrainedHero.Cost == 0))
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
            return Level < Building.MaxLevel;
        }

        internal int CostBuyOrUpgrade()
        {
            return CanLevelUp() == true ? Building.Levels[Level + 1].Cost : 0;
        }

        internal bool CheckRequirements()
        {
            // Сначала проверяем наличие золота
            if (Player.Gold < Building.Levels[Level + 1].Cost)
                return false;

            // Проверяем наличие очков строительства
            if ((Building.PointConstructionGuild > Player.PointConstructionGuild)
                || (Building.PointConstructionEconomic > Player.PointConstructionEconomic)
                || (Building.PointConstructionTemple > Player.PointConstructionTemple)
                || (Building.PointConstructionTradePost > Player.PointConstructionTradePost))
                return false;

            // Проверяем требования к зданиям
            return Player.CheckRequirements(Building.Levels[Level + 1].Requirements);
        }

        internal List<TextRequirement> GetTextRequirements()
        {
            if (Level == Building.MaxLevel)
                return null;

            List<TextRequirement> list = new List<TextRequirement>();

            if (Building.PointConstructionGuild > 0)
            {
                if (Building.PointConstructionGuild <= Player.PointConstructionGuild)
                    list.Add(new TextRequirement(true, $"Есть Разрешение на {(Level == 0 ? "cтроительство" : "улучшение")} гильдии"));
                else
                    list.Add(new TextRequirement(false, $"Нет Разрешений на {(Level == 0 ? "cтроительство" : "улучшение")} гильдии"));
            }

            if (Building.PointConstructionEconomic > 0)
            {
                if (Building.PointConstructionEconomic <= Player.PointConstructionEconomic)
                    list.Add(new TextRequirement(true, $"Есть Разрешение на {(Level == 0 ? "cтроительство" : "улучшение")} экономического сооружения")) ;
                else
                    list.Add(new TextRequirement(false, $"Нет Разрешения на {(Level == 0 ? "cтроительство" : "улучшение")} экономического сооружения"));
            }

            if (Building.PointConstructionTemple > 0)
            {
                if (Building.PointConstructionTemple <= Player.PointConstructionTemple)
                    list.Add(new TextRequirement(true, "Есть свободная Святая земля"));
                else
                    list.Add(new TextRequirement(false, "Нет свободных Святых земель"));
            }

            if (Building.PointConstructionTradePost > 0)
            {
                if (Building.PointConstructionTradePost <= Player.PointConstructionTradePost)
                    list.Add(new TextRequirement(true, "Есть свободное торговое место"));
                else
                    list.Add(new TextRequirement(false, "Нет свободных торговых мест"));
            }

            Player.TextRequirements(Building.Levels[Level + 1].Requirements, list);

            return list;
        }

        internal int Income()
        {
            return Level > 0 ? Building.Levels[Level].Income : 0;
        }

        internal bool DoIncome()
        {
            return Building.Levels[1].Income > 0;
        }

        internal int IncomeNextLevel()
        {
            return Level < Building.MaxLevel ? Building.Levels[Level + 1].Income : 0;
        }

        internal PlayerHero HireHero()
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= Building.TrainedHero.Cost);

            PlayerHero h = new PlayerHero(this, Player);

            if (Building.TrainedHero.Cost > 0)
            {
                Player.Gold -= Building.TrainedHero.Cost;
                if (Player.TypePlayer == TypePlayer.Human)
                    Program.formMain.SetNeedRedrawFrame();
            }

            AddHero(h);

            return h;
        }

        internal PlayerHero HireHero(TypeHero th)
        {
            Debug.Assert(Heroes.Count < MaxHeroes());
            Debug.Assert(Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
            //Debug.Assert(Player.Gold >= Building.TrainedHero.Cost);

            PlayerHero h = new PlayerHero(this, Player, th);

            if (Building.TrainedHero.Cost > 0)
            {
                Player.Gold -= Building.TrainedHero.Cost;
                if (Player.TypePlayer == TypePlayer.Human)
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

            return (Level > 0) && (Player.Gold >= Building.TrainedHero.Cost) && (Heroes.Count < MaxHeroes()) && (Player.CombatHeroes.Count < Player.Lobby.TypeLobby.MaxHeroes);
        }

        internal bool MaxHeroesAtPlayer()
        {
            return Player.CombatHeroes.Count == Player.Lobby.TypeLobby.MaxHeroes;
        }

        internal List<TextRequirement> GetTextRequirementsHire()
        {
            List<TextRequirement> list = new List<TextRequirement>();

            if (Level == 0)
                list.Add(new TextRequirement(false, Building.GetTextConstructionNotBuilded()));

            if ((Level > 0) && (Heroes.Count == MaxHeroes()))
                list.Add(new TextRequirement(false, Building.GetTextConstructionIsFull()));
            
            if (MaxHeroesAtPlayer())
                list.Add(new TextRequirement(false, "Достигнуто максимальное количество героев в королевстве"));

            return list;
        }      

        internal int MaxHeroes()
        {
            return Level > 0 ? Building.Levels[Level].MaxHeroes : 0;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Building.Name, Level > 0 ? (Building.LevelAsQuantity ? "Количество: " : "Уровень ") + Level.ToString() : "", Building.Description + ((Level > 0) && (Building.TrainedHero != null) ? Environment.NewLine + Environment.NewLine + "Героев: " + Heroes.Count.ToString() + "/" + MaxHeroes().ToString() : ""));
            Program.formMain.formHint.AddStep2Income(Income());
        }

        internal override void HideInfo()
        {
            Program.formMain.panelBuildingInfo.Visible = false;
        }

        internal override void ShowInfo()
        {
            Program.formMain.panelBuildingInfo.Visible = true;
            Program.formMain.panelBuildingInfo.PlayerObject = this;
        }
    }
}