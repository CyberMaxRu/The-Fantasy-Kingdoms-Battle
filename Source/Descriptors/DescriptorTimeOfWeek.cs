using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    sealed internal class DescriptorTimeOfWeek : DescriptorVisual
    {
        private string nameImageBackground;

        public DescriptorTimeOfWeek(XmlNode n): base(n)
        {
            Index = Descriptors.TimesOfWeek.Count;
            nameImageBackground = GetStringNotNull(n, "ImageBackground");
        }

        internal int Index { get; }

        internal Bitmap GetBitmap(Size size)
        {
            return Program.formMain.CollectionBackgroundImage.GetBitmap(nameImageBackground, size);
        }
    }
}
