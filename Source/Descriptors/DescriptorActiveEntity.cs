using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;    

namespace Fantasy_Kingdoms_Battle
{
    // Класс активной сущности - сооружение, существо
    internal abstract class DescriptorActiveEntity : DescriptorEntity
    {
        public DescriptorActiveEntity(XmlNode n) : base(n)
        {

        }

        internal List<DescriptorEntityForActiveEntity> Entities { get; } = new List<DescriptorEntityForActiveEntity>();// Список всех малых сущностей

        internal void AddEntity(DescriptorEntityForActiveEntity entity)
        {
            Debug.Assert(Entities.IndexOf(entity) == -1);

            Entities.Add(entity);
        }

        internal DescriptorEntityForActiveEntity FindEntity(string idEntity)
        {
            foreach (DescriptorEntityForActiveEntity e in Entities)
            {
                if (e.ID == idEntity)
                    return e;
            }

            throw new Exception($"В {ID} сущность {idEntity} не найдена.");
        }
    }
}