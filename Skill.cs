using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Skill
    {
        public Skill(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = n.SelectSingleNode("ImageIndex") != null ? Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText) : -1;
            Position = FormMain.Config.Skills.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Skill s in FormMain.Config.Skills)
            {
                if (s.ID == ID)
                {
                    throw new Exception("В конфигурации навыков повторяется ID = " + ID);
                }

                if (s.Name == Name)
                {
                    throw new Exception("В конфигурации навыков повторяется Name = " + Name);
                }

                if (s.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации навыков повторяется ImageIndex = " + ImageIndex.ToString());
                }
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }

        internal int Position { get; }
    }
}
