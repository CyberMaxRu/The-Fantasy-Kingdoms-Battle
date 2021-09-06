using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Cпособность у существа
    internal sealed class Ability : SmallEntity
    {
        public Ability(Creature creature, DescriptorAbility typeAbility) : base()
        {
            Creature = creature;
            TypeAbility = typeAbility;
        }

        internal Creature Creature { get; }
        internal DescriptorAbility TypeAbility { get; }

        internal override void Click(VCCell pe)
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
