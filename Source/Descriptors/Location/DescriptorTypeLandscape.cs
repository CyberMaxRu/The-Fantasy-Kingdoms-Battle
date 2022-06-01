using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс типа местности
    internal sealed class DescriptorTypeLandscape : DescriptorEntity
    {
        internal List<string> nameElements = new List<string>();
        private Bitmap BackgroundImage;

        public DescriptorTypeLandscape(XmlNode n) : base(n)
        {
            Index = Descriptors.TypeLandscapes.Count;

            // Загружаем список доступных элементов
            XmlNode ne = n.SelectSingleNode("Constructions");
            string name;

            foreach (XmlNode l in ne.SelectNodes("Construction"))
            {
                name = l.InnerText;

                // Проверяем, что такой элемент не повторяется
                foreach (string name2 in nameElements)
                {
                    Debug.Assert(name != name2);
                }

                nameElements.Add(name);
            }

            Debug.Assert(nameElements.Count > 0, $"У {ID} нет элементов.");
        }

        internal int Index { get; }
        internal List<DescriptorConstruction> Elements { get; } = new List<DescriptorConstruction>();

        internal override string GetTypeEntity() => "Ландшафт";

        internal override void TuneLinks()
        {
            base.TuneLinks();

            foreach (string name in nameElements)
            {
                Elements.Add(FormMain.Descriptors.FindConstruction(name));
            }

            nameElements = null;
        }
    }
}