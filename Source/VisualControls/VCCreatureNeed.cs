using System;
using System.Collections.Generic;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCreatureNeed : VCIconAndDigitValue
    {
        private CreatureNeed need;

        public VCCreatureNeed(VisualControl parent, int shiftX, int shiftY, int width)
            : base(parent, shiftX, shiftY, width, -1)
        {

        }

        internal void SetNeed(CreatureNeed need)
        {
            this.need = need;
            Visible = this.need != null;
        }

        internal override void Draw(Graphics g)
        {
            ImageIndex = need.Need.Descriptor.ImageIndex;
            Text = FormatDecimal100AsInt(need.Value);

            base.Draw(g);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(need.Need.Descriptor.Name);
            Program.formMain.formHint.AddStep3Type("Потребность");
            Program.formMain.formHint.AddStep4Level($"{need.Need.Descriptor.Name}: {FormatDecimal100(need.Value)}/{FormatDecimal100(100)}"
                + Environment.NewLine + $"Увеличение в день: {FormatDecimal100(need.IncreasePerDay)}");
            Program.formMain.formHint.AddStep5Description(need.Need.Descriptor.Description);

            return true;
        }
    }
}