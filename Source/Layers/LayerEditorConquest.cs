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
        private VCBitmap bmpBorderMinimap;

        private readonly VCPageControl pageControl;
        private readonly VCPageButton pageMap;

        private VCMap mapArdania;
        private DescriptorMap descriptorMap;

        VCButton btnLoadPicture;
        VCButton btnSetBorder;
        VCButton btnFindRegions;
        VCButton btnSaveMap;
        VCButton btnLoadMap;

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

            bmpBorderMinimap = new VCBitmap(this, 0, 0, Utils.LoadBitmap("Map.png"));
            bmpBorderMinimap.ShiftY = Height - bmpBorderMinimap.Height;

            pageControl.ShiftX = bmpBorderMinimap.NextLeft();

            btnLoadPicture = new VCButton(this, Width - 248, FormMain.Config.GridSize, "Загрузить картинку");
            btnLoadPicture.Width = 240;
            btnLoadPicture.Click += BtnLoadPicture_Click;

            btnSetBorder = new VCButton(this, Width - 248, btnLoadPicture.NextTop(), "Указать границу");
            btnSetBorder.Width = 240;
            btnSetBorder.Click += BtnSetBorder_Click;

            btnFindRegions = new VCButton(this, Width - 248, btnSetBorder.NextTop(), "Найти регионы");
            btnFindRegions.Width = 240;
            btnFindRegions.Click += BtnFindRegions_Click;

            btnSaveMap = new VCButton(this, Width - 248, btnFindRegions.NextTop(), "Сохранить карту");
            btnSaveMap.Width = 240;
            btnSaveMap.Click += BtnSaveMap_Click;

            btnLoadMap = new VCButton(this, Width - 248, btnSaveMap.NextTop(), "Загрузить карту");
            btnLoadMap.Width = 240;
            btnLoadMap.Click += BtnLoadMap_Click;

            ArrangeControls();
        }

        private void BtnLoadMap_Click(object sender, EventArgs e)
        {
            descriptorMap = new DescriptorMap(Program.FolderResources + @"Icons\conq\Conquest");
            mapArdania.Bitmap = descriptorMap.Bitmap;
        }

        private void BtnSaveMap_Click(object sender, EventArgs e)
        {
            descriptorMap.SaveToFile(Program.FolderResources + @"Icons\conq\Conquest");

        }

        private void BtnFindRegions_Click(object sender, EventArgs e)
        {
            descriptorMap.DetermineRegions();
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
            descriptorMap = new DescriptorMap(Utils.LoadBitmap(@"Conq\Ardania150_cut.png"));
            mapArdania.Bitmap = descriptorMap.Bitmap;
        }

        internal override void DrawBackground(Graphics g)
        {
            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, 0, 0);

            if (descriptorMap?.MiniMap != null)
                g.DrawImageUnscaled(descriptorMap.MiniMap, 14, Height - descriptorMap.MiniMap.Height - 12);
        }
    }
}
