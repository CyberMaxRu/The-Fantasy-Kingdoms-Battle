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

        internal Construction Lair { get => Entity as Construction; }
        internal bool ShowFlag { get; set; } = true;

        internal override void DoClick()
        {
            base.DoClick();

            if ((Lair != null) && (Lair.TypeFlag != TypeFlag.Battle))
            {
                //Program.formMain.ActivatePageLairs(Lair.Location.Settings.Number);
                Program.formMain.SelectPlayerObject(Lair);
            }
        }

        protected override bool AllowClick()
        {
            return base.AllowClick() && (Lair != null) && (Lair.TypeFlag != TypeFlag.Battle);
        }

        internal override bool PrepareHint()
        {
            if (Lair is null)
            { 
                PanelHint.AddSimpleHint(Lair != null ? Lair.NameLair() : "Флаг не назначен");
            }
            else
            {
                Debug.Assert(!(Lair is null));
                if (Lair.TypeFlag != TypeFlag.Battle)
                {
                    PanelHint.AddStep2Header(Lair.NameLair());
                    PanelHint.AddStep5Description(Lair.ListHeroesForHint());
                }
                else
                {
                    PanelHint.AddStep2Header("Битва против игрока");
                    PanelHint.AddStep4Level("Игрок: неизвестен");
                    PanelHint.AddStep5Description(Lair.ListHeroesForHint());
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
                Text = "";
            }
            else
            {
                ImageIsEnabled = Lair.listAttackedHero.Count > 0;
                Quantity = Lair.listAttackedHero.Count();

                if (Lair.TypeFlag != TypeFlag.Battle)
                {
                    if (Lair.Hidden)
                    {
                        ImageIndex = FormMain.IMAGE_INDEX_UNKNOWN;
                    }
                    else
                    {
                        ImageIndex = Lair.ImageIndexLair();
                    }
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
                switch (Lair.TypeFlag)
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
                        throw new Exception($"Неизвестный тип флага: {Lair.TypeFlag}.");
                }

                g.DrawImageUnscaled(Program.formMain.ilGui16.GetImage(imageIndex, true, false), Left - (Program.formMain.ilGui16.Size.Width / 2) + 4, Top - (Program.formMain.ilGui16.Size.Height / 2) + 4);
            }
        }

        protected override bool Selected()
        {
            return (Lair != null) && Program.formMain.PlayerObjectIsSelected(Lair);
        }
    }
}
