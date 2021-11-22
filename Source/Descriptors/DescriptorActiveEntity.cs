using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;    

namespace Fantasy_Kingdoms_Battle
{
    // Класс активной сущности, которая имеет своё меню - сооружение, существо, локация, игрок
    internal abstract class DescriptorActiveEntity : DescriptorEntity
    {
        public DescriptorActiveEntity(XmlNode n) : base(n)
        {
            // Загружаем меню
            XmlNode ncm = n.SelectSingleNode("CellsMenu");
            if (ncm != null)
            {
                foreach (XmlNode l in ncm.SelectNodes("CellMenu"))
                {
                    DescriptorCellMenu dcm = new DescriptorCellMenu(this, l);

                    foreach (DescriptorCellMenu dcm2 in CellsMenu)
                    {
                        Debug.Assert(!dcm2.Coord.Equals(dcm.Coord), $"У {ID} ячейка ({dcm.Coord.X + 1}, {dcm.Coord.Y + 1}) уже занята.");
                        if ((dcm2.IDCreatedEntity.Length > 0) && (dcm.IDCreatedEntity.Length > 0))
                            Debug.Assert(dcm2.IDCreatedEntity != dcm.IDCreatedEntity, $"У {ID} в меню повторяется объект {dcm.IDCreatedEntity}.");
                    }

                    CellsMenu.Add(dcm);
                }
            }
        }

        internal List<DescriptorEntityForActiveEntity> Entities { get; } = new List<DescriptorEntityForActiveEntity>();// Список всех малых сущностей
        internal List<DescriptorCellMenu> CellsMenu { get; } = new List<DescriptorCellMenu>();// Меню

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