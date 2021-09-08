using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - результат раунда боев между игроками
    internal sealed class VCResultRound : VisualControl
    {
        private VCText lblDay;
        private List<VCImage> listImages = new List<VCImage>();

        public VCResultRound(VisualControl parent, int shiftX, int shiftY, int players) : base(parent, shiftX, shiftY)
        {
            Visible = false;
            lblDay = new VCText(this, 0, 0, Program.formMain.fontSmallC, Color.White, 48);
            lblDay.Height = lblDay.Width;
            lblDay.ShowBorder = true;
            lblDay.StringFormat.Alignment = StringAlignment.Center;
            lblDay.StringFormat.LineAlignment = StringAlignment.Center;

            //
            VCImage24 img;
            int nextTop = lblDay.NextTop();

            for (int i = 0; i < players; i++)
            {
                VisualControl panel = new VisualControl(this, 0, nextTop);
                panel.Width = Program.formMain.imListObjects48.Size;
                panel.Height = Program.formMain.imListObjects48.Size;
                img = new VCImage24(panel, 0, 0, -1);
                img.ShiftX = (panel.Width - img.Width) / 2;
                img.ShiftY = (panel.Height - img.Height) / 2;
                nextTop = panel.NextTop();
                listImages.Add(img);
            }
        }

        internal void ShowPlayers(Player[] players, BattlesPlayers battlesPlayers)
        {
            Visible = true;
            lblDay.Text = battlesPlayers.Day.ToString() + Environment.NewLine + "день";

            foreach (Player lp in players.OrderBy(lp => lp.PositionInLobby))
            {
                // Находим результат боя игрока
                if (battlesPlayers.Players.ContainsKey(lp))
                {
                    listImages[lp.PositionInLobby - 1].ImageIndex = battlesPlayers.Players[lp] ? FormMain.GUI_24_TRANSP_WIN : FormMain.GUI_24_TRANSP_LOSE;
                    //listImages[lp.PositionInLobby - 1].ShowBorder = true;
                }
                else
                {
                    listImages[lp.PositionInLobby - 1].ImageIndex = -1;
                    //listImages[lp.PositionInLobby - 1].ShowBorder = false;
                }
            }
        }
    }
}
