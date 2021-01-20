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

        internal Player Player
        {
            get { return player; }
            set { player = value; player.Panel = this; panelAvatar.ShowCell(player); }
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
