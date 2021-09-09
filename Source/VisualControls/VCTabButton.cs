using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс иконки страницы
    internal sealed class VCTabButton : VCImage48
    {
        public VCTabButton(VisualControl parent, int shiftX, int shiftY, int imageIndex, VisualControl contextPage) : base(parent, shiftX, shiftY, imageIndex)
        {
            HighlightUnderMouse = true;

            ContextPage = contextPage;
        }

        internal int IndexPage { get; set; }
        internal VisualControl ContextPage { get; }

        internal override void DoClick()
        {
            base.DoClick();

            Program.formMain.PlayPushButton();
            (Parent as VCTabControl).ActivatePage(IndexPage);
            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            if ((Parent as VCTabControl).ActivePage == IndexPage)
                g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage((int)MenuCellFilter.Press, true, false), Left, Top);
        }
    }
}
