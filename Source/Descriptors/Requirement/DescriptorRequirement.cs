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
        public DescriptorRequirement(DescriptorCellMenu forCellMenu, XmlNode n) : base()
        {
            ForCellMenu = forCellMenu;
        }

        public DescriptorRequirement(DescriptorCellMenu forCellMenu) : base()
        {
            ForCellMenu = forCellMenu;
        }

        internal DescriptorCellMenu ForCellMenu { get; }
        internal abstract bool CheckRequirement(Player p);
        internal abstract TextRequirement GetTextRequirement(Player p);
    }
}