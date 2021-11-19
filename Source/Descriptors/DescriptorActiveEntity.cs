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
            // Загружаем меню
            XmlNode nr = n.SelectSingleNode("CellsMenu");
            if (nr != null)
            {
                DescriptorCellMenu research;

                foreach (XmlNode l in nr.SelectNodes("CellMenu"))
                {
                    research = new DescriptorCellMenu(this, l);

                    foreach (DescriptorCellMenu cm in CellsMenu)
                    {
                        Debug.Assert(!cm.Coord.Equals(p), $"У {ID} в ячейке ({p.X + 1}, {p.Y + 1}) уже есть сущность.");
                    }

                    foreach (DescriptorCellMenu tcm in CellsMenu)
                    {
                        //Debug.Assert(research.Construction. NameTypeObject != tcm.NameTypeObject, $"У {ID} в меню повторяется объект {research.NameTypeObject}.");
                    }

                    CellsMenu.Add(research);
                }
            }
        }

        internal List<DescriptorEntityForActiveEntity> Entities { get; } = new List<DescriptorEntityForActiveEntity>();// Список всех малых сущностей
        internal List<DescriptorCellMenu> CellsMenu { get; } = new List<DescriptorCellMenu>();// Меню активной сущности

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