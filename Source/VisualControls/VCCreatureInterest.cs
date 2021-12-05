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
            PanelHint.AddStep2Header(interest.Descriptor.Descriptor.Name);
            PanelHint.AddStep3Type("Интерес");
            PanelHint.AddStep4Level($"{interest.Descriptor.Descriptor.Name}: {FormatDecimal100(interest.Value)}/{FormatDecimal100(1000)}");
            PanelHint.AddStep5Description(interest.Descriptor.Descriptor.Description);

            return true;
        }
    }
}