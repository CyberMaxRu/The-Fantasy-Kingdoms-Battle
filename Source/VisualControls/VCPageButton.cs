using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс кнопки для PageControl'а
    internal sealed class VCPageButton : VCIconButton48
    {
        public VCPageButton(VisualControl parent, int shiftX, int shiftY, int imageIndex, string caption, string advice, TypeLobbyLocationSettings layer) : base(parent, shiftX, shiftY, imageIndex)
        {
            HighlightUnderMouse = true;
            ShowBorder = true;
            Caption = caption;
            Hint = caption;
            HintDescription = advice;
            Advice = advice;
            Layer = layer;
            PlaySoundOnClick = true;

            Page = new VisualControl(parent, 0, NextTop());
            Page.Visible = false;
            Page.Click += Page_Click;
        }

        internal VisualControl Page { get; }
        internal string Caption { get; set; }
        internal string Advice { get; }
        internal Bitmap PageImage { get; set; }
        internal BigEntity SelectedPlayerObject { get; set; }
        internal TypeLobbyLocationSettings Layer { get; }
        internal Location Location { get; private set; }

        internal override void Draw(Graphics g)
        {
            //if (Location != null)
            //    ImageIsEnabled = Location.Ownership;

            base.Draw(g);
        }

        private void Page_Click(object sender, EventArgs e)
        {
            Program.formMain.layerGame.SelectPlayerObject(null);
        }

        internal void UpdateLairs(Player player)
        {
        }
    }
}
