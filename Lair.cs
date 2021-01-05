using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс логова монстров
    internal sealed class Lair
    {
        public Lair(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = n.SelectSingleNode("Description").InnerText.Replace("/", Environment.NewLine);
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Line = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("Line"));

            Debug.Assert(Line >= 1);
            Debug.Assert(Line <= 3);

            // Проверяем, что таких же ID и наименования нет
            foreach (Lair l in FormMain.Config.Lairs)
            {
                if (l.ID == ID)
                    throw new Exception("В конфигурации логов повторяется ID = " + ID);

                if (l.Name == Name)
                    throw new Exception("В конфигурации логов повторяется Name = " + Name);

                if (l.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации логов повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
        internal int Line { get; }
        internal PanelLair Panel { get; set; }

    }
}
