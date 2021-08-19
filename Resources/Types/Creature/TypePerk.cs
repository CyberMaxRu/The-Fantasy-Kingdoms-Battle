using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Типы перков существ
    internal sealed class TypePerk : TypeObject
    {
        public TypePerk(XmlNode n) : base(n)
        {

        }
    }
}
