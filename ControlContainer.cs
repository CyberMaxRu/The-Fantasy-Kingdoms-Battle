using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal sealed class ControlContainer
    {
        private Dictionary<Control, Point> Controls = new Dictionary<Control, Point>();
        private int left;
        private int top;

        public ControlContainer(Control parent)
        {
            Parent = parent;

            //DoubleBuffered = true;
            //BackColor = Color.Transparent;
            //TextAlign = ContentAlignment.TopCenter;
            //Visible = false;

            //BackgroundImage = GuiUtils.MakeBackground(Size);
        }

        internal Control Parent { get; }
        internal int Left { get { return left; } set { left = value; ArrangeControls(); } }
        internal int Top { get { return top; } set { top = value; ArrangeControls(); } }

        private void ArrangeControls()
        {
            foreach (KeyValuePair<Control, Point> p in Controls)
            {
                ArrangeControl(p);
            }
        }

        private void ArrangeControl(KeyValuePair<Control, Point> p)
        {
            p.Key.Left = Left + p.Value.X;
            p.Key.Top = Top + p.Value.Y;
        }

        internal void AddControl(Control c, Point p)
        {
            Controls.Add(c, p);

            c.Visible = false;
            c.Parent = Parent;
            ArrangeControl(Controls.Last());
        }

        internal void SetVisible(bool visible)
        {
            foreach (KeyValuePair<Control, Point> c in Controls)
            {
                c.Key.Visible = visible;
            }
        }

        internal Size MaxSize()
        {
            Size maxSize = new Size();
            foreach (KeyValuePair<Control, Point> c in Controls)
            {
                maxSize.Width = Math.Max(maxSize.Width, c.Key.Left + c.Key.Width);
                maxSize.Height = Math.Max(maxSize.Height, c.Key.Top + c.Key.Height);
            }

            return maxSize;
        }
    }
}
