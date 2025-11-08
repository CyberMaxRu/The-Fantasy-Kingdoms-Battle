using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Компонет - объект карты
    internal sealed class ComponentObjectOfMap
    {
        public ComponentObjectOfMap(BigEntity owner, bool visible)
        {
            Debug.Assert(owner != null);
            Owner = owner;
            Visible = visible;
        }

        internal BigEntity Owner { get; }
        internal bool Visible { get; set; }
        internal List<Creature> ListHeroesForFlag { get; } = new List<Creature>();// Список существ, выполняющих флаг

        internal string ListHeroesForHint()
        {
            if (ListHeroesForFlag.Count == 0)
                return "Нет героев";
            else
            {
                string list = "";
                int pos = 1;
                foreach (Creature h in ListHeroesForFlag)
                {
                    list += (list != "" ? Environment.NewLine : "") + $"{pos}. {h.GetNameHero()} ({h.Level})";
                    pos++;
                }

                return list;
            }
        }
    }
}
