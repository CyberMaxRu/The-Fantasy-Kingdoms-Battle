using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCToolLabelResource : VCToolLabel
    {
        public VCToolLabelResource(VisualControl parent, int shiftX, int shiftY, DescriptorBaseResource resource)
            : base(parent, shiftX, shiftY, "", resource.ImageIndex16)
        {
            Resource = resource;
            ShowHint += VCToolLabelResource_ShowHint;
        }

        private void VCToolLabelResource_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Descriptor(Resource);
            PanelHint.AddStep5Description("Количество ресурса");
        }

        internal DescriptorBaseResource Resource { get; }

        internal void UpdateData(Player p)
        {
            Text = p.BaseResources[Resource.Number].ToString();
        }
    }
}