using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCToolLabelSettlementParameter : VCToolLabel
    {
        public VCToolLabelSettlementParameter(VisualControl parent, int shiftX, int shiftY, DescriptorSettlementParameter parameter)
            : base(parent, shiftX, shiftY, "", parameter.ImageIndex16)
        {
            Parameter = parameter;
            Width = 80;
            ShowHint += VCToolLabelSettlementParameter_ShowHint;
        }

        private void VCToolLabelSettlementParameter_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Descriptor(Parameter);
            PanelHint.AddStep5Description(Parameter.Description);
        }

        internal DescriptorSettlementParameter Parameter { get; }

        internal void UpdateData(Player p)
        {
            Text = p.CityParameters[Parameter.Index].ToString();
        }
    }
}
