using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настройки характеристик для типа существ
    internal class DescriptorCreatureProperty : DescriptorCreaturePropertyMain
    {
        public DescriptorCreatureProperty(DescriptorProperty dp, XmlNode n) : base(n)
        {
            Descriptor = dp;
        }

        internal DescriptorProperty Descriptor { get; }
    }
}