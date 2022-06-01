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

            if ((availableForHeroesString is null) && ForHeroes())
                AvailableForAllHeroes = true;

            if (availableForHeroesString is null)
                AvailableForAllHeroes = true;
        }

        public DescriptorSmallEntity(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex)
        {

        }

        internal bool AvailableForAllHeroes { get; }// Сущность доступна всем существам
        internal List<DescriptorCreature> AvailableForHeroes { get; } = new List<DescriptorCreature>();
        internal List<DescriptorSmallEntity> UseForResearch { get; } = new List<DescriptorSmallEntity>();

        protected override int ShiftImageIndex() => Config.ImageIndexFirstItems;
        protected virtual bool ForHeroes() => true;

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if (ForHeroes())
            {
                //Utils.Assert((AvailableForAllHeroes && (availableForHeroesString is null)) || (!AvailableForAllHeroes && (availableForHeroesString != null) && (availableForHeroesString.Count > 0)),
                //    $"Не настроена доступность героям у {ID}.");
            }
            else
            {
                //Debug.Assert(AvailableForAllHeroes == false);
                Debug.Assert(availableForHeroesString is null);
            }

            if (availableForHeroesString != null)
            {
                foreach (string nameHero in availableForHeroesString)
                    AvailableForHeroes.Add(Descriptors.FindCreature(nameHero));

                availableForHeroesString = null;
            }

            // Дополняем описание
            if (ForHeroes())
            {
                Description += Description.Length > 0 ? Environment.NewLine : "";
                if (AvailableForAllHeroes || (AvailableForHeroes.Count == 0))
                {
                    Description += "- Доступно всем героям";
                }
                else
                {
                    Debug.Assert(AvailableForHeroes.Count > 0);
                    
                    Description += "- Доступно героям:";

                    foreach (DescriptorCreature tc in AvailableForHeroes)
                    {
                        Description += Environment.NewLine + "  - " + tc.Name;
                    }
                }
            }
        }

        internal override int GetImageIndex(XmlNode n)
        {
            int imageIndex = base.GetImageIndex(n);

            XmlAttribute attrIcon = n.SelectSingleNode("ImageIndex").Attributes["Size"];
            if ((attrIcon != null) && (attrIcon.Value == "128"))
                imageIndex = XmlUtils.GetIntegerNotNull(n, "ImageIndex") - 1;

            return imageIndex;
        }
    }
}
