using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Perk : EntityForCreature
    {
        public Perk(Creature creature, DescriptorPerk descriptor, Entity owner, int counter = 0) : base(creature)
        {
            Debug.Assert(creature != null);
            Debug.Assert(descriptor != null);
            Debug.Assert(owner != null);

            Descriptor = descriptor;
            Owner = owner;
            Counter = counter;
        }

        internal DescriptorPerk Descriptor { get; }
        internal int Counter { get; private set; }// Счетчик действия
        internal Entity Owner { get; }// Владелец перка

        internal override int GetImageIndex() => Descriptor.ImageIndex;

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Descriptor.Name, Descriptor.ImageIndex);
            Program.formMain.formHint.AddStep5Description(Descriptor.Description);
            Program.formMain.formHint.AddStep9Honor(Descriptor.GetValueProperty(NamePropertyCreature.Honor));
            Program.formMain.formHint.AddStep9Enthusiasm(Descriptor.GetValueProperty(NamePropertyCreature.Enthusiasm));
            Program.formMain.formHint.AddStep9Morale(Descriptor.GetValueProperty(NamePropertyCreature.Morale));
            Program.formMain.formHint.AddStep9Luck(Descriptor.GetValueProperty(NamePropertyCreature.Luck));
            Program.formMain.formHint.AddStep18Owner(Owner);
        }
    }
}
