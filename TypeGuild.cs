using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип гильдии
    internal sealed class TypeGuild : TypeConstruction
    {
        public TypeGuild(XmlNode n) : base(n)
        {

        }

        internal override string GetTextConstructionNotBuilded() => "Гильдия не построена";
        internal override string GetTextConstructionIsFull() => "Гильдия заполнена";
    }
}
