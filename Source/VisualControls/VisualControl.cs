using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс и класс-контейнер для всех визуальных контролов
    internal class VisualControl : IDisposable
    {
        private int left;// Координата Left на главном окне (абсолютная)
        private int top;// Координата Top на главном окне (абсолютная)
        private int width;// Ширина контрола
        private int height;// Высота контрола

        private Bitmap bmpBorder;
        private Bitmap bmpBorderSelect;

        private bool _disposed = false;

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

        ~VisualControl() => Dispose(false);

        internal VisualControl Parent { get; private set; }
        internal int Left { get { return left; } private set { left = value; ValidateRectangle(); ArrangeControls(); } }
        internal int Top { get { return top; } private set { top = value; ValidateRectangle(); ArrangeControls(); } }
        internal int Width { get { return width; } set { width = value; ValidateRectangle(); } }
        internal int Height { get { return height; } set { height = value; ValidateRectangle(); } }
        internal int ShiftX { get; set; }// Смещение контрола относительно левого края на родителе
        internal int ShiftY { get; set; }// Смещение контрола относительно верхнего края на родителе
        internal Rectangle Rectangle { get; private set; }// Координаты и размер контрола
        internal bool Visible { get; set; } = true;// Видимость контрола
        internal bool ManualDraw { get; set; }// Ручное рисование контрола
        internal bool ShowBorder { get; set; }// Надо ли показывать бордюр
        internal bool Selected { get; set; }// Контрол выбран

        protected bool MouseEntered { get; set; }// Курсор мыши находится над контролом

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

        // Метод для рисования фона - то есть то, что будет перекрываться изображением через Draw
        internal virtual void DrawBackground(Graphics g)
        {
            if (Selected)
            {
                if ((bmpBorderSelect == null) || (bmpBorderSelect.Width != Width + 16) || (bmpBorderSelect.Height != Height + 16))
                {
                    bmpBorderSelect?.Dispose();
                    bmpBorderSelect = Program.formMain.bbSelect.DrawBorder(Width + 16, Height + 16);
                }

                g.DrawImageUnscaled(bmpBorderSelect, Left - 8, Top - 8);
            }

            // Рисуем контролы
            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                    vc.DrawBackground(g);
            }
        }

        // Метод для рисования. Передается подготовленный Graphics
        internal virtual void Draw(Graphics g)
        {
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
                Program.formMain.formHint.DrawHint(this);
            }
            else
            {
                Program.formMain.formHint.Clear();
                if (PrepareHint())
                    Program.formMain.formHint.DrawHint(this);
            }
        }

        internal virtual void MouseEnter(bool leftButtonDown)
        {
            Debug.Assert(!MouseEntered);

            MouseEntered = true;
        }

        internal virtual void MouseLeave()
        {
            Debug.Assert(MouseEntered);

            MouseEntered = false;
        }

        internal virtual void MouseDown() { }
        internal virtual void MouseUp() { }
        internal virtual void KeyUp(KeyEventArgs e) { }

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

        internal void RemoveControl(VisualControl vc)
        {
            Debug.Assert(vc != null);
            Debug.Assert(vc != this);
            Debug.Assert(Controls.IndexOf(vc) != -1);

            Controls.Remove(vc);
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            { 
                if (Parent != null)
                {
                    Parent.RemoveControl(this);
                    Parent = null;
                }

                if (disposing)
                {
                    bmpBorder?.Dispose();
                    bmpBorderSelect?.Dispose();
                }

                bmpBorder = null;
                bmpBorderSelect = null;

                _disposed = true;
            }
        }
    }
}
