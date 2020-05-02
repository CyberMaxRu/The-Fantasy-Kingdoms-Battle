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
    }
}
