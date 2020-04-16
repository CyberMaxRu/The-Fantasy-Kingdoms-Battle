using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели, отображающей информацию об игроке
    internal sealed class PanelAboutPlayer : Panel
    {
        internal PictureBox pbFraction;
        internal Label namePlayer;
        public PanelAboutPlayer(Player player, ImageList il)
        {
            Player = player;
            BorderStyle = BorderStyle.FixedSingle;

            pbFraction = new PictureBox()
            {
                Parent = this,
                Width = il.ImageSize.Width,
                Height = il.ImageSize.Height,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Image = il.Images[player.Fraction.ImageIndex]
            };

            namePlayer = new Label()
            {
                Parent = this,
                Text = player.Name,
                Top = Config.GRID_SIZE,
                Left = pbFraction.Width + (Config.GRID_SIZE * 2),
                Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Bold)
            };

            Height = pbFraction.Height + (Config.GRID_SIZE * 2);
            Width = 320;

            if (player == player.Lobby.CurrentPlayer)
                BackColor = Color.LightBlue;
            if (player == player.Lobby.CurrentPlayer.Opponent)
                BackColor = Color.LightCoral;
        }

        internal void ShowData()
        {

        }

        internal Player Player { get; }
    }
}
