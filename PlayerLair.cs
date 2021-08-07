using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal enum PriorityExecution { None = -1, Normal = 0, Warning = 1, High = 2, Exclusive = 3};
    internal enum TypeFlag { None, Scout, Attack, Defense };

    // Класс логова игрока
    internal sealed class PlayerLair : PlayerMapObject, ICell
    {
        public PlayerLair(LobbyPlayer p, TypePlace l, int x, int y, int layer) : base(p, l)
        {
            Player = p;
            TypeLair = l;
            X = x;
            Y = y;
            Layer = layer;

            Participant = new LairBattleParticipant(this);

            // Убрать эту проверку после настройки всех логов
            if (TypeLair.Monsters.Count > 0)
                CreateMonsters();
        }

        internal LobbyPlayer Player { get; }
        internal TypePlace TypeLair { get; }
        internal int Layer { get; }// Слой, на котором находится логово
        internal int X { get; }// Позиция по X в слое
        internal int Y { get; }// Позиция по Y в слое
        internal bool Hidden { get; private set; } = true;// Логово не разведано
        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня
        internal bool Destroyed { get; private set; } = false;// Логово уничтожено, работа с ним запрещена

        // Поддержка флага
        internal TypeFlag TypeFlag { get; private set; } = TypeFlag.None;// Тип установленного флага
        internal int DaySetFlag { get; private set; }// День установки флага
        internal int SpendedGoldForSetFlag { get; private set; }// Сколько золота было потрачено на установку флага
        internal PriorityExecution PriorityFlag { get; private set; } = PriorityExecution.None;// Приоритет разведки/атаки
        internal List<PlayerHero> listAttackedHero { get; } = new List<PlayerHero>();// Список героев, откликнувшихся на флаг
        internal LairBattleParticipant Participant { get; }

        private void CreateMonsters()
        {
            Debug.Assert(!Destroyed);
            //Debug.Assert(TypeLair.Monsters.Count > 0);

            Monster lm;
            foreach (MonsterLevelLair mll in TypeLair.Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new Monster(mll.Monster, mll.Level, Participant);
                    Monsters.Add(lm);
                    Participant.AddCombatHero(lm);
                }
            }
        }

        internal int CostScout()
        {
            Debug.Assert(Hidden);
            Debug.Assert(!Destroyed);

            return PriorityFlag < PriorityExecution.Exclusive ? 
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostScout * Player.Lobby.TypeLobby.CoefFlagScout[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal int CostAttack()
        {
            Debug.Assert(!Hidden);
            Debug.Assert(!Destroyed);

            return PriorityFlag < PriorityExecution.Exclusive ?
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostAttack * Player.Lobby.TypeLobby.CoefFlagAttack[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal int CostDefense()
        {
            Debug.Assert(!Hidden);
            Debug.Assert(!Destroyed);

            return PriorityFlag < PriorityExecution.Exclusive ?
                Player.Lobby.TypeLobby.LayerSettings[Layer].CostDefense * Player.Lobby.TypeLobby.CoefFlagDefense[(int)PriorityFlag + 1] / 100 : 0;
        }

        internal override void PrepareHint()
        {
            Debug.Assert(!Destroyed);

            if (Hidden)
                Program.formMain.formHint.AddStep1Header("Неизвестное место", "Место не разведано", "Установите флаг разведки для отправки героев к месту");
            else
            {
                Program.formMain.formHint.AddStep1Header(TypeLair.Name, "", TypeLair.Description);
                Program.formMain.formHint.AddStep2Reward(TypeLair.TypeReward.Gold);
                Program.formMain.formHint.AddStep3Greatness(TypeLair.TypeReward.Greatness, 0);
            }
        }

        internal string NameLair()
        {
            Debug.Assert(!Destroyed);
            return Hidden ? "Неизвестное место" : TypeLair.Name;
        }

        internal int ImageIndexLair()
        {
            Debug.Assert(!Destroyed);

            return Hidden ? FormMain.IMAGE_INDEX_UNKNOWN : TypeLair.ImageIndex;
        }

        internal int RequiredGold()
        {
            Debug.Assert(!Destroyed);

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

        internal bool CheckRequirements()
        {
            Debug.Assert(!Destroyed);

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
            Debug.Assert(!Destroyed);

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
            Debug.Assert(!Destroyed);

            if (Hidden)
                return TypeFlag.Scout;
            if (TypeLair.Monsters.Count > 0)
                return TypeFlag.Attack;
            return TypeFlag.Defense;
        }

        internal void IncPriority()
        {
            Debug.Assert(PriorityFlag < PriorityExecution.Exclusive);
            Debug.Assert(!Destroyed);

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
            Debug.Assert(!Destroyed);

            return DaySetFlag == Player.Lobby.Day ? SpendedGoldForSetFlag : 0;
        }

        internal void CancelFlag()
        {
            Debug.Assert(PriorityFlag != PriorityExecution.None);
            Debug.Assert(SpendedGoldForSetFlag > 0);
            Debug.Assert(DaySetFlag > 0);
            Debug.Assert(TypeFlag != TypeFlag.None);
            Debug.Assert(!Destroyed);

            Player.ReturnGold(Cashback());
            DropFlag();
        }

        internal string PriorityFlatToText()
        {
            Debug.Assert(!Destroyed);

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
            Debug.Assert(!Destroyed);

            Program.formMain.panelLairInfo.Visible = false;
        }

        internal override void ShowInfo()
        {
            Debug.Assert(!Destroyed);

            Program.formMain.panelLairInfo.Visible = true;
            Program.formMain.panelLairInfo.PlayerObject = this;
        }

        internal int MaxHeroesForFlag()
        {
            switch (TypeAction())
            {
                case TypeFlag.Scout:
                    return Player.Lobby.TypeLobby.MaxHeroesForScoutFlag;
                case TypeFlag.Attack:
                case TypeFlag.Defense:
                    return TypeLair.MaxHeroes;
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
            Debug.Assert(!Destroyed);
            Debug.Assert(listAttackedHero.Count < MaxHeroesForFlag());

            listAttackedHero.Add(ph);
            ph.TargetByFlag = this;
            ph.SetState(ph.StateForFlag(TypeFlag));
        }

        internal void RemoveAttackingHero(PlayerHero ph)
        {
            Debug.Assert(listAttackedHero.IndexOf(ph) != -1);
            Debug.Assert(ph.TargetByFlag == this);
            Debug.Assert(!Destroyed);

            ph.TargetByFlag = null;
            listAttackedHero.Remove(ph);
            ph.SetState(NameStateCreature.Nothing);
        }

        private void DropFlag()
        {
            Debug.Assert(TypeFlag != TypeFlag.None);
            Debug.Assert(!Destroyed);

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
            Debug.Assert(!Destroyed);

            Hidden = false;

            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            HandOutGoldHeroes();

            DropFlag();
        }

        // Логово захвачено
        internal void DoCapture()
        {
            Debug.Assert(!Hidden);
            Debug.Assert(TypeFlag == TypeFlag.Attack);
            Debug.Assert(!Destroyed);
            Debug.Assert(listAttackedHero.Count > 0);

            // Раздаем награду. Открыть место могли без участия героев (заклинанием)
            HandOutGoldHeroes();

            DropFlag();

            // Убираем себя из списка логов игрока
            Player.RemoveLair(this);
            Destroyed = true;

            // Ставим тип места, который должен быть после зачистки
            Debug.Assert(!(TypeLair.TypePlaceAfterClear is null));

            PlayerLair pl = new PlayerLair(Player, TypeLair.TypePlaceAfterClear, X, Y, Layer);
            pl.Hidden = false;
            Player.Lairs[Layer, Y, X] = pl;
        }

        internal void DoDefense()
        {
            Debug.Assert(!Hidden);
            Debug.Assert(TypeFlag == TypeFlag.Defense);
            Debug.Assert(!Destroyed);
            Debug.Assert(listAttackedHero.Count > 0);

            // Раздаем награду
            HandOutGoldHeroes();

            DropFlag();

            // Убираем себя из списка логов игрока
            Player.RemoveLair(this);
            Player.ApplyReward(this);

            Destroyed = true;
        }

        internal void MonsterIsDead(Monster m)
        {
            Debug.Assert(m != null);
            Debug.Assert(m.BattleParticipant == Participant);
            Debug.Assert(Monsters.IndexOf(m) != -1);

            m.SetIsDead();
            Participant.CombatHeroes.Remove(m);
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

        internal override bool CheckRequirementsForResearch(PlayerCellMenu research)
        {
            // Потом проверяем наличие золота
            if (Player.Gold < research.Cost())
                return false;

            // Проверяем требования к исследованию
            return Player.CheckRequirements(research.Research.Requirements);
        }

        internal override List<TextRequirement> GetTextRequirements(PlayerCellMenu research)
        {
            List<TextRequirement> list = new List<TextRequirement>();

            return list;
        }

        internal override void ResearchCompleted(PlayerCellMenu research)
        {
            base.ResearchCompleted(research);
        }

        internal override bool ShowMenuForPlayer() => !Hidden;

        protected override int GetLevel()
        {
            return 0;
        }

        protected override int GetQuantity()
        {
            return 0;
        }

        BitmapList ICell.BitmapList() => Program.formMain.imListObjectsCell;
        int ICell.ImageIndex() => ImageIndexLair();        
        bool ICell.NormalImage() => true;
        int ICell.Level() => 0;
        int ICell.Quantity() => 0;
        void ICell.PrepareHint() => PrepareHint();
        
        void ICell.Click(VCCell pe)
        {
            Program.formMain.SelectPlayerObject(this);
        }

        void ICell.CustomDraw(Graphics g, int x, int y, bool drawState) { }
    }
}
