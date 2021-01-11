using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Базовый класс для всех визуальных контролов
    internal abstract class VisualControl
    {
        private int left;
        private int top;
        private int width;
        private int height;

        public VisualControl()
        {

        }

        internal int Left { get { return left; } set { left = value; ArrangeControlsAndContainers(); } }
        internal int Top { get { return top; } set { top = value; ArrangeControlsAndContainers(); } }
        internal int Width { get { return width; } set { width = value; } }
        internal int Height { get { return height; } set { height = value; } }
        internal abstract void Draw(Graphics g);// Метод для рисования
        internal event EventHandler Click;
        internal event EventHandler ShowHint;

        internal void DoClick()
        {
            Click(this, new EventArgs());
        }

        internal void DoShowHint()
        {
            ShowHint(this, new EventArgs());
        }

        protected void ArrangeControlsAndContainers()
        {

        }

        internal int NextTop()
        {
            return Top + Height + FormMain.Config.GridSize;
        }
    }
}
