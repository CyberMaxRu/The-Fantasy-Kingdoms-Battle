using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class CapitalPage : DescriptorSmallEntity
    {
        public CapitalPage(XmlNode n) : base(n)
        {
            Index = Descriptors.CapitalPages.Count;
            NameTexture = GetStringFromXmlNode(n, "NameTexture");
        }

        internal int Index { get; }
        internal string NameTexture { get; }

        internal override string GetTypeEntity()
        {
            Utils.DoException("У страницы нельзя получать тип.");
            return "";
        }
    }
}