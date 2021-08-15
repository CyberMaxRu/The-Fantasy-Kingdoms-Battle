using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс иконки страницы
    internal sealed class VCTabButton : VCImage
    {
        private Pen penBorder = new Pen(FormMain.Config.CommonBorder);

        public VCTabButton(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY, bitmapList, imageIndex)
        {
            HighlightUnderMouse = true;
        }

        internal string NameTab { get; set; }
        internal int IndexPage { get; set; }
        internal VisualControl ContextPage { get; set; }

        internal override bool PrepareHint()
        {
            Program.formMain.formHint.AddSimpleHint(NameTab);
            return true;
        }

        internal override void DoClick()
        {
            base.DoClick();

            Program.formMain.PlayPushButton();
            (Parent as VCTabControl).ActivatePage(IndexPage);
            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void Draw(Graphics g)
        {
            ImageFilter = (Parent as VCTabControl).ActivePage == IndexPage ? ImageFilter.Press : ImageFilter.None;

            base.Draw(g);
        }
    }
}
