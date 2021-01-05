using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Базовый класс для всех юнитов
    internal class Unit : Object
    {
        public Unit(XmlNode n) : base(n)
        {
            TypeUnit = FormMain.Config.FindTypeUnit(n.SelectSingleNode("TypeUnit").InnerText);
        }

        internal TypeUnit TypeUnit { get; }
    }
}