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
        private readonly Label namePlayer;
        private readonly Label labelAboutBattles;
        private readonly ImageList imageListResultBattle;

        public PanelAboutPlayer(Player player, ImageList ilResultBattle)
        {
            Player = player;
            BorderStyle = BorderStyle.FixedSingle;
            imageListResultBattle = ilResultBattle;

            pbFraction = new PictureBox()
            {
                Parent = this,
                Width = Program.formMain.ilPlayerAvatars.ImageSize.Width,
                Height = Program.formMain.ilPlayerAvatars.ImageSize.Height,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
            };

            pbResultBattle = new PictureBox()
            {
                Parent = this,
                Width = imageListResultBattle.ImageSize.Width,
                Height = imageListResultBattle.ImageSize.Height,
                Left = 240,
                Top = FormMain.Config.GridSize,
            };

            namePlayer = new Label()
            {
                Parent = this,
                Text = player.Name,
                Top = FormMain.Config.GridSize,
                Left = pbFraction.Width + (FormMain.Config.GridSize * 2),
                Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Bold)
            };

            labelAboutBattles = new Label()
            {
                Parent = this,
                Top = namePlayer.Top + (FormMain.Config.GridSize * 3),
                Left = pbFraction.Width + (FormMain.Config.GridSize * 2),
                AutoSize = true
            };

            Height = pbFraction.Height + (FormMain.Config.GridSize * 2);
            Width = pbResultBattle.Left + pbResultBattle.Width + FormMain.Config.GridSize;
        }

        internal void ShowData()
        {
            pbFraction.Image = Program.formMain.ilPlayerAvatars.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilPlayerAvatars, Player.ImageIndexAvatar, Player.IsLive)];

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
                Battle b = Player.HistoryBattles.Last();
                int ii = b.Winner == null ? 1 : b.Winner == Player ? 0 : 2;

                pbResultBattle.Image = imageListResultBattle.Images[ii];
            }
        }

        internal Player Player { get; }
    }
}
