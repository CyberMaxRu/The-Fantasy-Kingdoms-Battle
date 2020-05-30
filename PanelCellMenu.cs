using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    // Класс панели исследования
    internal sealed class PanelCellMenu : PictureBox
    {
        private PlayerResearch research;
        public PanelCellMenu(Control parent, int left, int top)
        {
            Parent = parent;
            Left = left;
            Top = top;
            Size = Program.formMain.ilItems.ImageSize;
            SizeMode = PictureBoxSizeMode.Normal;
            BackColor = Color.Transparent;
            //DoubleBuffered = true;
            Visible = false;
        }

        internal PlayerResearch Research
        {
            get { return research; } 
            set { research = value; 
                Visible = research != null;
                if (Visible) Image = Program.formMain.ilItems.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilItems, research.Research.Item.ImageIndex, research.CheckRequirements())]; } 
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (research != null)
            {
                Program.formMain.formHint.ShowHint(new Point(8 + Parent.Left + Left, Parent.Top + Top + Height + 2),
                    research.Research.Item.Name,
                    "",
                    research.Research.Item.Description,
                    research.GetTextRequirements(),
                    research.Cost(),
                    research.Cost() <= research.Building.Player.Gold,
                    0,
                    0, false, null);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Program.formMain.formHint.HideHint();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Left)
            {
                if (research.CheckRequirements())
                    research.DoResearch();
            }
        }
    }
}
