using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс заклинания
    internal enum TypeEntity { Location, Construction };
    internal enum ActionOfSpell { ScoutInLocation };
    internal sealed class DescriptorConstructionSpell : DescriptorEntityForActiveEntity
    {
        public DescriptorConstructionSpell(DescriptorConstruction construction, XmlNode n) : base(construction, n)
        {
            TypeEntity = (TypeEntity)Enum.Parse(typeof(TypeEntity), GetStringNotNull(n, "TypeEntity"));
            Coord = GetPoint(n, "Coord");
            Scouted = GetBooleanNotNull(n, "Scouted");
            Action = (ActionOfSpell)Enum.Parse(typeof(ActionOfSpell), GetStringNotNull(n, "Action"));
        }

        internal Point Coord { get; }
        internal TypeEntity TypeEntity { get; }
        internal bool Scouted { get; }
        internal ActionOfSpell Action { get; }

        internal override string GetTypeEntity()
        {
            return "Заклинание";
        }
    }
}
