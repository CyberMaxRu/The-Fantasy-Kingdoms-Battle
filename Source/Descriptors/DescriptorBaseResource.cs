using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorBaseResource : DescriptorSmallEntity
    {
        private string constructionForMining;
        public DescriptorBaseResource(XmlNode n) : base(n)
        {
            ImageIndex16 = XmlUtils.GetIntegerNotNull(n, "ImageIndex16");
            Number = Descriptors.BaseResources.Count;
            constructionForMining = XmlUtils.GetString(n, "ConstructionForMining");

            foreach (DescriptorBaseResource br in Descriptors.BaseResources)
            {
                Debug.Assert(br.ID != ID);
                Debug.Assert(br.Name != Name);
                Debug.Assert(br.ImageIndex != ImageIndex);
            }
        }

        internal int Number { get; }
        internal int ImageIndex16 { get; }
        internal DescriptorConstruction ConstructionForMining { get; private set; }

        protected override int ShiftImageIndex()
        {
            return FormMain.Config.ImageIndexFirstItems + 1;
        }

        internal override string GetTypeEntity()
        {
            return "Базовый ресурс";
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if (constructionForMining.Length > 0)
            {
                ConstructionForMining = Descriptors.FindConstruction(constructionForMining);
                constructionForMining = "";
            }
        }
    }
}
