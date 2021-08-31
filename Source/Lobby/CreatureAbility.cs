using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Cпособность у существа
    internal sealed class CreatureAbility : SmallEntity
    {
        public CreatureAbility(Creature creature, TypeAbility typeAbility)
        {
            Creature = creature;
            TypeAbility = typeAbility;
        }

        internal Creature Creature { get; }
        internal TypeAbility TypeAbility { get; }

        public BitmapList BitmapList()
        {
            return Program.formMain.imListObjects48;
        }

        internal override void Click(VCCell pe)
        {
            
        }

        internal override string GetCost()
        {
            return "";
        }

        internal override void CustomDraw(Graphics g, int x, int y, bool drawState)
        {
            
        }

        internal override int GetImageIndex()
        {
            return TypeAbility.ImageIndex;
        }

        internal override int GetLevel()
        {
            return TypeAbility.MinUnitLevel;
        }

        internal override int GetQuantity()
        {
            return 0;
        }

        internal override bool GetNormalImage()
        {
            return Creature.Level >= TypeAbility.MinUnitLevel;
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeAbility.Name, "", TypeAbility.Description);
            // Здесь добавить требование к уровню
            //$"Уровень: {TypeAbility.MinUnitLevel}"
            //Program.formMain.formHint.
        }
    }
}
