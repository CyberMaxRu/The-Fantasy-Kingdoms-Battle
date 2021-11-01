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
        private PanelConstruction[,] constructions;

        public VCPageButton(VisualControl parent, int shiftX, int shiftY, int imageIndex, string caption, string advice, TypeLobbyLocationSettings layer) : base(parent, shiftX, shiftY, imageIndex)
        {
            HighlightUnderMouse = true;
            ShowBorder = true;
            Caption = caption;
            Advice = advice;
            Layer = layer;

            Page = new VisualControl(parent, 0, NextTop());
            Page.Visible = false;
            Page.Click += Page_Click;

            if (layer != null)
            {
                PageImage = GuiUtils.MakeCustomBackground(FormMain.Config.GetTexture(layer.TypeLandscape.NameTexture), Program.formMain.MainControl);

                constructions = new PanelConstruction[layer.TypeLobby.LairsHeight, layer.TypeLobby.LairsWidth];

                int top = 0;
                int left;
                int height = 0;

                for (int y = 0; y < layer.TypeLobby.LairsHeight; y++)
                {
                    left = 0;
                    for (int x = 0; x < layer.TypeLobby.LairsWidth; x++)
                    {
                        Debug.Assert(constructions[y, x] == null);
                        constructions[y, x] = new PanelConstruction(Page, left, top);

                        left += constructions[y, x].Width + FormMain.Config.GridSize;
                        height = constructions[y, x].Height;
                    }

                    top += height + FormMain.Config.GridSize;
                }

                Page.ApplyMaxSize();
            }
        }

        internal VisualControl Page { get; }
        internal string Caption { get; }
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
            Program.formMain.SelectPlayerObject(null);
        }

        internal void UpdateLairs(Player player)
        {
            Location = player.Locations[Layer.Number];

            for (int y = 0; y < Layer.TypeLobby.LairsHeight; y++)
                for (int x = 0; x < Layer.TypeLobby.LairsWidth; x++)
                {
                    constructions[y, x].Entity = player.Locations[Layer.Number].Lairs[y, x];
                    constructions[y, x].Visible = constructions[y, x].Entity != null;
                }
        }
    }
}
