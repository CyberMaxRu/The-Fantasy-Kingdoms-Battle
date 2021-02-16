using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class SecondarySkill : Entity
    {
        public SecondarySkill(XmlNode n) : base(n)
        {
        }

        internal void TuneDeferredLinks()
        {

        }

        protected override void DoPrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Name, "Вторичный навык", Description);
        }

        protected override int GetValue() => 0;
    }
}
