using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorProduct : DescriptorEntityForConstruction
    {
        private string nameEntity;

        public DescriptorProduct(DescriptorConstruction descriptor, XmlNode n) : base(descriptor, n)
        {
            nameEntity = GetStringNotNull(n, "Entity");
            Cost = GetIntegerNotNull(n, "Cost");
            Quantity = GetIntegerNotNull(n, "Quantity");
            Duration = GetIntegerNotNull(n, "Duration");
            InternalRefresh = GetIntegerNotNull(n, "InternalRefresh");

            foreach (DescriptorProduct pd in Config.ConstructionProducts)
            {
                Debug.Assert(pd.ID != ID);
            }

            Config.ConstructionProducts.Add(this);
        }

        internal DescriptorEntityForCreature DescriptorEntity { get; private set; }
        internal int Cost { get; }
        internal int Quantity { get; }
        internal int Duration { get; }
        internal int InternalRefresh { get; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            DescriptorEntity = Config.FindAbility(nameEntity, false);
            if (DescriptorEntity is null)
                DescriptorEntity = Config.FindItem(nameEntity, false);
            if (DescriptorEntity is null)
                DescriptorEntity = Config.FindGroupItem(nameEntity, false);

            Debug.Assert(Descriptor != null);
        }
    }
}
