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
        public DescriptorRequirement(DescriptorEntity forEntity, XmlNode n) : base()
        {
            ForEntity = forEntity;
        }

        public DescriptorRequirement(DescriptorEntity forEntity) : base()
        {
            ForEntity = forEntity;
        }

        internal DescriptorEntity ForEntity { get; }
        internal abstract bool CheckRequirement(Player p);
        internal abstract TextRequirement GetTextRequirement(Player p);
    }
}