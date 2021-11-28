using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorLevelTax : DescriptorWithID
    {
        public DescriptorLevelTax(XmlNode n) : base(n)
        {
            Index = Descriptors.LevelTaxes.Count;
            Percent = GetIntegerFromXmlNode(n, "Percent", 0, 200);
        }

        internal int Index { get; }
        internal int Percent { get; }
    }
}