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
    internal sealed class DescriptorTypeLandscape : DescriptorWithID
    {
        internal List<string> nameElements = new List<string>();

        public DescriptorTypeLandscape(XmlNode n) : base(n)
        {
            // Загружаем список доступных элементов
            XmlNode ne = n.SelectSingleNode("Elements");
            string name;

            foreach (XmlNode l in ne.SelectNodes("Element"))
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

        internal List<DescriptorElementLandscape> Elements { get; } = new List<DescriptorElementLandscape>();

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            foreach (string name in nameElements)
            {
                Elements.Add(FormMain.Config.FindElementLandscape(name));
            }

            nameElements = null;
        }
    }
}