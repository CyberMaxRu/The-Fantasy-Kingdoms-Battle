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

        private Bitmap bmpBorder;
        private readonly Pen penBorder = new Pen(FormMain.Config.CommonBorder);
        private Rectangle rectBorder;

        public VisualControl()
        {            
        }

        public VisualControl(VisualLayer vl)
        {
            vl.AddControl(this);
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

        // Список контролов, расположенных на нём
        internal List<VisualControl> Controls = new List<VisualControl>();

        internal event EventHandler Click;
        internal event EventHandler ShowHint;

        internal void SetPos(int left, int top)
        {
            Debug.Assert(Parent == null);
            Left = left;
            Top = top;
        }

        // Метод для рисования. Передается подготовленный Graphics
        internal virtual void Draw(Graphics g)
        {
            Debug.Assert(rectBorder.Left == Left);
            Debug.Assert(rectBorder.Top == Top);
            Debug.Assert(rectBorder.Width == Width - 1);
            Debug.Assert(rectBorder.Height == Height - 1);

            // Рисуем бордюр
            if (ShowBorder)
            {
                PrepareBorder();
                g.DrawImageUnscaled(bmpBorder, Left - 2, Top);
            }
            //g.DrawRectangle(penBorder, rectBorder);

            // Рисуем контролы
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

        private void PrepareBorder()
        {
            if (ShowBorder)
            {
                if ((bmpBorder == null) || (bmpBorder.Size.Width != Width + 4) || (bmpBorder.Size.Height != Height + 3))
                {
                    bmpBorder?.Dispose();
                    bmpBorder = Program.formMain.bbObject.DrawBorder(Width + 4, Height + 3);
                }
            }
            else
            {
                if (bmpBorder != null)
                {
                    bmpBorder?.Dispose();
                    bmpBorder = null;
                }
            }
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

        internal virtual void MouseEnter(bool leftButtonDown) { }
        internal virtual void MouseLeave() { }
        internal virtual void MouseDown() { }
        internal virtual void MouseUp() { }

        internal virtual void ArrangeControls()
        {
            foreach (VisualControl vc in Controls)
            {
                ArrangeControl(vc);
            }
        }

        internal virtual VisualControl GetControl(int x, int y)
        {
            if (!Visible)
                return null;

            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                {
                    VisualControl ivc = vc.GetControl(x, y);
                    if (ivc != null)
                        return ivc;

                    if (vc.Rectangle.Contains(x, y))
                        return vc;
                }
            }

            return Rectangle.Contains(x, y) ? this : null;
        }

        internal void ArrangeControl(VisualControl vc, bool checkInList = true)
        {
            Debug.Assert(vc != this);

            if (checkInList)
            {
                Debug.Assert(Controls.IndexOf(vc) != -1);
            }

            // Надобности вроде особой неты
            //Debug.Assert(Height > 0);
            //Debug.Assert(Width > 0);

            vc.Left = Left + vc.ShiftX;
            vc.Top = Top + vc.ShiftY;

            Debug.Assert(Left >= 0);
            Debug.Assert(Top >= 0);

            vc.ArrangeControls();
        }

        internal int NextLeft()
        {
            return ShiftX + Width + FormMain.Config.GridSize;
        }

        internal void PlaceBeforeControl(VisualControl vc)
        {
            ShiftX = vc.ShiftX - Width - FormMain.Config.GridSize;
        }

        internal int NextTop()
        {
            return ShiftY + Height + FormMain.Config.GridSize;
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

        internal void SetColorBorder(Color color)
        {
            penBorder.Color = color;
        }
    }
}
