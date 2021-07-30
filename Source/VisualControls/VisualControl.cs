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
        private static Dictionary<Size, Bitmap> poolBorders = new Dictionary<Size, Bitmap>();

        private int left;// Координата Left на главном окне (абсолютная)
        private int top;// Координата Top на главном окне (абсолютная)
        private int width;// Ширина контрола
        private int height;// Высота контрола
        private bool _visible;

        private Bitmap bmpBorder;
        private Bitmap bmpBorderSelect;
        private Size sizeBorder;

        private bool _disposed = false;

        public VisualControl()
        {
            _visible = true;
        }

        public VisualControl(VisualControl parent, int shiftX, int shiftY)
        {
            _visible = true;

            ShiftX = shiftX;
            ShiftY = shiftY;

            SetParent(parent);
            //Debug.Assert(VisualLayer != null);
        }

        ~VisualControl() => Dispose(false);

        internal VisualControl Parent { get; private set; }
        internal VisualControl VisualLayer { get; private set; }
        internal int Left { get { return left; } private set { left = value; ValidateRectangle(); ArrangeControls(); } }
        internal int Top { get { return top; } private set { top = value; ValidateRectangle(); ArrangeControls(); } }
        internal int Width { get { return width; } set { width = value; ValidateRectangle(); } }
        internal int Height { get { return height; } set { height = value; ValidateRectangle(); } }
        internal int ShiftX { get; set; }// Смещение контрола относительно левого края на родителе
        internal int ShiftY { get; set; }// Смещение контрола относительно верхнего края на родителе
        internal Rectangle Rectangle { get; private set; }// Координаты и размер контрола
        // Когда контрол скрывается во время различных изменений, в том числе во время рендеринга, 
        // надо убрать его как активного
        internal bool Visible
        {
            get => _visible;
            set { _visible = value; if (!_visible) Program.formMain.ControlHided(this); }
        }// Видимость контрола
        internal bool ManualDraw { get; set; }// Ручное рисование контрола
        internal bool ShowBorder { get; set; }// Надо ли показывать бордюр
        internal PlayerObject PlayerObject { get; set; }// Объект, ассоциированный с контролом

        internal bool IsError { get; set; }
        internal bool ShowHintParent { get; set; }// Показывать подсказку родителя
        internal bool ClickOnParent { get; set; }// Вызывать клик у родителя
        internal bool ManualSelected { get; set; } = false;

        // Защищенные свойства
        internal bool MouseOver { get; private set; }// Курсор мыши находится над контролом
        protected bool MouseClicked { get; private set; }// ЛКМ нажата

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

        internal void Paint(Graphics g)
        {
            DrawBackground(g);
            Draw(g);
            PaintForeground(g);

            foreach (VisualControl vc in Controls)
            {
                if (vc.Visible)
                    vc.Paint(g);
            }
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
        }

        // Метод для рисования. Передается подготовленный Graphics
        internal virtual void Draw(Graphics g)
        {
            if (IsError)
                g.FillRectangle(FormMain.Config.brushControl, Rectangle);
         }

        internal virtual void PaintForeground(Graphics g)
        {
            // Рисуем бордюр
            if (ShowBorder && Visible)
            {
                if ((sizeBorder == null) || (sizeBorder.Width != Width + 4) || (sizeBorder.Height != Height + 3))
                    sizeBorder = new Size(Width + 4, Height + 3);

                g.DrawImageUnscaled(GetBorder(sizeBorder), Left - 2, Top);
            }
        }

        internal virtual void DoClick()
        {
            Debug.Assert(Visible);

            if (AllowClick())
                if (!ClickOnParent)
                    Click?.Invoke(this, new EventArgs());
                else
                    Parent.DoClick();
        }

        protected virtual bool Selected() => ManualSelected;
        protected virtual bool AllowClick() => true;

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
            Debug.Assert(Visible);

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
            Debug.Assert(!MouseOver);
            Debug.Assert(Visible);

            MouseOver = true;
            MouseClicked = leftButtonDown;
        }

        internal virtual void MouseLeave()
        {
            Debug.Assert(MouseOver);

            MouseOver = false;
            MouseClicked = false;
        }

        internal virtual void MouseDown()
        {
            Debug.Assert(Visible);
            Debug.Assert(!MouseClicked);

            MouseClicked = true;
        }

        internal virtual void MouseUp()
        {
            Debug.Assert(Visible);
            Debug.Assert(MouseClicked);

            MouseClicked = false;
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
                            //if (vc.Rectangle.Contains(vc2.Rectangle))
                            //    vc2.ShowBorder = true;
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
                        //if (!Rectangle.Contains(vc.Rectangle))
                        //    vc.IsError = true;
                        Debug.Assert(Rectangle.Contains(vc.Rectangle), Rectangle.ToString() + " и " + vc.Rectangle.ToString());
                    }
                }
            }
        }

        internal VisualControl GetControl(int x, int y)
        {
            Debug.Assert(Visible);

            if (Rectangle.Contains(x, y))
            {
                VisualControl ivc;

                foreach (VisualControl vc in Controls)
                {
                    if (vc.Visible)
                    {
                        ivc = vc.GetControl(x, y);
                        if (!(ivc is null))
                            return ivc;
                    }
                }

                return this;
            }
            
            return null;
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
            Debug.Assert(Controls.IndexOf(vc) == -1);

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

        private Bitmap GetBorder(Size size)
        {
            // Ищем бордюр такого размера в пуле
            if (poolBorders.ContainsKey(size))
                return poolBorders[size];
            else
            {
                Bitmap bmpBorder = Program.formMain.bbObject.DrawBorder(size.Width, size.Height);
                poolBorders.Add(size, bmpBorder);
                return bmpBorder;
            }
        }

        internal void SetParent(VisualControl parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent != this);

            parent.AddControl(this);
            VisualLayer = parent.VisualLayer;

            foreach (VisualControl vc in Controls)
                vc.VisualLayer = VisualLayer;
        }
    }
}
