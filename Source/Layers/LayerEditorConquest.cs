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
        private Bitmap bmpMap;
        private VCBitmap bmpMinimap;

        private readonly VCPageControl pageControl;
        private readonly VCPageButton pageMap;

        private VCMap mapArdania;
        private DescriptorMap descriptorMap;

        VCButton btnLoadPicture;
        VCButton btnSetBorder;

        public LayerEditorConquest() : base()
        {
            bmpBackground = GuiUtils.MakeBackground(new Size(Width, Height));

            // Страницы редактора
            pageControl = new VCPageControl(this, 8, 8);
            //pageControl.PageChanged += PageControl_PageChanged;
            pageMap = pageControl.AddPage(Config.Gui48_Map, "Карта Ардании", "Карта Ардании", null);
            pageMap.Hint = "Карта Ардании";

            mapArdania = new VCMap(pageMap.Page, 0, 0);
            mapArdania.Width = 800;
            mapArdania.Height = 600;
            mapArdania.Click += MapArdania_Click;

            pageControl.ApplyMaxSize();
            pageControl.ArrangeControls();
            pageControl.ActivatePage(pageMap);

            bmpMinimap = new VCBitmap(this, 0, 0, Utils.LoadBitmap("Map.png"));
            bmpMinimap.ShiftY = Height - bmpMinimap.Height;

            pageControl.ShiftX = bmpMinimap.NextLeft();

            btnLoadPicture = new VCButton(this, Width - 248, FormMain.Config.GridSize, "Загрузить картинку");
            btnLoadPicture.Width = 240;
            btnLoadPicture.Click += BtnLoadPicture_Click;

            btnSetBorder = new VCButton(this, Width - 248, btnLoadPicture.NextTop(), "Указать границу");
            btnSetBorder.Width = 240;
            btnSetBorder.Click += BtnSetBorder_Click;

            ArrangeControls();
        }

        private void BtnSetBorder_Click(object sender, EventArgs e)
        {
            btnSetBorder.ManualSelected = !btnSetBorder.ManualSelected;
        }

        private void MapArdania_Click(object sender, EventArgs e)
        {
            if (btnSetBorder.ManualSelected)
            {
                Point p = mapArdania.MousePosToCoord(Program.formMain.MousePosToControl(mapArdania));
                descriptorMap.SearchBorder(p);
            }
        }

        private void BtnLoadPicture_Click(object sender, EventArgs e)
        {
            bmpMap = Utils.LoadBitmap(@"Conq\Ardania150_cut.png");
            mapArdania.Bitmap = bmpMap;

            descriptorMap = new DescriptorMap(mapArdania.Bitmap);
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, 0, 0);
        }
    }
}
