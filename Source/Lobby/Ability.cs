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

        internal override int GetImageIndex() => TypeAbility.ImageIndex;
        internal override int GetLevel() => TypeAbility.MinUnitLevel;
        internal override bool GetNormalImage() => Creature.Level >= TypeAbility.MinUnitLevel;

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeAbility.Name, "", TypeAbility.Description);
        }
    }
}
