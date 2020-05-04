using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс героя игрока
    internal sealed class PlayerHero
    {
        public PlayerHero(PlayerGuild pg)
        {
            Guild = pg;
            Hero = Guild.Guild.TrainedHero;
            Level = 1;

            for (int i = 0; i < Hero.Slots.Length; i++)
            {
                if (Hero.Slots[i].DefaultItem != null)
                {
                    Slots[i] = new PlayerItem(Hero.Slots[i].DefaultItem, 1);
                }
            }
        }

        internal PlayerGuild Guild { get; }        
        internal Hero Hero { get; }
        internal int Level { get; }

        internal int CurrentHealth;
        internal int MaxHealth;
        internal int CurrentMana;
        internal int MaxMana;
        internal Parameters OurParameters { get; } = new Parameters();
        internal Parameters ModifiedParameters { get; } = new Parameters();
        internal PlayerItem[] Slots { get; } = new PlayerItem[FormMain.SLOT_IN_INVENTORY];
        internal PanelHero Panel { get; set; }

        internal void ShowDate()
        {
            Debug.Assert(Panel != null);

            Panel.ShowData();
        }

        internal void Dismiss()
        {
            Debug.Assert(Guild.Heroes.IndexOf(this) != -1);
            Debug.Assert(Guild.Player.Heroes.IndexOf(this) != -1);

            if (Guild.Heroes.Remove(this) == false)
                throw new Exception("Не смог удалить себя из списка героев гильдии.");

            if (Guild.Player.Heroes.Remove(this) == false)
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
                        if (Guild.Player.GetItemFromHero(this, toCell) == true)
                            Slots[toCell] = null;
                        else
                            return;
                    }
                }

                // Если разный тип предметов, то пытаемся поместить предмет на склад
                if ((Slots[toCell] != null) && (Slots[toCell].Item != pi.Item))
                {
                    if (Guild.Player.GetItemFromHero(this, toCell) == true)
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
    }
}
