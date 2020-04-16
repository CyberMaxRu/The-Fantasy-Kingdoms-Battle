using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;   

namespace Fantasy_King_s_Battle
{
    // Класс внешнего здания
    internal sealed class PanelExternalBuilding : Panel
    {
        private readonly PictureBox pbImage;
        private readonly Label lblResource;
        public PanelExternalBuilding(Player player, BuildingOfPlayer building, ImageList ilBuilding, ImageList ilResources)
        {
            Player = player;
            Building = building;

            //BorderStyle = BorderStyle.FixedSingle;

            pbImage = new PictureBox()
            {
                Parent = this,
                Width = ilBuilding.ImageSize.Width,
                Height = ilBuilding.ImageSize.Height,
                Image = ilBuilding.Images[building.Building.ImageIndex]
            };

            lblResource = new Label()
            {
                Parent = this,
                Top = pbImage.Height,
                AutoSize = false,
                Width = pbImage.Width,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                ImageList = ilResources
            };

            Height = pbImage.Height + lblResource.Height;
            Width = pbImage.Width + (Config.GRID_SIZE * 2);
        }

        internal void ShowData()
        {
            lblResource.ImageIndex = -1;

            foreach (Resource r in FormMain.Config.Resources)
            {
                if (Building.Building.IncomeResources[r.Position] > 0)
                {
                    // Нет поддержка отображения более одного ресурса
                    Debug.Assert(lblResource.ImageIndex == -1);

                    lblResource.ImageIndex = r.ImageIndex;
                    lblResource.Text = Building.Building.IncomeResources[r.Position].ToString();
                }
            }
        }

        internal Player Player { get; }
        internal BuildingOfPlayer Building { get; }
    }
}