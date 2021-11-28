using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorLevelTax : DescriptorWithID
    {
        public DescriptorLevelTax(XmlNode n) : base(n)
        {
            Percent = GetIntegerFromXmlNode(n, "Percent", 0, 200);
        }

        internal int Percent { get; }
    }
}