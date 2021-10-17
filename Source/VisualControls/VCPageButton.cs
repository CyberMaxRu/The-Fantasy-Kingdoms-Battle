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
        private PanelConstruction[,] panelLairs;

        public VCPageButton(VisualControl parent, int shiftX, int shiftY, int imageIndex, string caption, string advice, TypeLobbyLayerSettings layer) : base(parent, shiftX, shiftY, imageIndex)
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
                PageImage = GuiUtils.MakeCustomBackground(FormMain.Config.GetTexture(layer.NameTexture), Program.formMain.MainControl);

                panelLairs = new PanelConstruction[layer.TypeLobby.LairsHeight, layer.TypeLobby.LairsWidth];

                int top = 0;
                int left;
                int height = 0;

                for (int y = 0; y < layer.TypeLobby.LairsHeight; y++)
                {
                    left = 0;
                    for (int x = 0; x < layer.TypeLobby.LairsWidth; x++)
                    {
                        Debug.Assert(panelLairs[y, x] == null);
                        panelLairs[y, x] = new PanelConstruction(Page, left, top);

                        left += panelLairs[y, x].Width + FormMain.Config.GridSize;
                        height = panelLairs[y, x].Height;
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
        internal TypeLobbyLayerSettings Layer { get; }

        private void Page_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(null);
        }

        internal override void Draw(Graphics g)
        {
            if (Layer != null)
                Text = Program.formMain.Settings.ShowShortNames ? Layer.Name : "";

            base.Draw(g);
        }

        internal void UpdateLairs(Lobby lobby)
        {
            for (int y = 0; y < Layer.TypeLobby.LairsHeight; y++)
                for (int x = 0; x < Layer.TypeLobby.LairsWidth; x++)
                {
                    panelLairs[y, x].Entity = lobby.CurrentPlayer.Lairs[Layer.Number, y, x];
                    panelLairs[y, x].Visible = panelLairs[y, x].Entity != null;
                }
        }
    }
}
