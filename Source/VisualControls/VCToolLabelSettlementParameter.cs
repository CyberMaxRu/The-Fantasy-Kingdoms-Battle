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
            ShowHint += VCToolLabelResource_ShowHint;
        }

        private void VCToolLabelResource_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Descriptor(Parameter);
            PanelHint.AddStep5Description("Количество ресурса");
        }

        internal DescriptorSettlementParameter Parameter { get; }

        internal void UpdateData(Player p)
        {
            //Text = p. BaseResources[Parameter.Number].ToString();
        }
    }
}
