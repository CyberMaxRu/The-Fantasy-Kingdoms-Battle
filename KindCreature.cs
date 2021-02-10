using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Виды существ
    internal sealed class KindCreature
    {
        public KindCreature(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n.SelectSingleNode("ID"));
            Name = XmlUtils.GetStringNotNull(n.SelectSingleNode("Name"));

            // Проверяем, что таких же ID и наименования нет
            foreach (KindCreature kc in FormMain.Config.KindCreatures)
            {
                Debug.Assert(kc.ID != ID);
                Debug.Assert(kc.Name != Name);
            }
        }

        internal string ID { get; }
        internal string Name { get; }       
    }
}
