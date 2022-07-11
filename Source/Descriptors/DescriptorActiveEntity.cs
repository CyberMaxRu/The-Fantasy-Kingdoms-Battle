using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.Utils;

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
                    DescriptorActionForEntity dcm = new DescriptorActionForEntity(l);

                    foreach (DescriptorActionForEntity dcm2 in CellsMenu)
                    {
                        Debug.Assert(!dcm2.Coord.Equals(dcm.Coord), $"У {ID} ячейка ({dcm.Coord.X + 1}, {dcm.Coord.Y + 1}) уже занята.");
                        if ((dcm2.IDCreatedEntity.Length > 0) && (dcm.IDCreatedEntity.Length > 0))
                            Debug.Assert(dcm2.IDCreatedEntity != dcm.IDCreatedEntity, $"У {ID} в меню повторяется объект {dcm.IDCreatedEntity}.");
                    }

                    CellsMenu.Add(dcm);
                }
            }
        }

        internal SortedList<string, DescriptorEntityForActiveEntity> Entities { get; } = new SortedList<string, DescriptorEntityForActiveEntity>();// Список всех малых сущностей
        internal List<DescriptorActionForEntity> CellsMenu { get; } = new List<DescriptorActionForEntity>();// Меню

        internal override void TuneLinks()
        {
            base.TuneLinks();

            foreach (KeyValuePair<string, DescriptorEntityForActiveEntity> e2 in Entities)
            {
                e2.Value.TuneLinks();
            }
        }

        internal override void AfterTuneLinks()
        {
            base.AfterTuneLinks();

            foreach (KeyValuePair<string, DescriptorEntityForActiveEntity> e2 in Entities)
            {
                e2.Value.AfterTuneLinks();
            }
        }

        internal void AddEntity(DescriptorEntityForActiveEntity entity)
        {
            Debug.Assert(!Entities.ContainsKey(entity.ID));

            foreach (KeyValuePair<string, DescriptorEntityForActiveEntity> e2 in Entities)
            {
                Assert(e2.Value.Name != entity.Name, $"Одинаковое имя {entity.Name} у {entity.ID} и {e2.Value.ID}.");
                //Assert(e2.Value.ImageIndex != entity.ImageIndex);
            }

            Entities.Add(entity.ID, entity);
        }

        internal DescriptorEntityForActiveEntity FindEntity(string idEntity)
        {
            if (Entities.TryGetValue(idEntity, out DescriptorEntityForActiveEntity entity))
                return entity;

            throw new Exception($"{ID}: сущность {idEntity} не найдена.");
        }
    }
}