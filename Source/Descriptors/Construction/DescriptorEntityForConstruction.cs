using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorEntityForConstruction : DescriptorEntityForActiveEntity
    {
        public DescriptorEntityForConstruction(DescriptorConstruction construction, XmlNode n) : base(n)
        {
            Construction = construction;
        }

        public DescriptorEntityForConstruction(DescriptorConstruction construction, string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex)
        {
            Construction = construction;
        }

        internal DescriptorConstruction Construction { get; }

        internal abstract string GetTypeEntity();
    }
}
