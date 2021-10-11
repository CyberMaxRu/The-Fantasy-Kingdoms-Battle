using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Типы существ
    internal sealed class DescriptorTypeCreature : DescriptorWithID
    {
        public DescriptorTypeCreature(XmlNode n) : base(n)
        {
            // Проверяем, что таких же ID и наименования нет
            foreach (DescriptorTypeCreature kc in Config.TypeCreatures)
            {
                Debug.Assert(kc.ID != ID);
                Debug.Assert(kc.Name != Name);
            }
        }
    }
}
