using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCCreatureProperty : VCCreaturePropertyMain
    {
        private CreatureProperty property;

        public VCCreatureProperty(VisualControl parent, int shiftX, int shiftY, int width)
            : base(parent, shiftX, shiftY, width)
        {

        }

        internal override void SetProperty(CreaturePropertyMain property)
        {
            base.SetProperty(property);

            this.property = property as CreatureProperty;
        }

        internal override void Draw(Graphics g)
        {
            ImageIndex = property.Property.ImageIndex;

            base.Draw(g);
        }

        internal override bool PrepareHint()
        {
            PanelHint.AddStep2Header(property.Property.Name);
            PanelHint.AddStep4Level($"{property.Property.Name}: {FormatDecimal100(property.Value)}");
            PanelHint.AddStep5Description(property.Property.Description);
            if (property.ListSource.Count > 0)
            {
                PanelHint.AddStep19Perks(property.ListSource, property.Property.Index);
            }

            return true;
        }
    }
}