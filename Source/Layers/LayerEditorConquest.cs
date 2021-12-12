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
        private VCBitmap bmpMinimap;

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

            mapArdania = new VCMap(pageMap.Page, 0, 0, "Ardania150_cut.png");
            mapArdania.Width = 800;
            mapArdania.Height = 600;
            //mapArdania.Click += MapArdania_Click;

            pageControl.ApplyMaxSize();
            pageControl.ArrangeControls();
            pageControl.ActivatePage(pageMap);

            descriptorMap = new DescriptorMap(mapArdania.Bitmap.Width, mapArdania.Bitmap.Height, mapArdania);

            bmpMinimap = new VCBitmap(this, 0, 0, Utils.LoadBitmap("Map.png"));
            bmpMinimap.ShiftY = Height - bmpMinimap.Height;

            pageControl.ShiftX = bmpMinimap.NextLeft();

            ArrangeControls();
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, 0, 0);
        }
    }
}
