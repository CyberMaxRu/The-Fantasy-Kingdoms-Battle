using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorEntityForConstruction : DescriptorSmallEntity
    {
        public DescriptorEntityForConstruction(DescriptorConstruction construction, XmlNode n) : base(n)
        {
            Construction = construction;
        }

        internal DescriptorConstruction Construction { get; }

        internal abstract string GetTypeEntity();
    }
}
