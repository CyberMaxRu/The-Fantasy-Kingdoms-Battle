using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class LayerEditorConquest : LayerCustom
    {
        private Bitmap bmpBackground;

        private readonly VCPageControl pageControl;
        private readonly VCPageButton pageMap;

        private VCMap mapArdania;
        private DescriptorMap descriptorMap;

        public LayerEditorConquest() : base()
        {
            bmpBackground = GuiUtils.MakeBackground(new Size(Width, Height));

            // Страницы редактора
            pageControl = new VCPageControl(this, 8, 8);
            //pageControl.PageChanged += PageControl_PageChanged;
            pageMap = pageControl.AddPage(Config.Gui48_Map, "Карта Ардании", "Карта Ардании", null);
            pageMap.Hint = "Итоги хода";

            pageControl.Width = 800;
            pageControl.Height = 600;
            pageControl.ArrangeControls();

            mapArdania = new VCMap(pageMap.Page, 0, 0, "Ardania150.png");
            //mapArdania.Click += MapArdania_Click;
            mapArdania.Width = pageMap.Page.Width;
            mapArdania.Height = pageMap.Page.Height;

            descriptorMap = new DescriptorMap(mapArdania.Bitmap.Width, mapArdania.Bitmap.Height, mapArdania);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, 0, 0);
        }
    }
}
