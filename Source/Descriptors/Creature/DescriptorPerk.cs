using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Типы перков существ
    internal sealed class DescriptorPerk : DescriptorSmallEntity
    {
        public DescriptorPerk(XmlNode n) : base(n)
        {
            ListProperty = new int[Descriptors.PropertiesCreature.Count];

            // Загружаем изменяемые свойства
            XmlNode np = n.SelectSingleNode("Properties");
            if (np != null)
            {
                string idProperty;
                int valueProperty;
                DescriptorProperty dpc;
                for (int i = 0; i < np.ChildNodes.Count; i++)
                {
                    idProperty = np.ChildNodes[i].Name;
                    valueProperty = Convert.ToInt32(np.ChildNodes[i].InnerText);
                    dpc = Descriptors.FindPropertyCreature(idProperty);

                    Debug.Assert(ListProperty[dpc.Index] == 0);
                    ListProperty[dpc.Index] = valueProperty;
                }
            }

            //Debug.Assert(Honor <= 0);
            //Debug.Assert(Honor > 0);
            //Debug.Assert(Enthusiasm >= 0);
            //Debug.Assert(Luck >= 0);

            foreach (DescriptorPerk dp in Descriptors.Perks)
            {
                Debug.Assert(dp.ID != ID);
                Debug.Assert(dp.Name != Name);
            }
        }

        internal int[] ListProperty { get; }

        internal override string GetTypeEntity() => "Особенность";
    }
}