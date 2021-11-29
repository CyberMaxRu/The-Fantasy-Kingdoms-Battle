using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class EntityProperties : List<CreatureProperty>
    {
        public EntityProperties(BigEntity entity, ListDefaultProperties listDefaults)
        {
            BigEntity = entity;

            Capacity = FormMain.Descriptors.PropertiesCreature.Count;

            for (int i = 0; i < Capacity; i++)
                Add(null);

            if (listDefaults != null)
            {
                foreach (DescriptorCreatureProperty dcp in listDefaults)
                    this[dcp.Descriptor.Index] = new CreatureProperty(entity, dcp.Descriptor);
            }
        }

        internal BigEntity BigEntity { get; }
    }
}