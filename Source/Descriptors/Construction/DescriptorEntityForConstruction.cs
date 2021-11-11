using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorEntityForConstruction : DescriptorSmallEntity
    {
        public DescriptorEntityForConstruction(DescriptorConstruction construction, XmlNode n) : base(n)
        {
            Construction = construction;
            NameTypeEntity = GetTypeEntity();
        }

        internal DescriptorConstruction Construction { get; }
        internal string NameTypeEntity { get; }

        internal abstract string GetTypeEntity();
    }
}
