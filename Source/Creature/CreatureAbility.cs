using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Cпособность у существа
    internal sealed class CreatureAbility : ICell
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
            return Program.formMain.ilItems;
        }

        public void Click(VCCell pe)
        {
            
        }

        public string Cost()
        {
            return null;
        }

        public void CustomDraw(Graphics g, int x, int y, bool drawState)
        {
            
        }

        public int ImageIndex()
        {
            return TypeAbility.ImageIndex;
        }

        public int Level()
        {
            return TypeAbility.MinUnitLevel;
        }

        public bool NormalImage()
        {
            return Creature.Level >= TypeAbility.MinUnitLevel;
        }

        public void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeAbility.Name, "", TypeAbility.Description);
            // Здесь добавить требование к уровню
            //$"Уровень: {TypeAbility.MinUnitLevel}"
            //Program.formMain.formHint.
        }

        public int Quantity()
        {
            return 0;
        }
    }
}
