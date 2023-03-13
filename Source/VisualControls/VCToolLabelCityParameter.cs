using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCToolLabelCityParameter : VCToolLabel
    {
        private Player player;

        public VCToolLabelCityParameter(VisualControl parent, int shiftX, int shiftY, DescriptorCityParameter parameter)
            : base(parent, shiftX, shiftY, "", parameter.ImageIndex16)
        {
            Parameter = parameter;
            Width = 128;
            ShowHint += VCToolLabelSettlementParameter_ShowHint;
        }

        private void VCToolLabelSettlementParameter_ShowHint(object sender, EventArgs e)
        {
            PanelHint.AddStep2Descriptor(Parameter);
            string description = Parameter.Description;
            int changeBase = player.Lobby.TypeLobby.ChangeCityParametersPerTurn[Parameter.Index];
            int changeByConstructions = player.ChangeCityParametersPerTurnByConstructions[Parameter.Index];
            if ((changeBase != 0) || (changeByConstructions != 0))
            {
                description += Environment.NewLine + " ";
                if (changeBase != 0)
                    description += Environment.NewLine + "Базовое значение: " + Utils.FormatDecimal100(changeBase);
                if (changeByConstructions != 0)
                    description += Environment.NewLine + "Сооружения: " + Utils.FormatDecimal100(changeByConstructions);
            }
            PanelHint.AddStep5Description(description);
        }

        internal DescriptorCityParameter Parameter { get; }

        internal void UpdateData(Player p)
        {
            player = p;

            Text = Utils.FormatDecimal100(p.CityParameters[Parameter.Index]);
            Text += $" ({Utils.FormatDecimal100(p.ChangeCityParametersPerTurn[Parameter.Index], true)})";
        }
    }
}
