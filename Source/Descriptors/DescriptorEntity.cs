using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый описатель для всех сущностей - сооружений, существ, умений, предметов и т.д.
    internal abstract class DescriptorEntity : DescriptorVisual
    {
        public DescriptorEntity(XmlNode n) : base(n)
        {
            Descriptors.AddEntity(this);
        }

        public DescriptorEntity(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex)
        {
            Descriptors.AddEntity(this);
        }
    }
}
