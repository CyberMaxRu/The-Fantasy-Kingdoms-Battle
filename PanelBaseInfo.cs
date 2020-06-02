using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Базовый класс панели информации
    internal abstract class PanelBaseInfo : BasePanel
    {
        protected enum Page { Products, Warehouse, Inhabitants, Statistics, Inventory, Abilities };

        private readonly Label lblName;
        private readonly PictureBox pbIcon;
        private List<PictureBox> btnPages = new List<PictureBox>();
        private int leftForNextPage;
        private Label lblCaptionPage;
        private Point pointPage;
        private Page activePage;

        public PanelBaseInfo(int width, int height) : base()
        {
            Width = width;
            Height = height;
            DoubleBuffered = true;

            lblName = new Label()
            {
                Parent = this,
                Width = Width - Config.GRID_SIZE * 2,
                Height = Config.GRID_SIZE * 3,
                MaximumSize = new Size(Width - Config.GRID_SIZE * 2, Config.GRID_SIZE * 3),
                Location = new Point(Config.GRID_SIZE, Config.GRID_SIZE),
                Font = new Font("Microsof Sans Serif", 13),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            pbIcon = new PictureBox()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = GuiUtils.NextTop(lblName),
                Size = GetImageList().ImageSize,
                BackColor = Color.Transparent
            };

            lblCaptionPage = new Label()
            {
                Parent = this,
                Left = 0,
                Width = ClientSize.Width,
                Top = TopForControls() + Program.formMain.ilGui.ImageSize.Height + Config.GRID_SIZE,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft Sans Serif", 12),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };

            pointPage = new Point(Config.GRID_SIZE, GuiUtils.NextTop(lblCaptionPage));
            leftForNextPage = Config.GRID_SIZE;
        }

        private void PbPage_Click(object sender, EventArgs e)
        {
            ActivatePage((Page)(sender as PictureBox).Tag);
        }

        // Используемые потомками методы
        protected int TopForControls() => GuiUtils.NextTop(pbIcon);
        protected int LeftForControls() => pbIcon.Left;
        protected int TopForIcon() => pbIcon.Top;
        protected int LeftAfterIcon() => GuiUtils.NextLeft(pbIcon);

        protected void AddPage(Page page)
        {
            int imageIndex;
            switch (page)
            {
                case Page.Products:
                    imageIndex = FormMain.GUI_PRODUCTS;
                    break;
                case Page.Warehouse:
                    imageIndex = FormMain.GUI_INVENTORY;
                    break;
                case Page.Inhabitants:
                    imageIndex = FormMain.GUI_INHABITANTS;
                    break;
                case Page.Statistics:
                    imageIndex = FormMain.GUI_PARAMETERS;
                    break;
                case Page.Inventory:
                    imageIndex = FormMain.GUI_INVENTORY;
                    break;
                case Page.Abilities:
                    imageIndex = FormMain.GUI_ABILITY;
                    break;
                default:
                    throw new Exception("Неизвестный тип страницы.");
            }

            PictureBox pbPage = new PictureBox()
            {
                Parent = this,
                Left = leftForNextPage,
                Top = TopForControls(),
                Size = GuiUtils.SizeButtonWithImage(Program.formMain.ilGui),
                BackgroundImage = Program.formMain.bmpForBackground,
                Image = Program.formMain.ilGui.Images[imageIndex],
                Tag = page
            };
            pbPage.Click += PbPage_Click;

            btnPages.Add(pbPage);
            leftForNextPage = GuiUtils.NextLeft(pbPage);

            if (btnPages.Count == 1)
                ActivatePage(page);
        }

        protected virtual void ActivatePage(Page page)
        {
            activePage = page;

            switch (page)
            {
                case Page.Products:
                    lblCaptionPage.Text = "Товары";
                    break;
                case Page.Warehouse:
                    lblCaptionPage.Text = "Склад";
                    break;
                case Page.Inhabitants:
                    lblCaptionPage.Text = "Жители";
                    break;
                case Page.Statistics:
                    lblCaptionPage.Text = "Статистика";
                    break;
                case Page.Inventory:
                    lblCaptionPage.Text = "Инвентарь";
                    break;
                case Page.Abilities:
                    lblCaptionPage.Text = "Способности";
                    break;
                default:
                    throw new Exception("Неизвестная страница");
            }
        }

        protected void SetPageVisible (Page page, bool visible)
        {
            foreach (PictureBox pb in btnPages)
                if ((Page)pb.Tag == page)
                {
                    pb.Visible = visible;

                    if (!visible)
                    {
                        Debug.Assert(btnPages[0] != pb);

                        ActivatePage((Page)btnPages[0].Tag);
                    }

                    break;
                }
        }

        protected Point LeftTopPage() => pointPage;

        // Переопределяемые потомками методы
        protected abstract ImageList GetImageList();
        protected abstract int GetImageIndex();
        protected abstract string GetCaption();

        // Общие для всех панелей методы
        internal virtual void ShowData()
        {
            lblName.Text = GetCaption();
            pbIcon.Image = GetImageList().Images[GetImageIndex()];
        }
    }
}
