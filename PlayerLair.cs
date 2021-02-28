using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum PriorityExecution { None = -1, Normal = 0, Warning = 1, High = 2, Exclusive = 3};

    // Класс логова игрока
    internal sealed class PlayerLair : BattleParticipant
    {
        public PlayerLair(Player p, TypeLair l, int layer) : base()
        {
            Player = p;
            TypeLair = l;
            Layer = layer;

            Level = 1;
            Name = l.Name;
            ImageIndexAvatar = l.ImageIndex;
            TypePlayer = TypePlayer.Lair;
            Hidden = true;

            // Убрать эту проверку после настройки всех логов
            if (TypeLair.LevelLairs.Count > 0)
                CreateMonsters();
        }
        internal Player Player { get; }
        internal TypeLair TypeLair { get; }
        internal int Level { get; private set; }// Текущий уровень логова
        internal int Layer { get; }// Слой, на котором находится логово
        internal bool Hidden { get; set; }// Логово не разведано
        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня

        // Поддержка флага
        internal int DaySetFlag { get; private set; }// День установки флага
        internal int SpendedGoldForSetFlag { get; private set; }// Сколько золота было потрачено на установку флага
        internal PriorityExecution PriorityFlag { get; set; } = PriorityExecution.None;// Приоритет разведки/атаки

        private void CreateMonsters()
        {
            Debug.Assert(TypeLair.LevelLairs.Count <= Level);
            Debug.Assert(TypeLair.LevelLairs[Level - 1].Monsters.Count > 0);

            Monster lm;
            foreach (MonsterLevelLair mll in TypeLair.LevelLairs[Level - 1].Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new Monster(mll.Monster, mll.Level, this);
                    Monsters.Add(lm);
                    CombatHeroes.Add(lm);
                }
            }
        }

        internal override void PreparingForBattle()
        {
            base.PreparingForBattle();
        }

        internal int CostScout()
        {
            Debug.Assert(Hidden);

            return PriorityFlag < PriorityExecution.Exclusive ? 
                Player.Lobby.TypeLobby.LairSettings[Layer].CostScout * Player.Lobby.TypeLobby.CoefFlagScout[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal int CostAttack()
        {
            Debug.Assert(!Hidden);
            Debug.Assert(Level > 0);

            return PriorityFlag < PriorityExecution.Exclusive ?
                TypeLair.LevelLairs[Level - 1].Cost * Player.Lobby.TypeLobby.CoefFlagAttack[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal override void PrepareHint()
        {
            if (Hidden)
                Program.formMain.formHint.AddStep1Header("Неизвестное логово", "Логово не разведано", "Установите флаг разведки для отправки героев к логову");
            else
                Program.formMain.formHint.AddStep1Header(TypeLair.Name, "", TypeLair.Description);
        }

        internal string NameLair()
        {
            return Hidden ? "Неизвестное логово" : TypeLair.Name;
        }

        internal int ImageIndexLair()
        {
            return Hidden ? FormMain.IMAGE_INDEX_NONE : TypeLair.ImageIndex;
        }

        internal int RequiredGold()
        {
            return Hidden ? CostScout() : CostAttack();
        }

        internal bool CheckRequirements()
        {
            return Player.Gold >= RequiredGold();
        }

        internal void IncPriority()
        {
            Debug.Assert(PriorityFlag < PriorityExecution.Exclusive);

            if (Hidden)
            {
                int gold = RequiredGold();// На всякий случай запоминаем точное значение. вдруг потом при трате что-нибудь поменяется
                Player.SpendGold(gold);
                SpendedGoldForSetFlag = gold;

                if (DaySetFlag == 0)
                    DaySetFlag = Player.Lobby.Turn;
                PriorityFlag++;


                //Debug.Assert()
            }
            else
            {
            }

            Program.formMain.UpdateTarget(this);
        }

        internal int Cashback()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag > 0);
            Debug.Assert(DaySetFlag > 0);

            return DaySetFlag == Player.Lobby.Turn ? SpendedGoldForSetFlag : 0;
        }

        internal void CancelFlag()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag > 0);
            Debug.Assert(DaySetFlag > 0);


            Player.ReturnGold(Cashback());
            SpendedGoldForSetFlag = 0;
            DaySetFlag = 0;
            PriorityFlag = PriorityExecution.None;
        }

        internal string PriorityFlatToText()
        {
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
                    return "Экслюзивный";
                default:
                    throw new Exception("Неизвестный приоритет: " + PriorityFlag.ToString());
            }
        }
    }
}
