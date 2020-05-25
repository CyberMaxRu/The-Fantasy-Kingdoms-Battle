using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum StateHeroInBattle { Melee, Shoot, Cast, Drink, Healing, Rest, Dead, Resurrection, None }// Состояние героя в бою

    // Класс героя игрока
    internal sealed class PlayerHero
    {
        public PlayerHero(PlayerBuilding pb)
        {
            Building = pb;
            Player = pb.Player;
            Hero = Building.Building.TrainedHero;

            IsLive = true;

            if (Hero.MaxLevel > 1)
            {
                Level = 0;

                if (Hero.Slots.Length > 0)
                {
                    for (int i = 0; i < Hero.Slots.Length; i++)
                    {
                        if ((Hero.Slots[i] != null) && (Hero.Slots[i].DefaultItem != null))
                        {
                            Slots[i] = new PlayerItem(Hero.Slots[i].DefaultItem, 1);
                        }
                    }
                }

                ParametersBase = new HeroParameters(Hero.BaseParameters);

                // Переходим на 1 уровень
                LevelUp();
                ParametersWithAmmunition = new HeroParameters(ParametersBase);

                FindWeaponAndArmour();
                UpdateBaseParameters();
            }
            else
                Level = 1;

        }

        internal Player Player { get; }
        internal PlayerBuilding Building { get; }
        internal Hero Hero { get; }
        internal int Level { get; private set; }

        internal HeroParameters ParametersBase { get; }// Свои параметры, без учета амуниции
        internal HeroParameters ParametersWithAmmunition { get; }// Параметры с учетом амуниции
        internal HeroParameters ParametersInBattle { get; private set; }// Параметры во время боя, с учетом всех боевых перков, баффов и прочего
        internal PlayerItem[] Slots { get; } = new PlayerItem[FormMain.SLOT_IN_INVENTORY];
        internal PlayerItem Weapon { get; private set; }// Оружие 
        internal PlayerItem Armour { get; private set; }// Доспех
        internal PanelHero Panel { get; set; }
        internal bool IsLive { get; private set; }

        // Параметры во время боя
        internal Point CoordInPlayer { get; set; }
        internal StateHeroInBattle State { get; private set; }
        internal PlayerHero Target { get; private set; }

        // Статистика за лобби
        internal int Battles { get; }// Участвовал в сражениях
        internal int Wins { get; }// Побед        
        internal int Loses { get; }// Поражений
        internal int Draws { get; }// Ничьих
        internal int DoDamages { get; }// Нанес урона
        internal int DoKills { get; }// Убил героев противника
        internal int Dies { get; }// Сколько раз был убит
        internal int WinStrike { get; }// Побед подряд
        internal int LoseStrike { get; }// Поражений подряд
        internal int DrawStrike { get; }// Ничьих подряд

        // Параметры во время боя

        internal void ShowDate()
        {
            Debug.Assert(Panel != null);

            Panel.ShowData(this);
        }

        internal void Dismiss()
        {
            Debug.Assert(Building.Heroes.IndexOf(this) != -1);
            Debug.Assert(Building.Player.Heroes.IndexOf(this) != -1);

            if (Building.Heroes.Remove(this) == false)
                throw new Exception("Не смог удалить себя из списка героев гильдии.");

            if (Building.Player.Heroes.Remove(this) == false)
                throw new Exception("Не смог удалить себя из списка героев игрока.");

            if (Panel != null)
                Panel.Dispose();
        }

        internal int FindSlotWithItem(Item item)
        {
            // Сначала ищем слот, заполненный таким же предметом
            for (int i = 0; i < Slots.Length; i++)
            {
                if ((Slots[i] != null) && (Slots[i].Item == item))
                    return i;
            }

            return -1;
        }

        internal int FindCellForItem(Item item)
        {
            int number = FindSlotWithItem(item);
            if (number != -1)
                return number;

            // Ищем пустой слот, разрешенный для такого типа предметов
            for (int i = 0; i < Slots.Length; i++)
            {
                if (item.TypeItem.Single == true)
                {
                    if (Hero.Slots[i].TypeItem == item.TypeItem)
                        return i;
                }
                else if ((Slots[i] == null) && (Hero.Slots[i].TypeItem == item.TypeItem))
                    return i;
            }

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
            if (pi.Item.TypeItem != Hero.Slots[toCell].TypeItem)
                return;

            if (Slots[toCell] != null)
            {
                if (Hero.Slots[toCell].DefaultItem != null)
                {
                    // Если это дефолтный предмет, удаляем его
                    if (Slots[toCell].Item == Hero.Slots[toCell].DefaultItem)
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
                Slots[toCell] = new PlayerItem(pi.Item, Math.Min(Hero.Slots[toCell].MaxQuantity, quantity));
                pi.Quantity -= Slots[toCell].Quantity;
            }
            else
            {
                int add = Math.Min(Hero.Slots[toCell].MaxQuantity - Slots[toCell].Quantity, quantity);
                if (add > 0)
                {
                    Slots[toCell] = new PlayerItem(pi.Item, add);
                    pi.Quantity -= add;
                }
            }

            Debug.Assert(Slots[toCell] != null);

            switch (pi.Item.TypeItem.Category)
            {
                case CategoryItem.Weapon:
                    Weapon = pi;
                    break;
                case CategoryItem.Armour:
                    Armour = pi;
                    break;
            }
        }

        internal PlayerItem TakeItem(int fromCell, int quantity)
        {
            Debug.Assert(quantity > 0);
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
                pi = new PlayerItem(Slots[fromCell].Item, quantity);
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

            return pi;
        }

        internal void ValidateCell(int number)
        {
            if ((Hero.Slots[number].DefaultItem != null) && (Slots[number] == null))
            {
                Slots[number] = new PlayerItem(Hero.Slots[number].DefaultItem, 1);
            }
        }

        internal void MoveItem(int fromSlot, int toSlot)
        {
            Debug.Assert(Slots[fromSlot] != null);
            Debug.Assert(fromSlot != toSlot);

            if (Slots[fromSlot].Item.TypeItem == Hero.Slots[toSlot].TypeItem)
            {
                PlayerItem tmp = null;
                if (Slots[toSlot] != null)
                    tmp = Slots[toSlot];
                Slots[toSlot] = Slots[fromSlot];
                Slots[fromSlot] = tmp;
            }
        }

        private void LevelUp()
        {
            Debug.Assert(Level < Hero.MaxLevel);

            // Прибавляем безусловные параметры
            if (Hero.ConfigNextLevel != null)
            {
                ParametersBase.Health += Hero.ConfigNextLevel.Health;
                ParametersBase.Mana += Hero.ConfigNextLevel.Mana;

                // Прибавляем очки характеристик
                int t;
                for (int i = 0; i < Hero.ConfigNextLevel.StatPoints; i++)
                {
                    t = FormMain.Rnd.Next(100);

                    if (t < Hero.ConfigNextLevel.WeightStrength)
                        ParametersBase.Strength++;
                    else if (t < Hero.ConfigNextLevel.WeightStrength + Hero.ConfigNextLevel.WeightDexterity)
                        ParametersBase.Dexterity++;
                    else if (t < Hero.ConfigNextLevel.WeightStrength + Hero.ConfigNextLevel.WeightDexterity + Hero.ConfigNextLevel.WeightMagic)
                        ParametersBase.Magic++;
                    else
                        ParametersBase.Vitality++;
                }
            }

            Level++;
        }

        // Подготовка к сражению
        internal void PrepareToBattle()
        {
            Debug.Assert(Target == null);

            ParametersInBattle = new HeroParameters(ParametersWithAmmunition);
            State = StateHeroInBattle.None;
        }

        // Делает шаг битвы
        internal void DoStepBattle(Battle b)
        {
            Debug.Assert(IsLive == true);

            switch (State)
            {
                case StateHeroInBattle.None:
                    Debug.Assert(Target == null);

                    // Если сейчас ничего не выполняем, ищем, что можно сделать
                    // Сначала атакуем
                    if (SearchTargetForMelee() == false)
                    {

                    }

                    break;
                case StateHeroInBattle.Melee:
                    break;
                default:
                    break;
            }

            bool SearchTargetForMelee()
            {
                Debug.Assert(Hero.CategoryHero == CategoryHero.Melee);

                // Ищем, кого атаковать
                List<PlayerHero> targets = new List<PlayerHero>();

                foreach (PlayerHero h in b.Heroes)
                {
                    // Собираем список вражеских героев вокруг себя
                    if (h.Player != Player)
                        if (IsNeighbour(h) == true)
                            targets.Add(h);
                }

                if (targets.Count > 0)
                {
                    Debug.Assert(this != targets[0]);
                    Target = targets[0];
                    State = StateHeroInBattle.Melee;

                    return true;
                }
                else
                    return false;
            }

            bool IsNeighbour(PlayerHero ph)
            {
                Debug.Assert(this != ph);

                return Utils.PointsIsNeighbor(this.ParametersInBattle.Coord, ph.ParametersInBattle.Coord);
            }
        }

        // Применяем шаг битвы
        internal void ApplyStepBattle()
        {

        }

        internal void UpdateBaseParameters()
        {
            ParametersBase.Health = (ParametersBase.Vitality * ParametersBase.CoefHealth) + (Level * Hero.ConfigNextLevel.Health);
            ParametersBase.Mana = (ParametersBase.Magic * ParametersBase.CoefMana) + (Level * Hero.ConfigNextLevel.Mana);
            ParametersBase.Stamina = (ParametersBase.Vitality * ParametersBase.CoefStamina) + (Level * Hero.ConfigNextLevel.Stamina);

            UpdateParamsWithAmmunition();
        }

        internal void UpdateParamsWithAmmunition()
        {
            // Копируем базовые параметры
            ParametersWithAmmunition.GetFromParams(ParametersBase);

            // Применяем амуницию
            Debug.Assert(Weapon != null);
            Debug.Assert(Armour != null);

            ParametersWithAmmunition.MaxMeleeDamage = Weapon.Item.DamageMelee + (Weapon.Item.DamageMelee * ParametersWithAmmunition.Strength / 100);
            ParametersWithAmmunition.MinMeleeDamage = ParametersWithAmmunition.MaxMeleeDamage / 2;
            ParametersWithAmmunition.MaxMissileDamage = Weapon.Item.DamageMissile + (Weapon.Item.DamageMissile * ParametersWithAmmunition.Strength / 100);
            ParametersWithAmmunition.MinMissileDamage = ParametersWithAmmunition.MaxMissileDamage / 2;
            ParametersWithAmmunition.MagicDamage = (ParametersWithAmmunition.Magic / 5) * Weapon.Item.DamageMagic + Level;
            ParametersWithAmmunition.DefenseMelee = Armour.Item.DefenseMelee;
            ParametersWithAmmunition.DefenseMissile = Armour.Item.DefenseMissile;
            ParametersWithAmmunition.DefenseMagic = Armour.Item.DefenseMagic;

            Debug.Assert((ParametersWithAmmunition.MaxMeleeDamage > 0) || (ParametersWithAmmunition.MaxMissileDamage > 0) || (ParametersWithAmmunition.MagicDamage > 0));
        }

        internal void UpdateParamsInBattle()
        {

        }

        internal void FindWeaponAndArmour()
        {
            Weapon = null;
            Armour = null;

            foreach (PlayerItem pi in Slots)
            {
                if (pi != null)
                {
                    switch (pi.Item.TypeItem.Category)
                    {
                        case CategoryItem.Weapon:
                            Debug.Assert(Weapon == null);
                            Weapon = pi;
                            break;
                        case CategoryItem.Armour:
                            Debug.Assert(Armour == null);
                            Armour = pi;
                            break;
                    }
                }
            }
        }
    }
}
