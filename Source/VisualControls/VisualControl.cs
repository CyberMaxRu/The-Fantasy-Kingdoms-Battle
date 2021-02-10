using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс и класс-контейнер для всех визуальных контролов
    internal class VisualControl
    {
        private int left;
        private int top;
        private int width;
        private int height;

        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);
        private Rectangle rectBorder;

        public VisualControl()
        {
        }

        public VisualControl(VisualControl parent, int shiftX, int shiftY)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent != this);

            ShiftX = shiftX;
            ShiftY = shiftY;
            parent.AddControl(this);
        }

        internal VisualControl Parent { get; private set; }
        internal int Left { get { return left; } private set { left = value; ValidateRectangle(); ArrangeControls(); } }
        internal int Top { get { return top; } private set { top = value; ValidateRectangle(); ArrangeControls(); } }
        internal int Width { get { return width; } set { width = value; ValidateRectangle(); } }
        internal int Height { get { return height; } set { height = value; ValidateRectangle(); } }
        internal int ShiftX { get; set; }// Смещение контрола относительно левого края на родителе
        internal int ShiftY { get; set; }// Смещение контрола относительно верхнего края на родителе
        internal Rectangle Rectangle { get; private set; }// Координаты и размер контрола
        internal bool Visible { get; set; } = true;// Видимость контрола
        internal bool ShowBorder { get; set; }// Надо ли показывать бордюр
        internal Color ColorBorder { get; set; } = FormMain.Config.CommonBorder;// Цвет бордюра

        // Список контролов, расположенных на нём
        internal List<VisualControl> Controls = new List<VisualControl>();

        internal event EventHandler Click;
        internal event EventHandler ShowHint;

        // Метод для рисования. Передается подготовленный Graphics
        internal virtual void Draw(Graphics g)
        {
            Debug.Assert(rectBorder.Left == Left);
            Debug.Assert(rectBorder.Top == Top);
            Debug.Assert(rectBorder.Width == Width - 1);
            Debug.Assert(rectBorder.Height == Height - 1);

            // Рамка вокруг панели
            if (ShowBorder)
            {
                penBorder.Color = ColorBorder;
                g.DrawRectangle(penBorder, rectBorder);
            }

            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                    vc.Draw(g);
            }
        }

        internal virtual void DoClick()
        {
            Click?.Invoke(this, new EventArgs());
        }

        protected virtual void ValidateRectangle()
        {
            if ((Rectangle.Left != Left) || (Rectangle.Top != Top) || (Rectangle.Width != Width) || (Rectangle.Height != Height))
            {
                Rectangle = new Rectangle(Left, Top, Width, Height);
                rectBorder = new Rectangle(Left, Top, Width - 1, Height - 1);
            }
        }

        internal virtual bool PrepareHint()
        {
            return false;
        }

        internal virtual void DoShowHint()
        {
            if (ShowHint != null)
            {
                Program.formMain.formHint.Clear();
                ShowHint.Invoke(this, new EventArgs());
                Program.formMain.formHint.ShowHint(this);
            }
            else
            {
                Program.formMain.formHint.Clear();
                if (PrepareHint())
                    Program.formMain.formHint.ShowHint(this);
            }
        }

        internal virtual void MouseEnter()
        {
        }

        internal virtual void MouseLeave()
        {
        }

        internal virtual void ArrangeControls()
        {
            foreach (VisualControl vc in Controls)
            {
                ArrangeControl(vc);
            }
        }

        internal int NextLeft()
        {
            return ShiftX + Width + FormMain.Config.GridSize;
        }

        internal int NextTop()
        {
            return ShiftY + Height + FormMain.Config.GridSize;
        }

        internal virtual VisualControl GetControl(int x, int y)
        {
            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                {
                    VisualControl ivc = vc.GetControl(x, y);
                    if (ivc != vc)
                        return ivc;

                    if (vc.Rectangle.Contains(x, y))
                        return vc;
                }
            }

            return this;
        }

        protected void ArrangeControl(VisualControl vc)
        {
            vc.Left = Left + vc.ShiftX;
            vc.Top = Top + vc.ShiftY;

            vc.ArrangeControls();
        }

        internal void AddControl(VisualControl vc)
        {
            Debug.Assert(vc != null);
            Debug.Assert(vc != this);

            Controls.Add(vc);
            if (vc.Parent == null)
                vc.Parent = this;
        }

        internal virtual Size MaxSize()
        {
            Size maxSize = new Size(Width, Height);

            foreach (VisualControl vc in Controls)
            {
                maxSize.Width = Math.Max(maxSize.Width, vc.ShiftX + vc.Width);
                maxSize.Height = Math.Max(maxSize.Height, vc.ShiftY + vc.Height);
            }

            return maxSize;
        }

        internal virtual void ApplyMaxSize()
        {
            Size s = MaxSize();
            Width = s.Width;
            Height = s.Height;
        }
    }
}
