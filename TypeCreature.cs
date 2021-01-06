using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Типы существ
    internal sealed class TypeCreature
    {
        public TypeCreature(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n.SelectSingleNode("ID"));
            Name = XmlUtils.GetStringNotNull(n.SelectSingleNode("Name"));

            // Проверяем, что таких же ID и наименования нет
            foreach (TypeCreature tc in FormMain.Config.TypesCreatures)
            {
                Debug.Assert(tc.ID != ID);
                Debug.Assert(tc.Name != Name);
            }
        }

        internal string ID { get; }
        internal string Name { get; }       
    }
}
