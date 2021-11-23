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

            foreach (DescriptorProduct pd in Descriptors.ConstructionProducts)
            {
                Debug.Assert(pd.ID != ID);
            }

            Descriptors.ConstructionProducts.Add(this);
        }

        internal DescriptorEntityForCreature DescriptorEntity { get; private set; }
        internal DescriptorEntityForConstruction EntityForConstruction { get; private set; }

        protected override string GetName(XmlNode n)
        {
            return "noname";
        }

        internal override int GetImageIndex(XmlNode n)
        {
            return 0;// Изначально устанавливаем ImageIndex в 0, а потом инициализируем при настройке связи
        }

        internal override string GetTypeEntity()
        {
            return DescriptorEntity.GetTypeEntity();
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            DescriptorEntity = Descriptors.FindAbility(nameEntity, false);
            if (DescriptorEntity is null)
                DescriptorEntity = Descriptors.FindItem(nameEntity, false);
            if (DescriptorEntity is null)
                DescriptorEntity = Descriptors.FindGroupItem(nameEntity, false);
            if ( DescriptorEntity is null)
                DescriptorEntity = Descriptors.FindGroupItem(nameEntity, false);
            if (DescriptorEntity is null)
            {
                EntityForConstruction = Construction.FindConstructionEvent(nameEntity, false);
                if (EntityForConstruction is null)
                    EntityForConstruction = Construction.FindConstructionImprovement(nameEntity, false);
                if (EntityForConstruction is null)
                    EntityForConstruction = Construction.FindConstructionTournament(nameEntity, false);
                if (EntityForConstruction is null)
                    EntityForConstruction = Construction.FindConstructionService(nameEntity, false);
            }

            Debug.Assert((DescriptorEntity != null) || (EntityForConstruction != null), $"Сущность {nameEntity} не найдена.");

            if (DescriptorEntity != null)
            {
                ImageIndex = DescriptorEntity.ImageIndex;
                Name = DescriptorEntity.Name;
            }
            else
            {
                ImageIndex = EntityForConstruction.ImageIndex;
                Name = EntityForConstruction.Name;
            }

            nameEntity = "";
        }
    }
}
