﻿using System;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCreatureNeed : VCCreaturePropertyMain
    {
        private CreatureNeed need;

        public VCCreatureNeed(VisualControl parent, int shiftX, int shiftY, int width)
            : base(parent, shiftX, shiftY, width)
        {

        }

        internal override void SetProperty(CreaturePropertyMain property)
        {
            base.SetProperty(property); 

            need = property as CreatureNeed;
        }

        internal override void Draw(Graphics g)
        {
            ImageIndex = need.Need.Descriptor.ImageIndex;

            base.Draw(g);
        }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(need.Need.Descriptor.Name);
            Program.formMain.formHint.AddStep3Type("Потребность");
            Program.formMain.formHint.AddStep4Level($"{need.Need.Descriptor.Name}: {FormatDecimal100(need.Value)}/{FormatDecimal100(1000)}"
                + Environment.NewLine + $"Увеличение в день: {FormatDecimal100(need.IncreasePerDay)}");
            Program.formMain.formHint.AddStep5Description(need.Need.Descriptor.Description);

            return true;
        }
    }
}