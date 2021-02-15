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
    internal sealed class PlayerHero : Creature
    {
        private VCCell panelEntity;
        public PlayerHero(PlayerBuilding pb) : base(pb.Building.TrainedHero)
        {
            Building = pb;
            DayOfHire = Player.Lobby.Turn;
            TypeHero = pb.Building.TrainedHero;

            // Берем оружие и доспехи
            MeleeWeapon = TypeHero.WeaponMelee;
            RangeWeapon = TypeHero.WeaponRange;
            Armour = TypeHero.Armour;
        }

        internal PlayerBuilding Building { get; }// Здание, которому принадлежит герой
        internal Player Player => Building.Player;// Игрок, которому принадлежит герой
        internal TypeHero TypeHero { get; } // Класс героя

        // Основные параметры
        internal Weapon MeleeWeapon { get; private set; }// Рукопашное оружие 
        internal Weapon RangeWeapon { get; private set; }// Стрелковое оружие 
        internal Armour Armour { get; private set; }// Доспех
        internal Point CoordInPlayer { get; set; }// Координаты героя в слотах игрока

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

        // Увольнение героя
        internal void Dismiss()
        {
            Debug.Assert(Building.Heroes.IndexOf(this) != -1);
            Debug.Assert(Building.Player.CombatHeroes.IndexOf(this) != -1);

            Building.Heroes.Remove(this);
            Building.Player.CombatHeroes.Remove(this);

            Debug.Assert(Building.Heroes.IndexOf(this) == -1);
            Debug.Assert(Building.Player.CombatHeroes.IndexOf(this) == -1);
        }

        internal int FindSlotWithItem(Item item)
        {
            // Сначала ищем слот, заполненный таким же предметом
            for (int i = 0; i < Inventory.Count; i++)
                if (Inventory[i].Item == item)
                    return i;

            return -1;
        }

        internal int FindCellForItem(Item item)
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

        internal void AcceptItem(PlayerItem pi, int quantity)
        {
            Debug.Assert(pi.Quantity > 0);
            Debug.Assert(quantity > 0);
            Debug.Assert(pi.Quantity >= quantity);

            int toCell = FindCellForItem(pi.Item);
            if (toCell == -1)
                return;

            AcceptItem(pi, quantity, toCell);
        }

        internal void AcceptItem(PlayerItem pi, int quantity, int toCell)
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
                        if (Building.Player.GetItemFromHero(this, toCell) == true)
                            Slots[toCell] = null;
                        else
                            return;
                    }
                }

                // Если разный тип предметов, то пытаемся поместить предмет на склад
                if ((Slots[toCell] != null) && (Slots[toCell].Item != pi.Item))
                {
                    if (Building.Player.GetItemFromHero(this, toCell) == true)
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

        internal PlayerItem TakeItem(int fromCell, int quantity)
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

            ParametersWithAmmunition.MaxMeleeDamage = MeleeWeapon.DamageMelee + (MeleeWeapon.DamageMelee * ParametersWithAmmunition.Strength / 100);
            ParametersWithAmmunition.MinMeleeDamage = ParametersWithAmmunition.MaxMeleeDamage / 2;
            if (RangeWeapon != null)
            {
                ParametersWithAmmunition.MaxArcherDamage = RangeWeapon.DamageRange + (RangeWeapon.DamageRange * ParametersWithAmmunition.Strength / 100);
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
            ParametersWithAmmunition.DefenseMelee = Armour.DefenseMelee;
            ParametersWithAmmunition.DefenseArcher = Armour.DefenseArcher;
            ParametersWithAmmunition.DefenseMagic = Armour.DefenseMagic;

            Debug.Assert((ParametersWithAmmunition.MaxMeleeDamage > 0) || (ParametersWithAmmunition.MaxArcherDamage > 0) || (ParametersWithAmmunition.MagicDamage > 0));
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

        protected override int GetImageIndex()
        {
            return Program.formMain.TreatImageIndex(TypeHero.ImageIndex, Player);
        }

        protected override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeHero.Name, "", TypeHero.Description);
        }

        protected override void DoClick()
        {
            Program.formMain.SelectHero(this);
        }

        internal int Priority()
        {
            int posInPlayer = Player.CombatHeroes.IndexOf(this);
            Debug.Assert(posInPlayer != -1);
            return TypeHero.DefaultPositionPriority * 1000 + posInPlayer;
        }
    }
}