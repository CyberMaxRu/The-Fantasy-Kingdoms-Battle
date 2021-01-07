using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс здания
    internal sealed class TypeEconomicConstruction : TypeConstruction
    {
        public TypeEconomicConstruction(XmlNode n) : base(n)
        {
            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Description.Length > 0);

            /*if (!HasTreasury)
            {
                Debug.Assert(GoldByConstruction == 0);
            }*/

            // Проверяем, что таких же ID и наименования нет
            foreach (TypeEconomicConstruction tec in FormMain.Config.TypeEconomicConstructions)
            {
                Debug.Assert(tec.ID != ID);
                Debug.Assert(tec.Name != Name);
                Debug.Assert(tec.ImageIndex != ImageIndex);
            }
        }
    }
}