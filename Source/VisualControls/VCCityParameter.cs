using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCityParameter : VCIconAndDigitValue
    {
        private Construction construction;
        private HintListCustomCells listForHint = new HintListCustomCells();

        public VCCityParameter(VisualControl parent, int shiftX, int shiftY, DescriptorCityParameter parameter)
            : base(parent, shiftX, shiftY, 72, 0)
        {
            Parameter = parameter;
        }

        internal DescriptorCityParameter Parameter { get; }

        internal override void Draw(Graphics g)
        {
            Image.ImageIndex = Parameter.ImageIndex16;

            base.Draw(g);
        }

        internal void UpdateData(Construction c)
        {
            construction = c;

            if (construction.Level > 0)
            {
                Color = Color.White;

                if (c.ChangeCityParameters[Parameter.Index] != 0)
                    Text = FormatDecimal100(c.ChangeCityParameters[Parameter.Index], true);
                else
                    Text = "";
            }
            else
            {
                Color = Color.Silver;
                if ((c.Descriptor.Levels[1].ChangeCityParametersPerTurn != null) && (c.Descriptor.Levels[1].ChangeCityParametersPerTurn[Parameter.Index] != 0))
                    Text = FormatDecimal100(c.Descriptor.Levels[1].ChangeCityParametersPerTurn[Parameter.Index], true);
                else
                    Text = "";
            }
        }

        internal override bool PrepareHint()
        {
            PanelHint.AddStep2Descriptor(Parameter);
            PanelHint.AddStep5Description(Parameter.Description);

            if (construction.Level > 0)
            {
                listForHint.Clear();
                int change = construction.Descriptor.Levels[construction.Level].ChangeCityParametersPerTurn[Parameter.Index];
                if (change != 0)
                    listForHint.Add((construction.GetCellImageIndex(), FormatDecimal100(change), Color.White));

                foreach (ConstructionExtension ce in construction.Extensions)
                {
                    change = ce.Descriptor.ChangeCityParametersPerTurn[Parameter.Index];
                    if (change != 0)
                        listForHint.Add((ce.GetImageIndex(), FormatDecimal100(change), Color.White));
                }

                PanelHint.AddStep21ListCustomCells(listForHint);
            }

            return true;
        }
    }
}
