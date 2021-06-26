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
            VisualLayer = vl;
            vl.AddControl(this);
        }

        public VisualControl(VisualControl parent, int shiftX, int shiftY)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent != this);

            ShiftX = shiftX;
            ShiftY = shiftY;
            parent.AddControl(this);
            VisualLayer = parent.VisualLayer;
            //Debug.Assert(VisualLayer != null);
        }

        ~VisualControl() => Dispose(false);

        internal VisualControl Parent { get; private set; }
        internal VisualLayer VisualLayer { get; }
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
        internal PlayerObject PlayerObject { get; set; }// Объект, ассоциированный с контролом
        internal bool MouseEntered { get; private set; }// Курсор мыши находится над контролом
        internal bool LeftButtonPressed { get; private set; }// ЛКМ нажата
        internal bool IsError { get; set; }
        internal bool ShowHintParent { get; set; }// Показывать подсказку родителя

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
            if (Selected())
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

            if (IsError)
                g.FillRectangle(FormMain.Config.brushControl, Rectangle);

            // Рисуем контролы
            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                    vc.Draw(g);
            }
        }

        internal virtual void DoClick()
        {
            Debug.Assert(Visible);

            if (AllowClick())
                Click?.Invoke(this, new EventArgs());
        }

        protected virtual bool Selected() => false;
        protected virtual bool AllowClick() => true;

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
            return ShowHintParent && Parent.PrepareHint();
        }

        internal virtual void DoShowHint()
        {
            if (ShowHintParent && Parent.ShowHint != null)
            {
                Program.formMain.formHint.Clear();
                Parent.ShowHint.Invoke(this, new EventArgs());
                Program.formMain.formHint.DrawHint(Parent);
            }
            else if (ShowHint != null)
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
            LeftButtonPressed = leftButtonDown;
        }

        internal virtual void MouseLeave()
        {
            Debug.Assert(MouseEntered);

            MouseEntered = false;
            LeftButtonPressed = false;
        }

        internal virtual void MouseDown()
        {
            Debug.Assert(Visible);

            LeftButtonPressed = true;
        }

        internal virtual void MouseUp()
        {
            Debug.Assert(Visible);

            LeftButtonPressed = false;
        }

        internal virtual void KeyPress(KeyPressEventArgs e)
        {
            foreach (VisualControl vc in Controls)
                vc.KeyPress(e);
        }

        internal virtual void KeyUp(KeyEventArgs e)
        {
            foreach (VisualControl vc in Controls)
                vc.KeyUp(e);
        }

        internal virtual void ArrangeControls()
        {
            foreach (VisualControl vc in Controls)
            {
                /*if (vc.Visible && ((vc.Width == 0) || (vc.Height == 0)))
                {
                    vc.ShowBorder = !vc.ShowBorder;
                    vc.width = 1000;
                    vc.height = 1000;
                }*/
                if (vc.Visible)
                {
                    Debug.Assert(vc.Width > 0);
                    Debug.Assert(vc.Height > 0);
                }

                ArrangeControl(vc);
                vc.IsError = false;
            }

            // Проверяем, что контролы не наложены друг на друга и не выходят за пределы родителя
            foreach (VisualControl vc in Controls)
            {
                // Страницы главного экрана и страницы TabControl наложены друг на друга, но по умолчанию скрыты.
                // Поэтому невидимые контролы пропускаем
                if (vc.Visible)
                {
                    foreach (VisualControl vc2 in Controls)
                    {
                        if (vc2.Visible && (vc != vc2))
                        {
                            /*if (vc.Rectangle.Contains(vc2.Rectangle))
                            {
                                vc.IsError = true;
                                vc.ShowBorder = true;
                                vc2.IsError = true;
                                vc2.ShowBorder = true;
                            }*/
                            Debug.Assert(!vc.Rectangle.Contains(vc2.Rectangle), vc.Rectangle.ToString() + " и " + vc2.Rectangle.ToString());
                        }
                    }

                    // Проверяем, что дочерний контрол не выходит за пределы этого контрола
                    //if (Visible && !Rectangle.Contains(vc.Rectangle))
                    //{
                    //    vc.IsError = true;
                    //    vc.ShowBorder = true;
                    //}
                    if (Visible)
                    {
                        if (!Rectangle.Contains(vc.Rectangle))
                            vc.ShowBorder = true;
                        //Debug.Assert(Rectangle.Contains(vc.Rectangle), Rectangle.ToString() + " и " + vc.Rectangle.ToString());
                    }
                }
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
                if (disposing)
                {
                    bmpBorder?.Dispose();
                    bmpBorderSelect?.Dispose();

                    if (Parent != null)
                    {
                        Parent.RemoveControl(this);
                        Parent = null;
                    }
                }

                bmpBorder = null;
                bmpBorderSelect = null;

                _disposed = true;
            }
        }
    }
}
