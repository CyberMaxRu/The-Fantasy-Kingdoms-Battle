using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс панели с информацией об отряде
    internal sealed class PanelSquad : Panel
    {
        private readonly PictureBox pbImage;
        public PanelSquad(Squad squad)
        {
            Squad = squad;

            //BorderStyle = BorderStyle.FixedSingle;            
            pbImage = new PictureBox()
            {
                Parent = this,
                Width = squad.TypeUnit.Fraction.ILTypeUnits.ImageSize.Width,
                Height = squad.TypeUnit.Fraction.ILTypeUnits.ImageSize.Height,
                Image = squad.TypeUnit.Fraction.ILTypeUnits.Images[squad.TypeUnit.ImageIndex]
            };

            Height = pbImage.Height;
            Width = pbImage.Width + (Config.GRID_SIZE * 2);
        }

        internal void ShowData()
        {

        }

        internal Squad Squad { get; }
    }
}
