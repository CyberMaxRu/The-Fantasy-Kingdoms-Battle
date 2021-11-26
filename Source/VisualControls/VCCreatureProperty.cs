using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCCreatureProperty : VCIconAndDigitValue
    {
        private CreatureProperty property;

        public VCCreatureProperty(VisualControl parent, int shiftX, int shiftY, int width)
            : base(parent, shiftX, shiftY, width, -1)
        {

        }

        internal void SetProperty(CreatureProperty property)
        {
            this.property = property;
            Visible = this.property != null;
        }

        internal override void Draw(Graphics g)
        {
            ImageIndex = property.Property.ImageIndex;
            Text = FormatDecimal100AsInt(property.Value);

            base.Draw(g);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(property.Property.Name);
            Program.formMain.formHint.AddStep4Level($"{property.Property.Name}: {FormatDecimal100(property.Value)}");
            Program.formMain.formHint.AddStep5Description(property.Property.Description);
            if (property.ListSource.Count > 0)
            {
                List<(DescriptorEntity, string)> list = new List<(DescriptorEntity, string)>();

                foreach (Perk p in property.ListSource)
                {
                    Assert(p.Descriptor.GetValueProperty(property.Property.NameProperty) != 0);

                    list.Add((p.Descriptor, DecIntegerBy10(p.Descriptor.GetValueProperty(property.Property.NameProperty), true)));
                }

                Assert(list.Count > 0);
                Program.formMain.formHint.AddStep19Descriptors(list);
            }

            return true;
        }
    }
}