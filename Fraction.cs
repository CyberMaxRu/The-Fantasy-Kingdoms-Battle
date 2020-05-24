using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс фракции
    internal sealed class Fraction
    {
        public Fraction(XmlNode n, FormMain fm)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = n.SelectSingleNode("ImageIndex") != null ? Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText) : -1;
            Position = FormMain.Config.Fractions.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Fraction f in FormMain.Config.Fractions)
            {
                if (f.ID == ID)
                {
                    throw new Exception("В конфигурации фракций повторяется ID = " + ID);
                }

                if (f.Name == Name)
                {
                    throw new Exception("В конфигурации фракций повторяется Name = " + Name);
                }

                if (f.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации фракций повторяется ImageIndex = " + ImageIndex.ToString());
                }
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }
        internal int Position { get; }
    }
}
