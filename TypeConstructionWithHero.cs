using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Тип здания с героями
    internal abstract class TypeConstructionWithHero : TypeConstruction
    {
        public TypeConstructionWithHero(XmlNode n) : base(n)
        {
            GoldByConstruction = XmlUtils.GetInteger(n.SelectSingleNode("GoldByConstruction"));
        }

        internal int GoldByConstruction { get; }// Количество золота в казне при постройке
        internal TypeHero TrainedHero { get; set; }
    }
}
