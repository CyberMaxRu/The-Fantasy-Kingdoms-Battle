using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс иконки страницы
    internal sealed class VCTabButton : VCImage
    {
        private Pen penBorder = new Pen(FormMain.Config.CommonBorder);
        private int shiftX;

        public VCTabButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            HighlightUnderMouse = true;
        }

        internal string NameTab { get; set; }
        internal int IndexPage { get; set; }
        internal VisualControl ContextPage { get; set; }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(NameTab, "", "");
            return true;
        }

        internal override void DoClick()
        {
            base.DoClick();

            (Parent as VCTabControl).ActivatePage(IndexPage);
            Program.formMain.ShowFrame();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            shiftX = IndexPage > 0 ? 4 : 0;

            if ((Parent as VCTabControl).ActivePage == IndexPage)
            {
                g.DrawLine(penBorder, Left, Top, Left + Width, Top);// Верх
                g.DrawLine(penBorder, Left, Top, Left, Top + Height - 1);// Левый край
                g.DrawLine(penBorder, Left + Width, Top, Left + Width, Top + Height - 1);// Правый край
                if (shiftX > 0)
                    g.DrawLine(penBorder, Left - shiftX, Top + Height - 1, Left, Top + Height - 1);
            }
            else
            {
                g.DrawLine(penBorder, Left - shiftX, Top + Height - 1, Left + Width, Top + Height - 1);
            }
        }
    }
}
