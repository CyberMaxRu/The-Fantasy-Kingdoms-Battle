using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Типы существ
    internal sealed class TypeCreature : DescriptorWithID
    {
        public TypeCreature(XmlNode n) : base(n)
        {
            // Проверяем, что таких же ID и наименования нет
            foreach (TypeCreature kc in Config.TypeCreatures)
            {
                Debug.Assert(kc.ID != ID);
                Debug.Assert(kc.Name != Name);
            }
        }
    }
}
