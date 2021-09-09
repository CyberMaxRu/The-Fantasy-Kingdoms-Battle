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

        private PlayerCellMenu research;

        public VCMenuCell(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY, -1)
        {
        }

        internal bool Used { get; set; }
        internal PlayerCellMenu Research
        {
            get { return research; }
            set
            {
                research = value;
                Visible = research != null;
                if (Visible)
                    Text = research.Cost().ToString();
            }
        }

        internal override void DoClick()
        {
            base.DoClick();

            Debug.Assert(research != null);
            if (research.CheckRequirements())
            {
                Program.formMain.PlayPushButton();
                research.DoResearch();
            }
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
            if ((research != null) && (research.Research.TypeEntity != null))
            {
                Text = research.Cost().ToString();
                ImageIndex = research.Research.TypeEntity.ImageIndex;
                ImageIsEnabled = research.CheckRequirements();

                // Накладываем фильтр
                //if (!research.CheckRequirements())
                //    ImageFilter = ImageFilter.Disabled;
            }
            else if ((research != null) && (research.Research.TypeConstruction != null))
            {
                if (research.ConstructionForBuild != null)
                {
                    Construction pc = research.ObjectOfMap.Player.GetPlayerConstruction(research.Research.TypeConstruction);
                    Debug.Assert(!(pc is null));
                    Text = pc.CostBuyOrUpgrade().ToString();
                    ImageIndex = pc.TypeConstruction.ImageIndex;
                    ImageIsEnabled = research.CheckRequirements();
                }
                else
                {
                    Text = research.Research.TypeConstruction.Levels[1].Cost.ToString();
                    ImageIndex = research.Research.TypeConstruction.ImageIndex;
                    ImageIsEnabled = research.Player.CanBuildTypeConstruction(research.Research.TypeConstruction);
                }
                // Накладываем фильтр
                //if (!research.CheckRequirements())
                //    ImageFilter = ImageFilter.Disabled;
            }
            else
            {
                ImageIndex = -1;
            }

            base.Draw(g);

            if (Visible && (ImageIndex != -1))
            {
                if (ImageIsEnabled)
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
