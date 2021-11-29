using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class Perk : SmallEntity
    {
        public Perk(BigEntity creature, DescriptorPerk descriptor, Entity owner, int counter = 0) : base()
        {
            Debug.Assert(descriptor != null);
            Debug.Assert(owner != null);

            BigEntity = creature;
            Descriptor = descriptor;
            Owner = owner;
            Counter = counter;

            ListProperty = descriptor.ListProperty;
        }

        public Perk(BigEntity creature, List<DescriptorCreatureProperty> list) : base()
        {
            BigEntity = creature;

            ListProperty = new int[FormMain.Descriptors.PropertiesCreature.Count];

            foreach (DescriptorCreatureProperty property in list)
            {
                ListProperty[property.Descriptor.Index] = creature.Lobby.Rnd.Next(property.MinValueOnHire, property.MaxValueOnHire + 1);
            }
        }

        internal BigEntity BigEntity { get; }
        internal DescriptorPerk Descriptor { get; }
        internal int Counter { get; private set; }// Счетчик действия
        internal Entity Owner { get; }// Владелец перка
        internal int[] ListProperty { get; }

        internal override int GetImageIndex() => Descriptor != null ? Descriptor.ImageIndex : BigEntity.GetImageIndex();

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep2Header(Descriptor != null ? Descriptor.Name : BigEntity.Descriptor.Name, GetImageIndex());
            if (Descriptor != null)
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
            Program.formMain.formHint.AddStep9Properties(ListProperty);
            Program.formMain.formHint.AddStep18Owner(Owner);
        }
    }
}
