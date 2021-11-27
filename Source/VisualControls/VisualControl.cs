using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс и класс-контейнер для всех визуальных контролов
    internal class VisualControl : IDisposable
    {
        private int left;// Координата Left на главном окне (абсолютная)
        private int top;// Координата Top на главном окне (абсолютная)
        private int width;// Ширина контрола
        private int height;// Высота контрола
        private bool _visible;

        private Entity entity;
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
            set { if (_visible != value) { _visible = value; if (_visible) Program.formMain.ControlShowed(this); else Program.formMain.ControlHided(this); } }
        }// Видимость контрола
        internal bool ManualDraw { get; set; }// Ручное рисование контрола
        internal bool ShowBorder { get; set; }// Надо ли показывать бордюр
        internal Entity Entity { get => entity; set { SetEntity(value); } }// Объект, ассоциированный с контролом

        internal bool IsError { get; set; }
        internal bool ShowHintParent { get; set; }// Показывать подсказку родителя
        internal string Hint { get; set; }// Подсказка к контролу
        internal bool ClickOnParent { get; set; }// Вызывать клик у родителя
        internal bool ManualSelected { get; set; } = false;
        internal bool PlaySoundOnEnter { get; set; } = false;// Проигрывать звук при входе в контрол
        internal bool PlaySoundOnClick { get; set; } = false;// Проигрывать звук при клике

        internal Bitmap BackgroundImage { get; set; }// Фоновое изображение

        // Защищенные свойства
        internal bool MouseOver { get; private set; }// Курсор мыши находится над контролом
        protected bool MouseClicked { get; private set; }// ЛКМ нажата

        // Список контролов, расположенных на нём
        internal List<VisualControl> Controls = new List<VisualControl>();
        internal List<VisualControl> SlaveControls { get; private set; }
        internal int ShiftAtMasterControl { get; set; }
        internal bool ShiftFromMasterControlToDown { get; set; } = true;


        internal event EventHandler Click;
        internal event EventHandler RightClick;
        internal event EventHandler ShowHint;

        protected virtual void SetEntity(Entity po)
        {
            //Debug.Assert(po != null);

            entity = po;
        }

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
            if (BackgroundImage != null)
            {
                Debug.Assert(width == BackgroundImage.Width);
                Debug.Assert(height == BackgroundImage.Height);

                g.DrawImageUnscaled(BackgroundImage, Left, Top);
            }

            if (Selected())
            {
                Program.formMain.bbSelect.DrawBorder(g, Left - 8, Top - 8, Width + 16, Height + 16);
            }
        }

        // Метод для рисования. Передается подготовленный Graphics
        internal virtual void Draw(Graphics g)
        {
            if (IsError)
                g.FillRectangle(FormMain.Config.brushControl, Rectangle);
         }

        internal virtual void PaintBorder(Graphics g)
        {
            Program.formMain.bbObject.DrawBorder(g, Left - 2, Top, Width + 4, Height + 3);
        }

        internal virtual void PaintForeground(Graphics g)
        {
            // Рисуем бордюр
            if (ShowBorder && Visible)
                PaintBorder(g);
        }

        internal virtual void DoClick()
        {
            Debug.Assert(Visible);

            if (AllowClick())
                if (!ClickOnParent)
                {
                    if (PlaySoundOnClick)
                        Program.formMain.PlayPushButton();
                    Click?.Invoke(this, new EventArgs());
                }
                else
                {
                    if (PlaySoundOnClick)
                        Program.formMain.PlayPushButton();
                    Parent.DoClick();
                }
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

            if (Hint != null)
            {
                Program.formMain.formHint.Clear();
                Program.formMain.formHint.AddSimpleHint(Hint);
                Program.formMain.formHint.DrawHint(this);
            }
            else if (ShowHintParent && Parent.ShowHint != null)
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

            if (PlaySoundOnEnter)
            {
                Program.formMain.PlaySelectButton();
            }
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
            Program.formMain.SetNeedRedrawFrame();
        }

        internal virtual void MouseUp()
        {
            Debug.Assert(Visible);
            Debug.Assert(MouseClicked);

            MouseClicked = false;
            Program.formMain.SetNeedRedrawFrame();
        }

        internal virtual void RightButtonClick()
        {
            if (AllowClick())
                if (!ClickOnParent)
                {
                    RightClick?.Invoke(this, new EventArgs());
                }
                else
                {
                    Parent.RightButtonClick();
                }
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
                            //    vc2.IsError = true;
                            //Debug.Assert(!vc.Rectangle.Contains(vc2.Rectangle), vc.Rectangle.ToString() + " и " + vc2.Rectangle.ToString());
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
                        //Debug.Assert(Rectangle.Contains(vc.Rectangle), Rectangle.ToString() + " и " + vc.Rectangle.ToString());
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
                Assert(vc.Parent == this);
                Debug.Assert(Controls.IndexOf(vc) != -1);
            }

            // Надобности вроде особой неты
            //Debug.Assert(Height > 0);
            //Debug.Assert(Width > 0);

            vc.Left = Left + vc.ShiftX;
            vc.Top = Top + vc.ShiftY;

            //Debug.Assert(Left >= 0);
            //Debug.Assert(Top >= 0);

            if (vc.SlaveControls != null)
            {
                foreach (VisualControl svc in vc.SlaveControls)
                {
                    Assert(svc != this);
                    Assert(svc != vc);
                    Assert(svc.Parent == vc.Parent);

                    if (svc.ShiftFromMasterControlToDown)
                    {
                        svc.ShiftX = vc.ShiftX;
                        svc.ShiftY = vc.ShiftY + vc.Height + svc.ShiftAtMasterControl;
                    }
                    else
                    {
                        svc.ShiftX = vc.ShiftX + vc.Width + svc.ShiftAtMasterControl;
                        svc.ShiftY = vc.ShiftY;
                    }

                    ArrangeControl(svc);
                }
            }

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
            vc.Parent = null;
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
                    if (Parent != null)
                    {
                        Parent.RemoveControl(this);
                        Parent = null;
                    }
                }

                _disposed = true;
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

        internal void SetAsSlaveControl(VisualControl vcMaster, int shift, bool toDown)
        {
            if (vcMaster.SlaveControls is null)
            {
                vcMaster.SlaveControls = new List<VisualControl>();
            }
            else
            {
                Assert(vcMaster.SlaveControls.IndexOf(this) == -1);
            }
                
            vcMaster.SlaveControls.Add(this);
            ShiftAtMasterControl = shift;
            ShiftFromMasterControlToDown = toDown;
        }
    }
}
