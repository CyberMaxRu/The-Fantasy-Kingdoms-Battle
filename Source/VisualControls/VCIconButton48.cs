﻿using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Визуальный контрол - кнопка c иконкой
    internal class VCIconButton48 : VCImage48
    {
        public VCIconButton48(VisualControl parent, int shiftX, int shiftY, int imageIndex) : base(parent, shiftX, shiftY, imageIndex)
        {
            //UseFilter = true;
            PlaySoundOnClick = true;
            HighlightUnderMouse = true;
            ShowBorder = true;
        }

        internal ActionForEntity MenuCell { get; set; }

        internal override void Draw(Graphics g)
        {
            //ImageFilter = ImageFilter.Active;
            //if (Visible)
            //    g.DrawImageUnscaled(Program.formMain.bmpBackgroundEntity, Left - 1, Top - 1);

            if (MenuCell != null)
            {
                ImageIndex = MenuCell.GetImageIndex();
                ImageIsEnabled = MenuCell.GetImageIsEnabled();
                LowText = MenuCell.GetText();
                RestTimeExecuting = MenuCell.GetExtInfo();
                StateRestTime = MenuCell.GetStateRestTime();
                Level = ShowBorder ? MenuCell.GetLevel() : "";// Если нет бордюра, значит это режим очереди исследования. Уровень и количество не показываем
                Quantity = MenuCell.GetQuantity();
                Color = MenuCell.GetColorText();
            }

            base.Draw(g);
        }

        internal override void DoShowHint()
        {
            if (Visible && (MenuCell != null))
            {
                MenuCell.PrepareHint(PanelHint);
            }
            else
                base.DoShowHint();
        }
    }
}
