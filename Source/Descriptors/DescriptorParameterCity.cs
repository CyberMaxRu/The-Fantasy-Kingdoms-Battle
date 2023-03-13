using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс параметра населенного пункта
    internal sealed class DescriptorCityParameter : DescriptorSmallEntity
    {
        public DescriptorCityParameter(XmlNode n) : base(n)
        {
            ImageIndex16 = XmlUtils.GetIntegerNotNull(n, "ImageIndex16");
            Index = Descriptors.CityParameters.Count;

            foreach (DescriptorCityParameter sp in Descriptors.CityParameters)
            {
                Debug.Assert(sp.ID != ID);
                Debug.Assert(sp.Name != Name);
                Debug.Assert(sp.ImageIndex != ImageIndex);
            }
        }

        internal int Index { get; }
        internal int ImageIndex16 { get; }

        protected override int ShiftImageIndex()
        {
            return FormMain.Config.ImageIndexFirstItems + 1;
        }

        internal override string GetTypeEntity()
        {
            return "Параметр насел. пункта";
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

        }
    }
}
