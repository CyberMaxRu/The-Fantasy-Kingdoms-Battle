using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCNeighborhood : VCImage48
    {
        public VCNeighborhood(VisualControl parent, int shiftX, int shiftY, TypeLobbyLayerSettings layer) : base(parent, shiftX, shiftY, layer.ImageIndex)
        {
            ShowBorder = true;
            Layer = layer;
            Hint = Layer.Hint;
            Background = GuiUtils.MakeCustomBackground(FormMain.Config.GetTexture(layer.NameTexture), Program.formMain.MainControl);
        }

        internal TypeLobbyLayerSettings Layer { get; }
        internal Bitmap Background { get; }

        internal override void Draw(Graphics g)
        {
            Text = Program.formMain.Settings.ShowShortNames ? Layer.Name : "";

            base.Draw(g);
        }
    }
}
