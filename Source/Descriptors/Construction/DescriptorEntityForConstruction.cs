using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorEntityForConstruction : DescriptorEntityForActiveEntity
    {
        public DescriptorEntityForConstruction(DescriptorConstruction construction, XmlNode n) : base(n)
        {
            Construction = construction;

            Cost = new ListBaseResources(n.SelectSingleNode("Cost"));
            DaysProcessing = GetInteger(n, "DaysProcessing");
            Requirements = new ListDescriptorRequirements(this, n.SelectSingleNode("Requirements"));
        }

        internal DescriptorConstruction Construction { get; }

        internal abstract string GetTypeEntity();

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Requirements.TuneLinks();
        }

    }
}
