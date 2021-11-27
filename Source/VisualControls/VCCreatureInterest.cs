using System;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCreatureInterest : VCCreaturePropertyMain
    {
        private CreatureInterest interest;

        public VCCreatureInterest(VisualControl parent, int shiftX, int shiftY, int width)
            : base(parent, shiftX, shiftY, width)
        {
        }

        internal override void SetProperty(CreaturePropertyMain property)
        {
            base.SetProperty(property);

            interest = property as CreatureInterest;
        }

        internal override void Draw(Graphics g)
        {
            ImageIndex = interest.Descriptor.Descriptor.ImageIndex;

            base.Draw(g);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(interest.Descriptor.Descriptor.Name);
            Program.formMain.formHint.AddStep3Type("Интерес");
            Program.formMain.formHint.AddStep4Level($"{interest.Descriptor.Descriptor.Name}: {FormatDecimal100(interest.Value)}/{FormatDecimal100(1000)}");
            Program.formMain.formHint.AddStep5Description(interest.Descriptor.Descriptor.Description);

            return true;
        }
    }
}