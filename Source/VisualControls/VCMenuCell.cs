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
        private ConstructionCellMenu research;

        public VCMenuCell(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, -1)
        {
            lblBanner = new VCLabel(this, 0, 0, Program.formMain.fontBigCaptionC, Color.White, Height, "");
            lblBanner.StringFormat.LineAlignment = StringAlignment.Center;
            lblBanner.Width = Width;
            lblBanner.Visible = false;
        }

        internal bool Used { get; set; }
        internal ConstructionCellMenu Research
        {
            get { return research; }
            set
            {
                research = value;
                Visible = research != null;
                if (Visible)
                    Text = research.GetText();
            }
        }

        internal override void DoClick()
        {
            base.DoClick();

            research.Click();
        }

        internal override bool PrepareHint()
        {
            // После клика на ячейке меню, надо перепоказать подсказку
            // Если на ячейке исследования больше нет, то сообщаем, что подсказки нет
            if (research != null)
            {
                research.PrepareHint();
                return true;
            }

            return false;
        }

        internal override void Draw(Graphics g)
        {
            if (research != null)
            {
                ImageIndex = research.GetImageIndex();

                if (research.PosInQueue > 0)
                {
                    //Level = "";
                    //Text = "";
                    Level = (research.PosInQueue + 1).ToString();

                    //ImageIsEnabled = false;
                    //lblBanner.Visible = true;
                    if (research.PosInQueue == 1)
                    {
                        Text = research.DaysLeft.ToString() + " д.";

                        lblBanner.Text = research.DaysLeft.ToString();
                        lblBanner.Color = Color.LimeGreen;
                    }
                    else
                    {
                        Text = "ожид.";
                        lblBanner.Text = research.PosInQueue.ToString();
                        lblBanner.Color = Color.DarkGoldenrod;
                    }
                }
                else
                {
                    ImageIsEnabled = true;
                    lblBanner.Visible = false;
                    Text = research.GetText();
                    Level = research.GetLevel();
                }
                //ImageIsEnabled = research.CheckRequirements();

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

            base.Draw(g);

            if (Visible && (ImageIndex != -1))
            {
                if (research.CheckRequirements() && (research.DaysProcessed == 0))
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
