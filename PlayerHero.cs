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

        internal int FindSlotForItem(PlayerItem pi)
        {
            // Сначала ищем слот, заполненный таким же предметом, потом по типу предмета
            for (int i = 0; i < Slots.Length; i++)
            {
                if ((Slots[i] != null) && (Slots[i].Item == pi.Item))
                    return i;
            }

            for (int i = 0; i < Slots.Length; i++)
            {
                if ((Slots[i] == null) && (Hero.Slots[i].TypeItem == pi.Item.TypeItem))
                    return i;
            }

            return -1;
        }

        internal bool TryAcceptItem(PlayerItem pi)
        {
            Debug.Assert(pi.Quantity > 0);

            int slot = FindSlotForItem(pi);
            if (slot == -1)
                return false;

            return TryAcceptItem(pi, slot);
        }

        internal bool TryAcceptItem(PlayerItem pi, int toSlot)
        {
            Debug.Assert(pi.Quantity > 0);

            // Проверяем совместимость
            if (pi.Item.TypeItem != Hero.Slots[toSlot].TypeItem)
                return false;

            if (Slots[toSlot] != null)
            {
                if (Hero.Slots[toSlot].DefaultItem != null)
                {
                    // Если это дефолтный предмет, удаляем его
                    if (Slots[toSlot].Item == Hero.Slots[toSlot].DefaultItem)
                        Slots[toSlot] = null;
                    else
                    {
                        // Если не можем поместить вещь на склад, выходим
                        if (Guild.Player.GetItemFromHero(this, toSlot) == false)
                            return false;
                    }
                }

                Slots[toSlot] = pi;
            }
            else
                Slots[toSlot] = pi;

            return true;
        }
    }
}
