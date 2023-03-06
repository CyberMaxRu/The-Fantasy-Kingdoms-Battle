using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum MenuCellFilter { None = -1, Active = 0, Select = 1, Press = 2, Disabled = 3 };

    // Визуальный контрол - ячейка меню
    internal sealed class VCMenuCell : VCImage48
    {
        private VCLabel lblBanner;
        private ActionForEntity research;

        public VCMenuCell(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, -1)
        {
            PlaySoundOnClick = true;

            lblBanner = new VCLabel(this, 0, 0, Program.formMain.fontBigCaptionC, Color.White, Height, "");
            lblBanner.StringFormat.LineAlignment = StringAlignment.Center;
            lblBanner.Width = Width;
            lblBanner.Visible = false;
        }

        internal bool Used { get; set; }
        internal ActionForEntity Research
        {
            get { return research; }
            set
            {
                research = value;
                Visible = research != null;
                if (Visible)
                    LowText = research.GetText();
            }
        }

        internal override void DoClick()
        {
            base.DoClick();

            research?.Click();
        }

        internal override bool PrepareHint()
        {
            // После клика на ячейке меню, надо перепоказать подсказку
            // Если на ячейке исследования больше нет, то сообщаем, что подсказки нет
            if (research != null)
            {
                research.PrepareHint(PanelHint);
                return true;
            }

            return false;
        }

        internal override void Draw(Graphics g)
        {
            if (!ManualDraw)
            {
                if (research != null)
                {
                    ImageIndex = research.GetImageIndex();
                    Color = research.GetColorText();
                    RestTimeExecuting = research.GetExtInfo();
                    Level = research.GetLevel();
                    Quantity = research.GetQuantity();
                    LowText = research.GetText();

                    // Накладываем фильтр
                    //if (!research.CheckRequirements())
                    //    ImageFilter = ImageFilter.Disabled;
                }
                /*            else if ((research != null) && (research.Research.Construction != null))
                            {
                                if (research.ConstructionForBuild != null)
                                {
                                    Construction pc = research.ObjectOfMap.Player.GetPlayerConstruction(research.Research.Construction);
                                    Debug.Assert(!(pc is null));
                                    Text = pc.CostBuyOrUpgrade().ToString();
                                    ImageIndex = pc.TypeConstruction.ImageIndex;
                                    ImageIsEnabled = research.CheckRequirements();
                                }
                                else
                                {
                                    Text = research.Research.Construction.Levels[1].Cost.ToString();
                                    ImageIndex = research.Research.Construction.ImageIndex;
                                    ImageIsEnabled = research.Player.CanBuildTypeConstruction(research.Research.Construction);
                                }
                                // Накладываем фильтр
                                //if (!research.CheckRequirements())
                                //    ImageFilter = ImageFilter.Disabled;
                            }*/
                else
                {
                    ImageIndex = -1;
                }
            }

            base.Draw(g);

            if (Visible && (ImageIndex != -1))
            {
                if (ManualDraw || research.GetImageIsEnabled())
                {
                    if (MouseClicked && MouseOver)
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage((int)MenuCellFilter.Press, true, false), Left, Top);
                    else if (MouseOver)
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage((int)MenuCellFilter.Select, true, false), Left, Top);
                    else
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage((int)MenuCellFilter.Active, true, false), Left, Top);
                }
                else
                    g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage((int)MenuCellFilter.Disabled, true, false), Left, Top);
            }
        }
    }
}
