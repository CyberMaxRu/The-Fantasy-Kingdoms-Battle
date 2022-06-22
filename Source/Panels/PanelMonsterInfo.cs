using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class PanelMonsterInfo : PanelCreatureInfo
    {
        private VCIconAndDigitValue lvGreatness;

        public PanelMonsterInfo(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
        }

        internal Creature Monster { get => Entity as Creature; }

        protected override void CreateCustomControls()
        {
            base.CreateCustomControls();

            lvGreatness = new VCIconAndDigitValue(this, FormMain.Config.GridSize, lvGold.NextTop() - FormMain.Config.GridSizeHalf, imgIcon.Width, FormMain.GUI_16_GREATNESS);

            separator.ShiftY = lvGreatness.NextTop();
            pageControl.ShiftY = separator.NextTop();
        }

        internal override void Draw(Graphics g)
        {
            lvGold.Text = Monster.TypeCreature.TypeReward.Cost.Gold.ToString();
            lvGreatness.Text = Monster.TypeCreature.TypeReward.Greatness.ToString();

            base.Draw(g);
        }
    }
}
