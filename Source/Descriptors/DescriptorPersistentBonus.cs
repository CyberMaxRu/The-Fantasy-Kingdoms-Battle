using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.Utils;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypePersistentBonus { Economic, Military, Other };

    internal sealed class DescriptorPersistentBonus : DescriptorWithID
    {
        public DescriptorPersistentBonus(XmlNode n) : base(n)
        {
            Type = (TypePersistentBonus)Enum.Parse(typeof(TypePersistentBonus), GetString(n, "Type"));

            foreach (DescriptorPersistentBonus b in Descriptors.PersistentBonuses)
            {
                Assert(b.ID != ID);
                Assert(b.Name != Name);
            }
        }

        public TypePersistentBonus Type { get; set; }
    }
}