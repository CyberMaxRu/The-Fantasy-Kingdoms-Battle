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
            Program.formMain.formHint.AddStep2Header(property.Property.Name);
            Program.formMain.formHint.AddStep4Level($"{property.Property.Name}: {FormatDecimal100(property.Value)}");
            Program.formMain.formHint.AddStep5Description(property.Property.Description);
            if (property.ListSource.Count > 0)
            {
                List<(DescriptorEntity, string)> list = new List<(DescriptorEntity, string)>();

                foreach (Perk p in property.ListSource)
                {
                    for (int i = 0; i < p.ListProperty.Length; i++)
                    {
                        if (p.ListProperty[i] != 0)
                            list.Add((p.Descriptor, FormatDecimal100(p.ListProperty[i], true)));
                    }
                }

                Assert(list.Count > 0);
                Program.formMain.formHint.AddStep19Descriptors(list);
            }

            return true;
        }
    }
}