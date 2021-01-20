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

        public VisualControl(bool b)
        {
            ShiftOnParent = new Point(0, 0);
        }

        public VisualControl(VisualControl parent, Point shift)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent != this);

            ShiftOnParent = shift;
            parent.AddControl(this);
        }

        internal int Left { get { return left; } set { left = value; ArrangeControls(); } }
        internal int Top { get { return top; } set { top = value; ArrangeControls(); } }
        internal int Width { get { return width; } set { width = value; } }
        internal int Height { get { return height; } set { height = value; } }
        internal Point ShiftOnParent { get; set; }// Смещение контрола относительно левого верхнего края на родителе
        internal bool Visible { get; set; } = true;

        // Список контролов, расположенных на нём, со смещением относительно левого верхнего угла
        internal List<VisualControl> Controls = new List<VisualControl>();

        internal event EventHandler Click;
        internal event EventHandler ShowHint;

        // Метод для рисования. Передается Bitmap, подготовленный Graphics, смещение контрола относительно левого верхнего угла
        internal virtual void Draw(Graphics g, int x, int y)
        {
            foreach (VisualControl vc in Controls)
            {
                vc.Draw(g, x + vc.ShiftOnParent.X, y + vc.ShiftOnParent.Y);
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

        protected virtual void ArrangeControls()
        {
            foreach (VisualControl vc in Controls)
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
            foreach (VisualControl vc in Controls.Where(vc => vc.Visible))
            {                
                if (Utils.PointInRectagle(vc.ShiftOnParent.X, vc.ShiftOnParent.Y, vc.Width, vc.Height, left, top))
                    return vc;
            }

            return this;
        }

        private void ArrangeControl(VisualControl vc)
        {
            vc.Left = Left + vc.ShiftOnParent.X;
            vc.Top = Top + vc.ShiftOnParent.Y;

            vc.ArrangeControls();
        }

        internal void AddControl(VisualControl vc)
        {
            Debug.Assert(vc != null);
            Debug.Assert(vc != this);

            Controls.Add(vc);
        }

        internal virtual Size MaxSize()
        {
            Size maxSize = new Size(Width, Height);

            foreach (VisualControl vc in Controls)
            {
                maxSize.Width = Math.Max(maxSize.Width, vc.left + vc.Width);
                maxSize.Height = Math.Max(maxSize.Height, vc.top + vc.Height);
            }

            return maxSize;
        }
    }
}
