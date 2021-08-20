using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ImageFilter { None = -1, Active = 0, Select = 1, Press = 2, Disabled = 3 };

    // Визуальный контрол - ячейка меню
    internal sealed class VCMenuCell : VCImage48
    {

        private PlayerCellMenu research;

        public VCMenuCell(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList) : base(parent, shiftX, shiftY, bitmapList, -1)
        {
        }

        internal ImageFilter ImageFilter { get; set; }// private field?
        internal bool Used { get; set; }
        internal PlayerCellMenu Research
        {
            get { return research; }
            set
            {
                research = value;
                Visible = research != null;
                if (Visible)
                    Cost = research.Cost().ToString();
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
            if (Visible && (BitmapList.Size == Program.formMain.ilMenuCellFilters.Size))
            {
                if (ImageIsEnabled)
                {
                    if (ShowAsPressed || (MouseClicked && MouseOver))
                        ImageFilter = ImageFilter.Press;
                    else if (MouseOver)
                        ImageFilter = ImageFilter.Select;
                    else
                        ImageFilter = ImageFilter.Active;
                }
                else
                    ImageFilter = ImageFilter.Disabled;
            }


            if ((research != null) && (research.Research.Entity != null))
            {
                Cost = research.Cost().ToString();
                ImageIndex = research.Research.Entity.ImageIndex;
                ImageIsEnabled = research.CheckRequirements();
                BitmapList = Program.formMain.imListObjects48;

                // Накладываем фильтр
                //if (!research.CheckRequirements())
                //    ImageFilter = ImageFilter.Disabled;
            }
            else if ((research != null) && (research.Research.TypeConstruction != null))
            {
                if (research.ConstructionForBuild != null)
                {
                    PlayerConstruction pc = research.ObjectOfMap.Player.GetPlayerConstruction(research.Research.TypeConstruction);
                    Debug.Assert(!(pc is null));
                    Cost = pc.CostBuyOrUpgrade().ToString();
                    ImageIndex = pc.TypeConstruction.ImageIndex;
                    ImageIsEnabled = research.CheckRequirements();
                    BitmapList = Program.formMain.imListObjects48;
                }
                else
                {
                    Cost = research.Research.TypeConstruction.Levels[1].Cost.ToString();
                    ImageIndex = research.Research.TypeConstruction.ImageIndex;
                    ImageIsEnabled = research.Player.CanBuildTypeConstruction(research.Research.TypeConstruction);
                    BitmapList = Program.formMain.imListObjects48;

                }
                // Накладываем фильтр
                //if (!research.CheckRequirements())
                //    ImageFilter = ImageFilter.Disabled;
            }
            else
            {
                ImageIndex = -1;
                ImageFilter = ImageFilter.None;
            }

            base.Draw(g);

            if (Visible && (ImageIndex != -1))
            {
                switch (ImageFilter)
                {
                    case ImageFilter.Active:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(0, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    case ImageFilter.Select:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(1, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    case ImageFilter.Press:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(2, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    case ImageFilter.Disabled:
                        g.DrawImageUnscaled(Program.formMain.ilMenuCellFilters.GetImage(3, true, false), Left + ShiftImageX, Top + ShiftImageY);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
