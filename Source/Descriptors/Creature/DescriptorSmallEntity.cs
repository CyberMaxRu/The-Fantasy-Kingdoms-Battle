using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorSmallEntity : DescriptorEntity
    {
        private List<string> availableForHeroesString;

        public DescriptorSmallEntity(XmlNode n) : base(n)
        {
            AvailableForAllHeroes = XmlUtils.GetBoolean(n, "AvailableForAll", false);

            // Загружаем классы героев, которые могут использовать способность
            XmlNode nch = n.SelectSingleNode("Heroes");
            if (nch != null)
            {
                availableForHeroesString = new List<string>();
                string nameHero;

                foreach (XmlNode l in nch.SelectNodes("Hero"))
                {
                    nameHero = l.InnerText;

                    // Проверяем, что такой класс героев не повторяется
                    foreach (string nameHero2 in availableForHeroesString)
                    {
                        Debug.Assert(nameHero != nameHero2, $"Герой {nameHero} повторяется в списке доступных героев для {ID}.");
                    }

                    availableForHeroesString.Add(nameHero);
                }
            }

            Debug.Assert((AvailableForAllHeroes && (availableForHeroesString is null)) || (!AvailableForAllHeroes && (availableForHeroesString != null) && (availableForHeroesString.Count > 0)),
                $"Не настроена доступность героям у {ID}.");
        }

        internal bool AvailableForAllHeroes { get; }// Сущность доступна всем существам
        internal List<DescriptorCreature> AvailableForHeroes { get; } = new List<DescriptorCreature>();

        protected override int ShiftImageIndex() => Config.ImageIndexFirstItems;

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            if (availableForHeroesString != null)
            {
                foreach (string nameHero in availableForHeroesString)
                    AvailableForHeroes.Add(Config.FindCreature(nameHero));

                availableForHeroesString = null;
            }

            // Дополняем описание
            Description += Description.Length > 0 ? Environment.NewLine : "";
            if (AvailableForAllHeroes)
            {
                Description += "- Доступно всем героям";
            }
            else
            {
                Description += "- Доступно героям:";

                foreach (DescriptorCreature tc in AvailableForHeroes)
                {
                    Description += Environment.NewLine + "  - " + tc.Name;
                }
            }
        }
    }
}
