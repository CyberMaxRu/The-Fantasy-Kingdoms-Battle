using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс панели информации
    internal abstract class PanelBaseInfo : VisualControl
    {
        protected enum Page { Products, Warehouse, Inhabitants, Statistics, Inventory, Abilities, Description };

        private readonly VCLabel lblName;
        protected readonly VCImage128 imgIcon;
        protected readonly VCSeparator separator;
        protected VCTabControl pageControl;

        public PanelBaseInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            Visible = false;
            ShowBorder = true;

            lblName = new VCLabel(this, FormMain.Config.GridSize, FormMain.Config.GridSize - 3, Program.formMain.fontMedCaptionC, FormMain.Config.BattlefieldPlayerName, FormMain.Config.GridSize * 3, "");
            lblName.StringFormat.Alignment = StringAlignment.Center;
            lblName.Color = Color.MediumAquamarine;
            //lblName.ShowBorder = true;
            lblName.TruncLongText = true;
            lblName.LeftMargin = 2;
            lblName.RightMargin = 2;

            imgIcon = new VCImage128(this, FormMain.Config.GridSize, lblName.NextTop());
            imgIcon.ShowHint += ImgIcon_ShowHint;

            separator = new VCSeparator(this, FormMain.Config.GridSize, TopForControls());

            pageControl = new VCTabControl(this, FormMain.Config.GridSize, separator.NextTop())
            {
                //Parent = this,
                Width = Width - FormMain.Config.GridSize * 2,
                Height = Height - separator.NextTop() - FormMain.Config.GridSize
            };
        }

        private void ImgIcon_ShowHint(object sender, EventArgs e)
        {
            Debug.Assert(Entity != null);
            Entity.PrepareHint();
        }

        internal override void ArrangeControls()
        {
            lblName.Width = Width - FormMain.Config.GridSize * 2;
            separator.Width = lblName.Width;

            base.ArrangeControls();

            pageControl.Width = Width - (pageControl.ShiftX * 2);
        }

        // Используемые потомками методы
        protected int TopForControls() => imgIcon.NextTop();
        protected int LeftForControls() => imgIcon.ShiftX;
        protected int TopForIcon() => imgIcon.ShiftY;
        protected int LeftAfterIcon() => imgIcon.NextLeft();

        //protected Point LeftTopPage() => pointPage;

        // Переопределяемые потомками методы
        protected abstract int GetImageIndex();
        protected abstract bool ImageIsEnabled();
        protected abstract string GetCaption();

        // Общие для всех панелей методы

        internal override void Draw(Graphics g)
        {
            lblName.Text = GetCaption();
            imgIcon.ImageIndex = GetImageIndex();
            imgIcon.ImageIsEnabled = ImageIsEnabled();

            base.Draw(g);

            // Рисуем бордюр
            g.DrawImageUnscaled(Program.formMain.bmpBorderBig, imgIcon.Left - 2, imgIcon.Top - 2);
        }

        protected void ShowChapter<T, T1>(VCLabel toplabel, VCLabel bottomlabel, List<T> listProperty, List<T1> listControls) where T : CreaturePropertyMain where T1 : VCCreaturePropertyMain
        {
            int numberProperty = 0;
            int nextLeft = 0;
            int nextTop = toplabel.NextTop() - 4;

            for (int i = 0; i < listProperty.Count; i++)
            {
                listControls[i].ClearSlaveControls();

                if (listProperty[i] != null)
                {
                    VCCreaturePropertyMain idv = listControls[numberProperty];
                    idv.SetProperty(listProperty[i]);
                    idv.ShiftX = nextLeft;
                    idv.ShiftY = nextTop;

                    if (numberProperty % 4 == 3)
                    {
                        nextLeft = 0;
                        nextTop = idv.NextTop() - 4;
                    }
                    else
                    {
                        nextLeft = idv.NextLeft() - 4;
                    }

                    if (bottomlabel != null)
                    {
                        if (numberProperty % 4 == 0)
                        {
                            bottomlabel.SetAsSlaveControl(idv, FormMain.Config.GridSize, true);
                        }
                    }

                    toplabel.ArrangeControl(idv);

                    numberProperty++;
                }
            }

            if ((numberProperty == 0) && (bottomlabel != null))
            {
                bottomlabel.SetAsSlaveControl(toplabel, FormMain.Config.GridSize, true);
                toplabel.ArrangeControl(toplabel);
            }

            for (; numberProperty < listProperty.Count; numberProperty++)
            {
                listControls[numberProperty].SetProperty(null);
                listControls[numberProperty].ClearSlaveControls();
            }
        }
    }
}
