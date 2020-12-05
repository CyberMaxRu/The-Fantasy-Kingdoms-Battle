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
    internal sealed class PanelCellMenu : Label
    {
        private PlayerResearch research;
        private bool mouseOver;
        private bool mouseClicked;

        public PanelCellMenu(Control parent, Point location)
        {
            Parent = parent;
            Location = location;
            Size = Program.formMain.ilItems.ImageSize;
            BackColor = Color.Transparent;
            ForeColor = FormMain.Config.CommonCost;
            Font = FormMain.Config.FontCost;
            TextAlign = ContentAlignment.BottomCenter;
            Visible = false;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if ((e.Button == MouseButtons.Left) && research.CheckRequirements())
            {
                research.DoResearch();

                Program.formMain.UpdateMenu();

            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Program.formMain.formHint.Clear();
            Program.formMain.formHint.AddStep1Header(research.Research.Entity.Name, "", research.Research.Entity.Description);
            Program.formMain.formHint.AddStep3Requirement(research.GetTextRequirements());
            Program.formMain.formHint.AddStep4Gold(research.Cost(), research.Cost() <= research.Building.Player.Gold);
            Program.formMain.formHint.ShowHint(this);

            mouseOver = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Program.formMain.formHint.HideHint();

            mouseOver = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
                mouseClicked = e.Button == MouseButtons.Left;

            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (mouseClicked != false)
            {
                mouseClicked = false;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Рисуем иконку
            pe.Graphics.DrawImageUnscaled(Program.formMain.ilItems.Images[research.Research.Entity.ImageIndex], 0, 0);

            // Накладываем фильтр
            if (!research.CheckRequirements())
                pe.Graphics.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.Images[3], 0, 0);
            else if (mouseClicked)
                pe.Graphics.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.Images[2], 0, 0);
            else if (mouseOver)
                pe.Graphics.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.Images[1], 0, 0);
            else
                pe.Graphics.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.Images[0], 0, 0);

            // Рисуем цену
            base.OnPaint(pe);
        }

        internal PlayerResearch Research
        {
            get { return research; }
            set
            {
                research = value;
                Visible = research != null;
                if (Visible)
                    Text = research.Cost().ToString();

            }
        }
    }
}
