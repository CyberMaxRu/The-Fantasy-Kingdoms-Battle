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
            Cost = GetIntegerNotNull(n, "Cost", ID);
            Quantity = GetIntegerNotNull(n, "Quantity", ID);
            Duration = GetIntegerNotNull(n, "Duration", ID);
            InternalRefresh = GetIntegerNotNull(n, "InternalRefresh", ID);

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

        protected override string GetName(XmlNode n)
        {
            return "noname";
        }

        internal override int GetImageIndex(XmlNode n)
        {
            return 0;// Изначально устанавливаем ImageIndex в 0, а потом инициализируем при настройке связи
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            DescriptorEntity = Config.FindAbility(nameEntity, false);
            if (DescriptorEntity is null)
                DescriptorEntity = Config.FindItem(nameEntity, false);
            if (DescriptorEntity is null)
                DescriptorEntity = Config.FindGroupItem(nameEntity, false);

            Debug.Assert(DescriptorEntity != null);

            ImageIndex = DescriptorEntity.ImageIndex;
            Name = DescriptorEntity.Name;
        }
    }
}
