using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorSmallEntity : DescriptorEntity
    {
        public DescriptorSmallEntity(XmlNode n) : base(n)
        {

        }

        protected override int ShiftImageIndex() => FormMain.Config.ImageIndexFirstItems;
    }
}
