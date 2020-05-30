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
        private Label lblCost;

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

            lblCost = new Label()
            {
                Parent = this,
                Size = Size,
                BackColor = Color.Transparent,
                ForeColor = Program.formMain.ColorCost,
                Font = Program.formMain.fontCost,
                TextAlign = ContentAlignment.BottomCenter
            };

            lblCost.MouseEnter += LblCost_MouseEnter;
            lblCost.MouseLeave += LblCost_MouseLeave;
            lblCost.MouseClick += LblCost_MouseClick;
        }

        private void LblCost_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Left)
            {
                if (research.CheckRequirements())
                {
                    research.DoResearch();

                    Program.formMain.UpdateMenu();
                }
            }
        }

        private void LblCost_MouseLeave(object sender, EventArgs e)
        {
            Program.formMain.formHint.HideHint();
        }

        private void LblCost_MouseEnter(object sender, EventArgs e)
        {
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

        internal PlayerResearch Research
        {
            get { return research; }
            set
            {
                research = value;
                Visible = research != null;
                if (Visible)
                {
                    Image = Program.formMain.ilItems.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilItems, research.Research.Item.ImageIndex, research.CheckRequirements())];
                    lblCost.Text = research.Cost().ToString();
                }
            }
        }
    }
}
