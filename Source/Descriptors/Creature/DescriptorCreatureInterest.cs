using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настройки интереса для типа существ
    internal class DescriptorCreatureInterest : DescriptorCreaturePropertyMain
    {
        public DescriptorCreatureInterest(DescriptorInterest di, XmlNode n) : base(n)
        {
            Descriptor = di;
        }

        internal DescriptorInterest Descriptor { get; }
    }
}