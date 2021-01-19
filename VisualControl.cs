using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Базовый класс и класс-контейнер для всех визуальных контролов
    internal class VisualControl
    {
        private int left;
        private int top;
        private int width;
        private int height;

        public VisualControl()
        {
            //Visible = true;
        }

        internal int Left { get { return left; } set { left = value; ArrangeControlsAndContainers(); } }
        internal int Top { get { return top; } set { top = value; ArrangeControlsAndContainers(); } }
        internal int Width { get { return width; } set { width = value; } }
        internal int Height { get { return height; } set { height = value; } }
        internal bool Visible { get; set; } = true;

        // Список контролов, расположенных на нём, со смещением относительно левого верхнего угла
        internal Dictionary<VisualControl, Point> Controls = new Dictionary<VisualControl, Point>();

        internal event EventHandler Click;
        internal event EventHandler ShowHint;

        // Метод для рисования. Передается Bitmap, подготовленный Graphics, смещение контрола относительно левого верхнего угла
        internal virtual void Draw(Bitmap b, Graphics g, int x, int y)
        {
            foreach (KeyValuePair<VisualControl, Point> c in Controls)
            {
                c.Key.Draw(b, g, x + c.Value.X, y + c.Value.Y);
            }
        }

        internal virtual void DoClick()
        {
            Click?.Invoke(this, new EventArgs());
        }

        internal virtual bool PrepareHint()
        {
            return false;
        }

        internal virtual void DoShowHint()
        {
            if (ShowHint != null)
                ShowHint.Invoke(this, new EventArgs());
            else
            {
                Program.formMain.formHint.Clear();
                if (PrepareHint())
                    Program.formMain.formHint.ShowHint(Program.formMain.ctrlTransparent);
            }
        }

        protected virtual void ArrangeControlsAndContainers()
        {
            foreach (KeyValuePair<VisualControl, Point> vc in Controls)
            {
                ArrangeControl(vc);
            }
        }

        internal int NextTop()
        {
            return Top + Height + FormMain.Config.GridSize;
        }

        internal int NextLeft()
        {
            return Left + Width + FormMain.Config.GridSize;
        }

        internal virtual VisualControl GetControl(int left, int top)
        {
            return this;
        }

        private void ArrangeControl(KeyValuePair<VisualControl, Point> p)
        {
            p.Key.Left = Left + p.Value.X;
            p.Key.Top = Top + p.Value.Y;
        }

        internal void AddControl(VisualControl cc, Point p)
        {
            Debug.Assert(cc != this);

            Controls.Add(cc, p);
            //ArrangeControl(Controls.Last());
        }

        internal virtual Size MaxSize()
        {
            Size maxSize = new Size(Width, Height);

            foreach (KeyValuePair<VisualControl, Point> c in Controls)
            {
                maxSize.Width = Math.Max(maxSize.Width, c.Value.X + c.Key.Width);
                maxSize.Height = Math.Max(maxSize.Height, c.Value.Y + c.Key.Height);
            }

            return maxSize;
        }
    }
}
