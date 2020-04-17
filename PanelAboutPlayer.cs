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
        private readonly PictureBox pbFraction;
        private readonly Label namePlayer;
        private readonly Label labelAboutBattles;
        private readonly ImageList imageList;

        public PanelAboutPlayer(Player player, ImageList il)
        {
            Player = player;
            BorderStyle = BorderStyle.FixedSingle;
            imageList = il;

            pbFraction = new PictureBox()
            {
                Parent = this,
                Width = il.ImageSize.Width,
                Height = il.ImageSize.Height,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
            };

            namePlayer = new Label()
            {
                Parent = this,
                Text = player.Name,
                Top = Config.GRID_SIZE,
                Left = pbFraction.Width + (Config.GRID_SIZE * 2),
                Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Bold)
            };

            labelAboutBattles = new Label()
            {
                Parent = this,
                Top = namePlayer.Top + (Config.GRID_SIZE * 3),
                Left = pbFraction.Width + (Config.GRID_SIZE * 2),
                AutoSize = true
            };

            Height = pbFraction.Height + (Config.GRID_SIZE * 2);
            Width = 320;
        }

        internal void ShowData()
        {
            if (Player.IsLive == true)
                pbFraction.Image = imageList.Images[Player.Fraction.ImageIndex];
            else
                pbFraction.Image = imageList.Images[Player.Fraction.ImageIndex + FormMain.Config.Fractions.Count];

            if (Player == Player.Lobby.CurrentPlayer)
                BackColor = Color.LightBlue;
            else if (Player == Player.Lobby.CurrentPlayer.Opponent)
                BackColor = Color.LightCoral;
            else
                BackColor = Color.FromKnownColor(KnownColor.Control);

            labelAboutBattles.Text = "Сражений: " + (Player.Wins + Player.Loses + Player.Draws).ToString() + " (" + Player.Wins.ToString()
                + "/" + Player.Draws.ToString() + "/" + Player.Loses.ToString() + ")";
        }

        internal Player Player { get; }
    }
}
