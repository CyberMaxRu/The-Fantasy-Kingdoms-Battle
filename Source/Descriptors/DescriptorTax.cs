using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorTax : DescriptorWithID
    {
        public DescriptorTax(XmlNode n) : base(n)
        {
            Percent = GetIntegerFromXmlNode(n, "Percent", 0, 200);
        }

        internal int Percent { get; }
    }
}