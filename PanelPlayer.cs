using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс панели игрока лобби
    internal sealed class PanelPlayer : VisualControl
    {
        private Player player;
        private PanelEntity panelAvatar;
        private Rectangle rectBorder;
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);
        private readonly SolidBrush brushCurDurability = new SolidBrush(FormMain.Config.BattlefieldPlayerHealth);
        private readonly SolidBrush brushMaxDurability = new SolidBrush(FormMain.Config.BattlefieldPlayerHealthNone);
        private readonly SolidBrush brushBackground = new SolidBrush(Color.White);

        public PanelPlayer(int imageIndex) : base()
        {
            panelAvatar = new PanelEntity();
            AddControl(panelAvatar, new Point(FormMain.Config.GridSize, FormMain.Config.GridSize));

            Width = panelAvatar.Width + (FormMain.Config.GridSize * 2);
            Height = panelAvatar.Height + (FormMain.Config.GridSize * 2);
        }

        internal Player Player { get { return player; }
            set { player = value; player.Panel = this; panelAvatar.ShowCell(player); /*Refresh()*/; } }

        protected void OnPaint(PaintEventArgs e)
        {
            if (player == null)
                return;

            Debug.Assert(Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            Debug.Assert(player.LastBattleDamageToCastle >= -999);
            Debug.Assert(player.LastBattleDamageToCastle <= 999);

            // Фон панели
            if (player == player.Lobby.CurrentPlayer)
                brushBackground.Color = Color.LightBlue;
            else if (player == player.Lobby.CurrentPlayer.Opponent)
                brushBackground.Color = Color.LightCoral;
            else
                brushBackground.Color = Color.FromKnownColor(KnownColor.Control);

            //e.Graphics.FillRectangle(brushBackground, rectBorder);

            // Рамка вокруг панели
            penBorder.Color = player == player.Lobby.CurrentPlayer ? FormMain.Config.SelectedPlayerBorder : FormMain.Config.CommonBorder;
            e.Graphics.DrawRectangle(penBorder, rectBorder);

            // Прочность замка
            int dur = player.DurabilityCastle >= 0 ? player.DurabilityCastle : 0;
            GuiUtils.DrawBand(e.Graphics, new Rectangle(FormMain.Config.GridSize, FormMain.Config.GridSize + panelAvatar.Height + 1, panelAvatar.Width, 6), brushCurDurability, brushMaxDurability, dur, player.Lobby.TypeLobby.DurabilityCastle);

        }

        protected override void ArrangeControlsAndContainers()
        {
            rectBorder = new Rectangle(Left, Top, Width - 1, Height - 1);
        }

        internal override void Draw(Graphics g, int x, int y)
        {
            // Рамка вокруг панели
            penBorder.Color = player == player.Lobby.CurrentPlayer ? FormMain.Config.SelectedPlayerBorder : FormMain.Config.CommonBorder;
            g.DrawRectangle(penBorder, rectBorder);

            // Аватар игрока
            panelAvatar.Left = x + Controls[panelAvatar].X;
            panelAvatar.Top = y + Controls[panelAvatar].Y;
            panelAvatar.Draw(g, panelAvatar.Left, panelAvatar.Top);
        }

        internal override void DoClick()
        {

        }

        internal override bool PrepareHint()
        {
            return false;
        }

        internal override VisualControl GetControl(int x, int y)
        {
            if (Utils.PointInRectagle(Controls[panelAvatar].X, Controls[panelAvatar].Y, panelAvatar.Width, panelAvatar.Height, x, y))
                return panelAvatar;

            return this;
        }
    }
}
