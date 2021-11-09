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
    internal sealed class DescriptorProduct : DescriptorWithID
    {
        private string nameEntity;

        public DescriptorProduct(XmlNode n) : base(n)
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
        }

        internal DescriptorEntityForCreature Descriptor { get; private set; }
        internal int Cost { get; }
        internal int Quantity { get; }
        internal int Duration { get; }
        internal int InternalRefresh { get; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Descriptor = Config.FindAbility(nameEntity, false);
            if (Descriptor is null)
                Descriptor = Config.FindItem(nameEntity, false);
            if (Descriptor is null)
                Descriptor = Config.FindGroupItem(nameEntity, false);
            if (Descriptor is null)
                Descriptor = Config.Find(nameEntity, false);

            Debug.Assert(Descriptor != null;
        }
    }
}
