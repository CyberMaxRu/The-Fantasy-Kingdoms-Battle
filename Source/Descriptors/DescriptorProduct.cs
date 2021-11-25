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
    internal sealed class DescriptorProduct : DescriptorEntityForActiveEntity
    {
        private string nameEntity;

        public DescriptorProduct(DescriptorConstruction descriptor, XmlNode n) : base(descriptor, n)
        {
            nameEntity = GetStringNotNull(n, "Entity");

            foreach (DescriptorProduct pd in Descriptors.ConstructionProducts)
            {
                Debug.Assert(pd.ID != ID);
            }

            Descriptors.ConstructionProducts.Add(this);
        }

        internal DescriptorSmallEntity SmallEntity { get; private set; }

        protected override string GetName(XmlNode n)
        {
            return GetStringNotNull(n, "Entity"); 
        }

        internal override int GetImageIndex(XmlNode n)
        {
            return 0;// Изначально устанавливаем ImageIndex в 0, а потом инициализируем при настройке связи
        }

        internal override string GetTypeEntity()
        {
            return SmallEntity.GetTypeEntity();
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            SmallEntity = Descriptors.FindAbility(nameEntity, false);
            if (SmallEntity is null)
                SmallEntity = Descriptors.FindItem(nameEntity, false);
            if (SmallEntity is null)
                SmallEntity = Descriptors.FindGroupItem(nameEntity, false);
            if ( SmallEntity is null)
                SmallEntity = Descriptors.FindGroupItem(nameEntity, false);
            if (SmallEntity is null)
                SmallEntity = ActiveEntity.FindEntity(nameEntity);

            ImageIndex = SmallEntity.ImageIndex;
            Name = SmallEntity.Name;

            nameEntity = "";
        }
    }
}
