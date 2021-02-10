using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс вида горожанина - стражники, крестьяне
    internal sealed class TypeCitizen : TypeCreature
    {
        public TypeCitizen(XmlNode n) : base(n)
        {
            // Проверяем, что таких же ID и наименования нет
            foreach (TypeCitizen kc in FormMain.Config.TypeCitizens)
            {
                Debug.Assert(kc.ID != ID);
                Debug.Assert(kc.Name != Name);
                Debug.Assert(kc.ImageIndex != ImageIndex);
            }
        }
    }
}