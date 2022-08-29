using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс требования
    internal abstract class DescriptorRequirement : Descriptor
    {
        public DescriptorRequirement(Descriptor forEntity, XmlNode n, ListDescriptorRequirements list) : base()
        {
            ForEntity = forEntity;
            List = list;                
        }

        public DescriptorRequirement(Descriptor forEntity, ListDescriptorRequirements list) : base()
        {
            ForEntity = forEntity;
            List = list;
        }

        internal Descriptor ForEntity { get; }
        internal ListDescriptorRequirements List { get; }
        internal virtual bool CheckRequirement(Player p) => List.AllowCheating && p.CheatingIgnoreRequirements;
        internal abstract TextRequirement GetTextRequirement(Player p, Construction inConstruction = null);
    }
}