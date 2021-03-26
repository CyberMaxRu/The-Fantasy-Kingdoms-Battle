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

            Name = l.Name;
            ImageIndexAvatar = l.ImageIndex;
            TypePlayer = TypePlayer.Lair;

            // Убрать эту проверку после настройки всех логов
            if (TypeLair.Monsters.Count > 0)
                CreateMonsters();
        }

        internal Player Player { get; }
        internal TypeLair TypeLair { get; }
        internal int Layer { get; }// Слой, на котором находится логово
        internal bool Hidden { get; set; } = true;// Логово не разведано
        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня

        // Поддержка флага
        internal int DaySetFlag { get; private set; }// День установки флага
        internal int SpendedGoldForSetFlag { get; private set; }// Сколько золота было потрачено на установку флага
        internal PriorityExecution PriorityFlag { get; private set; } = PriorityExecution.None;// Приоритет разведки/атаки
        internal List<PlayerHero> listAttackedHero { get; } = new List<PlayerHero>();// Список героев, откликнувшихся на флаг

        private void CreateMonsters()
        {
            //Debug.Assert(TypeLair.Monsters.Count > 0);

            Monster lm;
            foreach (MonsterLevelLair mll in TypeLair.Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new Monster(mll.Monster, mll.Level, this);
                    Monsters.Add(lm);
                    AddCombatHero(lm);
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
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostScout * Player.Lobby.TypeLobby.CoefFlagScout[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal int CostAttack()
        {
            Debug.Assert(!Hidden);

            return PriorityFlag < PriorityExecution.Exclusive ?
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostAttack * Player.Lobby.TypeLobby.CoefFlagAttack[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal override void PrepareHint()
        {
            if (Hidden)
                Program.formMain.formHint.AddStep1Header("Неизвестное место", "Место не разведано", "Установите флаг разведки для отправки героев к месту");
            else
                Program.formMain.formHint.AddStep1Header(TypeLair.Name, "", TypeLair.Description);
        }

        internal string NameLair()
        {
            return Hidden ? "Неизвестное место" : TypeLair.Name;
        }

        internal int ImageIndexLair()
        {
            return TypeLair.ImageIndex;
            return Hidden ? FormMain.IMAGE_INDEX_NONE : TypeLair.ImageIndex;
        }

        internal int RequiredGold()
        {
            return Hidden ? CostScout() : CostAttack();
        }

        internal bool CheckRequirements()
        {
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

        internal void IncPriority()
        {
            Debug.Assert(PriorityFlag < PriorityExecution.Exclusive);

            // 

            if (Hidden)
            {
                int gold = RequiredGold();// На всякий случай запоминаем точное значение. вдруг потом при трате что-нибудь поменяется
                Player.SpendGold(gold);
                SpendedGoldForSetFlag += gold;

                if (DaySetFlag == 0)
                    DaySetFlag = Player.Lobby.Turn;
                PriorityFlag++;

                //Debug.Assert()
            }
            else
            {
            }

            if (PriorityFlag == PriorityExecution.Normal)
                Player.AddFlag(this);
            else
                Player.UpPriorityFlag(this);
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
            Player.RemoveFlag(this);
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
                    return "Максимальный";
                default:
                    throw new Exception("Неизвестный приоритет: " + PriorityFlag.ToString());
            }
        }

        internal override void HideInfo()
        {
            Program.formMain.panelLairInfo.Visible = false;
        }

        internal override void ShowInfo()
        {
            Program.formMain.panelLairInfo.Visible = true;
            Program.formMain.panelLairInfo.PlayerObject = this;
        }
    }
}
