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
        private List<string> availableForHeroesString = new List<string>();

        public DescriptorSmallEntity(XmlNode n) : base(n)
        {
            AvailableForAllHeroes = XmlUtils.GetBoolean(n, "AvailableForAll", false);

            // Загружаем классы героев, которые могут использовать способность
            XmlNode nch = n.SelectSingleNode("Heroes");
            if (nch != null)
            {
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

            Debug.Assert((AvailableForAllHeroes && (availableForHeroesString.Count > 0)) || (!AvailableForAllHeroes && (availableForHeroesString.Count > 0)));
        }

        internal bool AvailableForAllHeroes { get; }// Сущность доступна всем существам
        internal List<DescriptorCreature> AvailableForHeroes { get; } = new List<DescriptorCreature>();

        protected override int ShiftImageIndex() => Config.ImageIndexFirstItems;

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            foreach (string nameHero in availableForHeroesString)
                AvailableForHeroes.Add(Config.FindCreature(nameHero));

            availableForHeroesString = null;
        }
    }
}
