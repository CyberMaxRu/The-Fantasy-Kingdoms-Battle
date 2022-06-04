using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCConstructionSatisfNeed : VCIconAndDigitValue
    {
        public VCConstructionSatisfNeed(VisualControl parent, int shiftX, int shiftY, int width, DescriptorNeed dn) : base(parent, shiftX, shiftY, width, dn.ImageIndex)
        {
            Need = dn;
        }

        internal DescriptorNeed Need { get; }
        internal Construction Construction { get; set; }

        internal override void Draw(Graphics g)
        {
            Text = Construction.SatisfactionNeeds != null ? Utils.DecIntegerBy10(Construction.SatisfactionNeeds[Need.Index]) : "";

            base.Draw(g);
        }

        internal override bool PrepareHint()
        {
            PanelHint.AddStep2Header(Need.Name);
            //PanelHint.AddStep5Description()

            return true;
        }
    }
}
