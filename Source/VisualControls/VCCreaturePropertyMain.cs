using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class VCCreaturePropertyMain : VCIconAndDigitValue
    {
        private CreaturePropertyMain property;

        public VCCreaturePropertyMain(VisualControl parent, int shiftX, int shiftY, int width)
            : base(parent, shiftX, shiftY, width, -1)
        {
            Visible = false;
        }

        internal virtual void SetProperty(CreaturePropertyMain property)
        {
            this.property = property;
            Visible = property != null;
        }

        internal override void Draw(Graphics g)
        {
            Text = FormatDecimal100AsInt(property.Value);

            base.Draw(g);
        }
    }
}