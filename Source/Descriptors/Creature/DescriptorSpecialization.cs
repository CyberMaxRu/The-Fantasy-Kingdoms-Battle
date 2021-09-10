using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorSpecialization : DescriptorSmallEntity
    {
        public DescriptorSpecialization(XmlNode n) : base(n)
        {
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();
        }

        /*protected override void DoPrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Name, "Специализация", Description);
        }*/

        /*protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost() => null;*/
    }
}