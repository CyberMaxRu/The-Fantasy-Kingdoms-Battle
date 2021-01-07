using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal sealed class Monster : KindCreature
    {
        public Monster(XmlNode n) : base(n)
        {
            // Проверяем, что таких же ID и наименования нет
            foreach (Monster m in FormMain.Config.Monsters)
            {
                if (m.ID == ID)
                {
                    throw new Exception("В конфигурации монстров повторяется ID = " + ID);
                }

                if (m.Name == Name)
                {
                    throw new Exception("В конфигурации монстров повторяется Name = " + Name);
                }

                if (m.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации монстров повторяется ImageIndex = " + ImageIndex.ToString());
                }
            }

            foreach (KindHero h in FormMain.Config.KindHeroes)
            {
                if (h.ID == ID)
                {
                    throw new Exception("В конфигурации героев есть ID как у монстра: " + ID);
                }
            }
        }
    }
}
