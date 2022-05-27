using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCButtonTargetLair : VCCell
    {
        public VCButtonTargetLair(VisualControl parent) : base(parent, 0, 0)
        {
            ManualDraw = true;
        }

        internal BigEntity Lair { get => Entity as BigEntity; }
        internal bool ShowFlag { get; set; } = true;

        internal override void DoClick()
        {
            base.DoClick();

            if ((Lair != null) && (Lair.ComponentObjectOfMap.TypeFlag != TypeFlag.Battle))
            {
                //Program.formMain.ActivatePageLairs(Lair.Location.Settings.Number);
                Program.formMain.layerGame.SelectPlayerObject(Lair);
            }
        }

        protected override bool AllowClick()
        {
            return base.AllowClick() && (Lair != null) && (Lair.ComponentObjectOfMap.TypeFlag != TypeFlag.Battle);
        }

        internal override bool PrepareHint()
        {
            if (Lair is null)
            { 
                PanelHint.AddSimpleHint(Lair != null ? Lair.GetName() : "Флаг не назначен");
            }
            else
            {
                Debug.Assert(!(Lair is null));
                if (Lair.ComponentObjectOfMap.TypeFlag != TypeFlag.Battle)
                {
                    PanelHint.AddStep2Header(Lair.GetName());
                    PanelHint.AddStep5Description(Lair.ComponentObjectOfMap.ListHeroesForHint());
                }
                else
                {
                    PanelHint.AddStep2Header("Битва против игрока");
                    PanelHint.AddStep4Level("Игрок: неизвестен");
                    PanelHint.AddStep5Description(Lair.ComponentObjectOfMap.ListHeroesForHint());
                }
            }

            return true;
        }

        /*protected override int GetLevel()
        {
            return Level = (int)Lair.PriorityFlag + 1;
        }*/

        internal override void Draw(Graphics g)
        {
            if (Lair is null)
            {
                ImageIndex = FormMain.IMAGE_INDEX_NONE;
                ImageIsEnabled = false;
                Level = "";
                Quantity = 0;
                LowText = "";
            }
            else
            {
                ImageIsEnabled = Lair.ComponentObjectOfMap.ListHeroesForFlag.Count > 0;
                Quantity = Lair.ComponentObjectOfMap.ListHeroesForFlag.Count();

                if (Lair.ComponentObjectOfMap.TypeFlag != TypeFlag.Battle)
                {
                    ImageIndex = Lair.ComponentObjectOfMap.Visible ? Lair.GetImageIndex() : FormMain.IMAGE_INDEX_UNKNOWN;
                }
                else
                {
                    ImageIndex = FormMain.Config.Gui48_Battle2;
                }
            }

            base.Draw(g);
        }

        internal override void PaintForeground(Graphics g)
        {
            base.PaintForeground(g);

            if ((Lair != null) && ShowFlag)
            {
                int imageIndex;
                switch (Lair.ComponentObjectOfMap.TypeFlag)
                {
                    case TypeFlag.Scout:
                        imageIndex = FormMain.GUI_16_FLAG_SCOUT;
                        break;
                    case TypeFlag.Attack:
                        imageIndex = FormMain.GUI_16_FLAG_ATTACK;
                        break;
                    case TypeFlag.Defense:
                        imageIndex = FormMain.GUI_16_FLAG_DEFENSE;
                        break;
                    case TypeFlag.Battle:
                        imageIndex = FormMain.GUI_16_FLAG_ATTACK;
                        break;
                    default:
                        throw new Exception($"Неизвестный тип флага: {Lair.ComponentObjectOfMap.TypeFlag}.");
                }

                g.DrawImageUnscaled(Program.formMain.ilGui16.GetImage(imageIndex, true, false), Left - (Program.formMain.ilGui16.Size.Width / 2) + 4, Top - (Program.formMain.ilGui16.Size.Height / 2) + 4);
            }
        }

        protected override bool Selected()
        {
            return (Lair != null) && Program.formMain.layerGame.PlayerObjectIsSelected(Lair);
        }
    }
}
