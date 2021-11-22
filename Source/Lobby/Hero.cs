using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Класс героя игрока
    internal sealed class Hero : Creature
    {
        public Hero(Construction pb, BattleParticipant bp, DescriptorCreature creature) : base(creature, bp)
        {
            Construction = pb;
            Abode = Construction;
            DayOfHire = Player.Lobby.Turn;

            if (creature.CategoryCreature == CategoryCreature.Hero)
            {
                Debug.Assert(Food != null);
                Debug.Assert(Rest != null);
                Debug.Assert(Entertainment != null);
                Debug.Assert(Money != null);

                FullName = (creature.PrefixName.Length > 0 ? creature.PrefixName + " " : "")
                    + GetRandomName(creature.NameFromTypeHero == null ? creature.Names : creature.NameFromTypeHero.Names)
                    + " " + GetRandomName(creature.SurnameFromTypeHero == null ? creature.Surnames : creature.Surnames);
            }
            else
            {
                FullName = TypeCreature.Name;
            }

            //
            Initialize();

            string GetRandomName(List<string> list)
            {
                return list.Count > 0 ? list[Player.Lobby.Rnd.Next(list.Count)] : "";
            }
        }

        internal Construction Construction { get; }// Здание, которому принадлежит герой
        internal Construction Abode { get; private set; }// Текущая обитель героя
        internal Construction NeedMoveToAbode { get; set; }// Героя необходимо перенести в новую обитель

        internal Player Player => Construction.Player;// Игрок, которому принадлежит герой
        internal string FullName { get; }// Полное имя
        internal int Gold { get; private set; }// Количество золота у героя


        // Выполнение флагов
        internal Construction TargetByFlag { get; set; }// Логово флага, который выполняется

        // Статистика за лобби
        internal int DayOfHire { get; }// На каком дне нанят
        internal int Battles { get; }// Участвовал в сражениях
        internal int Wins { get; }// Побед        
        internal int Loses { get; }// Поражений
        internal int Draws { get; }// Ничьих
        internal int DoDamages { get; }// Нанес урона
        internal int DoKills { get; }// Убил героев противника
        internal int Dies { get; }// Сколько раз был убит
        internal int WinStreak { get; }// Побед подряд
        internal int LoseStreak { get; }// Поражений подряд
        internal int DrawStreak { get; }// Ничьих подряд
        internal ResultBattle PriorResultBattle { get; set; }// Предыдущий результат битвы для расчета страйков

        //internal bool Selected { get; set; }

        // Увольнение героя
        internal void Dismiss()
        {
            Debug.Assert(Construction.Heroes.IndexOf(this) != -1);
            Debug.Assert(Construction.Player.CombatHeroes.IndexOf(this) != -1);

            Construction.Heroes.Remove(this);
            Construction.Player.CombatHeroes.Remove(this);

            Debug.Assert(Construction.Heroes.IndexOf(this) == -1);
            Debug.Assert(Construction.Player.CombatHeroes.IndexOf(this) == -1);
        }

        internal int FindSlotWithItem(DescriptorItem item)
        {
            // Сначала ищем слот, заполненный таким же предметом
            for (int i = 0; i < Inventory.Count; i++)
                if (Inventory[i].Descriptor == item)
                    return i;

            return -1;
        }

        internal int FindCellForItem(DescriptorItem item)
        {
            int number = FindSlotWithItem(item);
            if (number != -1)
                return number;

            // Ищем пустой слот, разрешенный для такого типа предметов
            //for (int i = 0; i < Inventory.Count; i++)
            //    if (Inventory[i].Item == item)
            //        return i;

            return -1;
        }

        internal void AcceptItem(Item pi, int quantity)
        {
            Debug.Assert(pi.Quantity > 0);
            Debug.Assert(quantity > 0);
            Debug.Assert(pi.Quantity >= quantity);

            int toCell = FindCellForItem(pi.Descriptor);
            if (toCell == -1)
                return;

            AcceptItem(pi, quantity, toCell);
        }

        internal void AcceptItem(Item pi, int quantity, int toCell)
        {
            Debug.Assert(pi.Quantity > 0);
            Debug.Assert(quantity > 0);
            Debug.Assert(pi.Quantity >= quantity);

            // Проверяем совместимость
            /*            if (pi.Item.TypeItem != ClassHero.Slots[toCell].TypeItem)
                            return;

                        if (Slots[toCell] != null)
                        {
                            if (ClassHero.Slots[toCell].DefaultItem != null)
                            {
                                // Если это дефолтный предмет, удаляем его
                                if (Slots[toCell].Item == ClassHero.Slots[toCell].DefaultItem)
                                    Slots[toCell] = null;
                                else
                                {
                                    // Иначе помещаем предмет на склад
                                    // Если не можем поместить вещь на склад, выходим
                                    if (Construction.Player.GetItemFromHero(this, toCell) == true)
                                        Slots[toCell] = null;
                                    else
                                        return;
                                }
                            }

                            // Если разный тип предметов, то пытаемся поместить предмет на склад
                            if ((Slots[toCell] != null) && (Slots[toCell].Item != pi.Item))
                            {
                                if (Construction.Player.GetItemFromHero(this, toCell) == true)
                                    Slots[toCell] = null;
                                else
                                    return;
                            }
                        }

                        if (Slots[toCell] == null)
                        {
                            Slots[toCell] = new PlayerItem(pi.Item, Math.Min(ClassHero.MaxQuantityTypeItem(ClassHero.Slots[toCell].TypeItem), quantity), false);
                            pi.Quantity -= Slots[toCell].Quantity;
                        }
                        else
                        {
                            int add = Math.Min(ClassHero.MaxQuantityTypeItem(ClassHero.Slots[toCell].TypeItem) - Slots[toCell].Quantity, quantity);
                            if (add > 0)
                            {
                                Slots[toCell] = new PlayerItem(pi.Item, add, false);
                                pi.Quantity -= add;
                            }
                        }

                        Debug.Assert(Slots[toCell] != null);*/
        }

        internal Item TakeItem(int fromCell, int quantity)
        {
            return null;
            /*Debug.Assert(quantity > 0);
            Debug.Assert(Slots[fromCell] != null);
            Debug.Assert(Slots[fromCell].Quantity > 0);
            Debug.Assert(Slots[fromCell].Quantity >= quantity);

            PlayerItem pi;

            // Если забирают всё, то возвращаем ссылку на этот предмет и убираем его у себя, иначе делим предмет
            if (Slots[fromCell].Quantity == quantity)
            {
                pi = Slots[fromCell];
                Slots[fromCell] = null;

                ValidateCell(fromCell);
            }
            else
            {
                pi = new PlayerItem(Slots[fromCell].Item, quantity, false);
                Slots[fromCell].Quantity -= quantity;
            }

            switch (pi.Item.TypeItem.Category)
            {
                case CategoryItem.Weapon:
                    Weapon = null;
                    break;
                case CategoryItem.Armour:
                    Armour = null;
                    break;
            }

            return pi;*/
        }
        protected override void UpdateParamsWithAmmunition()
        {
            base.UpdateParamsWithAmmunition();

            // Применяем амуницию
            if ((MeleeWeapon == null) || (Armour == null))
                return;
            Debug.Assert(MeleeWeapon != null);
            Debug.Assert(Armour != null);

            ParametersWithAmmunition.MaxMeleeDamage = MeleeWeapon.Descriptor.DamageMelee + (MeleeWeapon.Descriptor.DamageMelee * ParametersWithAmmunition.Strength / 100);
            ParametersWithAmmunition.MinMeleeDamage = ParametersWithAmmunition.MaxMeleeDamage / 2;
            if (RangeWeapon != null)
            {
                ParametersWithAmmunition.MaxArcherDamage = RangeWeapon.Descriptor.DamageRange + (RangeWeapon.Descriptor.DamageRange * ParametersWithAmmunition.Strength / 100);
                ParametersWithAmmunition.MinArcherDamage = ParametersWithAmmunition.MaxArcherDamage / 2;
            }
            else
            {
                ParametersWithAmmunition.MaxArcherDamage = 0;
                ParametersWithAmmunition.MinArcherDamage = 0;
            }
            /*            if (Weapon.DamageMagic > 0)
                            ParametersWithAmmunition.MagicDamage = (ParametersWithAmmunition.Magic / 5) * Weapon.DamageMagic + Level;
                        else
                            ParametersWithAmmunition.MagicDamage = 0;*/
            ParametersWithAmmunition.DefenseMelee = Armour.Descriptor.DefenseMelee;
            ParametersWithAmmunition.DefenseArcher = Armour.Descriptor.DefenseRange;
            ParametersWithAmmunition.DefenseMagic = Armour.Descriptor.DefenseMagic;

            //Debug.Assert((ParametersWithAmmunition.MaxMeleeDamage > 0) || (ParametersWithAmmunition.MaxArcherDamage > 0) || (ParametersWithAmmunition.MagicDamage > 0));
        }

        internal void ValidateCell(int number)
        {
            //if ((ClassHero.Slots[number].DefaultItem != null) && (Slots[number] == null))
            //{
            //    Slots[number] = new PlayerItem(ClassHero.Slots[number].DefaultItem, 1, false);
            //}
        }

        internal void MoveItem(int fromSlot, int toSlot)
        {
            /*Debug.Assert(Slots[fromSlot] != null);
            Debug.Assert(fromSlot != toSlot);

            if (Slots[fromSlot].Item.TypeItem == ClassHero.Slots[toSlot].TypeItem)
            {
                PlayerItem tmp = null;
                if (Slots[toSlot] != null)
                    tmp = Slots[toSlot];
                Slots[toSlot] = Slots[fromSlot];
                Slots[fromSlot] = tmp;
            }*/
        }

        internal override int GetImageIndex()
        {
            //Debug.Assert(IsLive);

            return Program.formMain.TreatImageIndex(TypeCreature.ImageIndex, Player);
        }

        internal override bool GetNormalImage() => IsLive;

        internal string GetNameHero()
        {
            Debug.Assert(FullName != null);
            Debug.Assert(FullName.Length > 0);

            return TypeCreature.ImageIndex != FormMain.IMAGE_INDEX_CURRENT_AVATAR ? FullName : Player.GetName();
        }

        internal override void PrepareHint()
        {
            if (IsLive)
            {
                Program.formMain.formHint.AddStep1Name(GetNameHero());
                Program.formMain.formHint.AddStep2Header($"{TypeCreature.Name} ({TypeCreature.TypeCreature.Name})");
                Program.formMain.formHint.AddStep4Level($"Уровень {Level}");
                Program.formMain.formHint.AddStep5Description(TypeCreature.Description);
            }
            else
            {
                Program.formMain.formHint.AddStep1Name(GetNameHero());
                Program.formMain.formHint.AddStep2Header($"{TypeCreature.Name} ({TypeCreature.TypeCreature.Name})");
                Program.formMain.formHint.AddStep4Level($"Уровень {Level}");
                Program.formMain.formHint.AddStep5Description($"День смерти: {DayOfDeath}{Environment.NewLine}{NameReasonOfDeath()}");
            }
        }

        protected override void DoCustomDraw(Graphics g, int x, int y, bool drawState)
        {
            base.DoCustomDraw(g, x, y, drawState);

            if (drawState && (Construction.TypeConstruction.ID != "Castle"))
                Program.formMain.ilStateHero.DrawImage(g, StateCreature.ImageIndex, true, false, x - 7, y - 3);
        }

        internal override void HideInfo()
        {
            Program.formMain.panelHeroInfo.Visible = false;
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Debug.Assert(IsLive);

            Program.formMain.panelHeroInfo.Visible = true;
            Program.formMain.panelHeroInfo.Entity = this;
        }

        internal void ClearState()
        {
            Debug.Assert(IsLive);
            Debug.Assert((StateCreature.ID == NameStateCreature.DoAttackFlag.ToString())
                || (StateCreature.ID == NameStateCreature.DoDefenseFlag.ToString())
                || (StateCreature.ID == NameStateCreature.DoScoutFlag.ToString())
                || (StateCreature.ID == NameStateCreature.BattleWithPlayer.ToString()));
            Debug.Assert(TargetByFlag != null);

            // Убираем себя из флага на логове
            TargetByFlag.RemoveAttackingHero(this);
            SetState(NameStateCreature.Nothing);
        }

        internal NameStateCreature StateForFlag(TypeFlag typeFlag)
        {
            Debug.Assert(IsLive);

            switch (typeFlag)
            {
                case TypeFlag.None:
                    return NameStateCreature.Nothing;
                case TypeFlag.Scout:
                    return NameStateCreature.DoScoutFlag;
                case TypeFlag.Attack:
                    return NameStateCreature.DoAttackFlag;
                case TypeFlag.Defense:
                    return NameStateCreature.DoDefenseFlag;
                case TypeFlag.Battle:
                    return NameStateCreature.BattleWithPlayer;
                default:
                    throw new Exception($"Неизвестный тип флага: {typeFlag}");
            }
        }

        internal void AddGold(int income)
        {
            Debug.Assert(IsLive);
            Debug.Assert(income > 0);

            Gold += income;
        }

        internal void Initialize()
        {

            // Считаем энтузиазм и лояльность
            PerksChanged();
        }

        internal override void PrepareTurn()
        {
            base.PrepareTurn();

            Initialize();
        }

        internal void PrepareQueueShopping(List<UnitOfQueueForBuy> queue)
        {
            Debug.Assert(IsLive);

            foreach (PriorityConstructionForShopping pc in TypeCreature.PriorityConstructionForShoppings)
            {
                queue.Add(new UnitOfQueueForBuy(this, Player.FindConstruction(pc.Descriptor.ID), (int)pc.Priority));
            }           
        }

        internal void DoShopping(Construction c)
        {
            bool shopped = false;
            bool abilityBought = false;

            // Получаем список доступных покупок
            List<ConstructionProduct> listProducts = c.GetProducts(TypeCreature);

            // Покупаем все, что можем
            /*foreach (ConstructionProduct cp in listProducts)
            {
                if (cp.DescriptorAbility != null)
                {
                    if (!AbilityExists(cp.DescriptorAbility))
                    {
                        AddAbility(c.PurchaseAbility(this, cp));
                        abilityBought = true;
                    }
                }
            }*/

            if (abilityBought)
                SortAbilities();
        }

        internal override void SetIsDead(ReasonOfDeath reason)
        {
            base.SetIsDead(reason);

            Debug.Assert(Abode != null);
            Debug.Assert(Abode.Heroes.IndexOf(this) != -1);

            // Перемещаем героя из его сооружения на кладбище
            NeedMoveToAbode = Player.Graveyard;

            Player.AddNoticeForPlayer(this, TypeNoticeForPlayer.HeroIsDead);
        }

        internal string NameReasonOfDeath()
        {
            Debug.Assert(!IsLive);

            switch (ReasonOfDeath)
            {
                case ReasonOfDeath.InBattle:
                    return "Смерть в бою";
                case ReasonOfDeath.Hunger:
                    return "Смерть от голода";
                case ReasonOfDeath.Emaciation:
                    return "Смерть от истощения";
                case ReasonOfDeath.Boredom:
                    return "Суицид от скуки";
                case ReasonOfDeath.Poverty:
                    return "Суицид от нищеты";
                default:
                    throw new Exception($"Неизвестная причина смерти: {ReasonOfDeath}.");
            }
        }
    }
}