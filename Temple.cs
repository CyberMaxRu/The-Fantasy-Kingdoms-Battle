using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс храмов
    internal sealed class Temple
    {
        public Temple(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            Position = FormMain.Config.Temples.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Temple t in FormMain.Config.Temples)
            {
                if (t.ID == ID)
                    throw new Exception("В конфигурации храмов повторяется ID = " + ID);

                if (t.Name == Name)
                    throw new Exception("В конфигурации храмов повторяется Name = " + Name);

                if (t.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации храмов повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }
        internal int Position { get; }
        internal int Cost { get; }
        internal PanelTemple Panel { get; set; }
    }
}