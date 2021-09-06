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
            Descriptor = typeAbility;
        }

        internal Creature Creature { get; }
        internal DescriptorAbility Descriptor { get; }

        internal override int GetImageIndex() => Descriptor.ImageIndex;
        internal override int GetLevel() => Descriptor.MinUnitLevel;
        internal override bool GetNormalImage() => Creature.Level >= Descriptor.MinUnitLevel;

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Descriptor.Name, "", Descriptor.Description);
        }
    }
}
