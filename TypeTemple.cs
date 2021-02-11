using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип храма
    internal sealed class TypeTemple : TypeConstruction
    {
        public TypeTemple(XmlNode n) : base(n)
        {

        }
    }
}
