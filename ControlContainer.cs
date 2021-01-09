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
    internal class ControlContainer
    {
        private Control parent;
        private int left;
        private int top;
        private int width;
        private int height;

        public ControlContainer()
        {
        }

        public ControlContainer(Control aParent)
        {
            parent = aParent;

            //DoubleBuffered = true;
            //BackColor = Color.Transparent;
            //TextAlign = ContentAlignment.TopCenter;
            //Visible = false;

            //BackgroundImage = GuiUtils.MakeBackground(Size);
        }

        internal Control Parent
        {
            get => parent;
            set
            {
                parent = value;

                foreach (KeyValuePair<Control, Point> p in Controls)
                {
                    p.Key.Parent = parent;
                }

                foreach (KeyValuePair<ControlContainer, Point> p in Containers)
                {
                    p.Key.Parent = parent;
                }
            }
        }
        internal int Left { get { return left; } set { left = value; ArrangeControlsAndContainers(); } }
        internal int Top { get { return top; } set { top = value; ArrangeControlsAndContainers(); } }
        internal int Width { get { return width; } set { width = value; } }
        internal int Height { get { return height; } set { height = value; } }
        internal Dictionary<Control, Point> Controls = new Dictionary<Control, Point>();
        internal Dictionary<ControlContainer, Point> Containers = new Dictionary<ControlContainer, Point>();

        private void ArrangeControlsAndContainers()
        {
            foreach (KeyValuePair<Control, Point> p in Controls)
            {
                ArrangeControl(p.Key, p.Value);
            }

            foreach (KeyValuePair<ControlContainer, Point> p in Containers)
            {
                ArrangeContainer(p);
            }
        }

        internal void ArrangeControl(Control c, Point p)
        {
            if (!Controls[c].Equals(p))
                Controls[c] = p;

            c.Left = Left + p.X;
            c.Top = Top + p.Y;
        }

        private void ArrangeContainer(KeyValuePair<ControlContainer, Point> p)
        {
            p.Key.Left = Left + p.Value.X;
            p.Key.Top = Top + p.Value.Y;
        }

        internal void AddControl(Control c, Point p)
        {
            Debug.Assert(c != null);
            Debug.Assert(c.Parent == null);

            Controls.Add(c, p);

            c.Visible = false;
            if (Parent != null)
                c.Parent = Parent;

            ArrangeControl(c, p);
        }

        internal void AddContainer(ControlContainer cc, Point p)
        {
            Debug.Assert(cc != this);

            Containers.Add(cc, p);

            if ((cc.Parent == null) && (parent != null))
                cc.Parent = parent;
            cc.SetVisible(false);
            ArrangeContainer(Containers.Last());
        }

        internal void SetVisible(bool visible)
        {
            foreach (KeyValuePair<Control, Point> c in Controls)
            {
                c.Key.Visible = visible;
            }

            foreach (KeyValuePair<ControlContainer, Point> cc in Containers)
            {
                cc.Key.SetVisible(visible);
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

        internal void Repaint()
        {
            foreach (KeyValuePair<Control, Point> p in Controls)
            {
                p.Key.Invalidate();
            }

            foreach (KeyValuePair<ControlContainer, Point> p in Containers)
            {
                p.Key.Repaint();
            }
        }
    }
}
