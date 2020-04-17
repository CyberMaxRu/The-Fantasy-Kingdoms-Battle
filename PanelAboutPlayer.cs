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
        private readonly PictureBox pbResultBattle;
        private readonly PictureBox pbTypeBattle;
        private readonly Label namePlayer;
        private readonly Label labelAboutBattles;
        private readonly ImageList imageListFraction;
        private readonly ImageList imageListResultBattle;
        private readonly ImageList imageListTypeBattle;

        public PanelAboutPlayer(Player player, ImageList ilFraction, ImageList ilResultBattle, ImageList ilTypeBattle)
        {
            Player = player;
            BorderStyle = BorderStyle.FixedSingle;
            imageListFraction = ilFraction;
            imageListResultBattle = ilResultBattle;
            imageListTypeBattle = ilTypeBattle;

            pbFraction = new PictureBox()
            {
                Parent = this,
                Width = ilFraction.ImageSize.Width,
                Height = ilFraction.ImageSize.Height,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
            };

            pbResultBattle = new PictureBox()
            {
                Parent = this,
                Width = imageListResultBattle.ImageSize.Width,
                Height = imageListResultBattle.ImageSize.Height,
                Left = 240,
                Top = Config.GRID_SIZE,
            };

            pbTypeBattle = new PictureBox()
            {
                Parent = this,
                Width = imageListTypeBattle.ImageSize.Width,
                Height = imageListTypeBattle.ImageSize.Height,
                Left = pbResultBattle.Left + pbResultBattle.Width + Config.GRID_SIZE,
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
            Width = pbTypeBattle.Left + pbTypeBattle.Width + Config.GRID_SIZE;
        }

        internal void ShowData()
        {
            if (Player.IsLive == true)
                pbFraction.Image = imageListFraction.Images[Player.Fraction.ImageIndex];
            else
                pbFraction.Image = imageListFraction.Images[Player.Fraction.ImageIndex + FormMain.Config.Fractions.Count];

            if (Player == Player.Lobby.CurrentPlayer)
                BackColor = Color.LightBlue;
            else if (Player == Player.Lobby.CurrentPlayer.Opponent)
                BackColor = Color.LightCoral;
            else
                BackColor = Color.FromKnownColor(KnownColor.Control);

            labelAboutBattles.Text = "Сражений: " + (Player.Wins + Player.Loses + Player.Draws).ToString() + " (" + Player.Wins.ToString()
                + "/" + Player.Draws.ToString() + "/" + Player.Loses.ToString() + ")";

            if (Player.HistoryBattles.Count > 0)
            { 
                CourseBattle cb = Player.HistoryBattles.Last();
                int ii = cb.Winner == null ? 1 : cb.Winner == Player ? 0 : 2;

                pbResultBattle.Image = imageListResultBattle.Images[ii];
            }

            pbTypeBattle.Image = imageListTypeBattle.Images[0];
        }

        internal Player Player { get; }
    }
}
