using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс горожанина - стражники, крестьяне
    internal sealed class KindCitizen : KindCreature
    {
        public KindCitizen(XmlNode n) : base(n)
        {
            // Проверяем, что таких же ID и наименования нет
            foreach (KindCitizen kc in FormMain.Config.KindCitizens)
            {
                if (kc.ID == ID)
                {
                    throw new Exception("В конфигурации горожан повторяется ID = " + ID);
                }

                if (kc.Name == Name)
                {
                    throw new Exception("В конфигурации горожан повторяется Name = " + Name);
                }

                if (kc.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации горожан повторяется ImageIndex = " + ImageIndex.ToString());
                }
            }
        }
    }
}