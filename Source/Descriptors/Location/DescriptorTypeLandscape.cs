using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс типа местности
    internal sealed class DescriptorTypeLandscape : DescriptorWithID
    {
        public DescriptorTypeLandscape(XmlNode n) : base(n)
        {
        }
    }
}