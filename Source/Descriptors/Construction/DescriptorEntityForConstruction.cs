using System.Xml;


namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorEntityForConstruction : DescriptorSmallEntity
    {
        public DescriptorEntityForConstruction(DescriptorConstruction descriptor, XmlNode n) : base(n)
        {
            Descriptor = descriptor;
        }

        internal DescriptorConstruction Descriptor { get; }

        internal abstract string GetTypeEntity();
    }
}
