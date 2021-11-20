using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorConstructionLevel : DescriptorEntityForConstruction
    {
        public DescriptorConstructionLevel(DescriptorConstruction forConstruction, int number, XmlNode n) : base(forConstruction, n)
        {
            //Debug.Assert(Creating != null);// Для логов не надо

            Number = number;
            Builders = GetInteger(n, "Builders");
            MaxInhabitant = GetInteger(n, "MaxInhabitant");
            Capacity = GetInteger(n, "Capacity");
            GreatnessByConstruction = GetInteger(n, "GreatnessByConstruction");
            GreatnessPerDay = GetInteger(n, "GreatnessPerDay");
            BuildersPerDay = GetInteger(n, "BuildersPerDay");

            // Посещение
            XmlNode nv = n.SelectSingleNode("Visit");
            if (nv != null)
            {
                DescriptorVisit = new DescriptorConstructionVisit(this, nv);
            }

            Extensions = new ListSmallEntity(forConstruction, n.SelectSingleNode("Entities"));

            // Загружаем перки, которые дает сооружение
            ListPerks = new ListDescriptorPerks(n.SelectSingleNode("Perks"));

            if ((forConstruction.Category == CategoryConstruction.Lair) || (forConstruction.Category == CategoryConstruction.Place))
            {
                //Debug.Assert(DaysBuilding == 0);
                //Debug.Assert(Builders == 0);
            }
            else
            {
                //Debug.Assert(DaysBuilding >= 1, $"У {forConstruction.ID}.{number} DaysBuilding = 0.");
                //Debug.Assert(Builders >= 0);
            }

            Debug.Assert(Number >= 0);
            Debug.Assert(Number <= 5);
            Debug.Assert(MaxInhabitant >= 0);
            Debug.Assert(MaxInhabitant <= 100);
            Debug.Assert(Capacity >= 0);
            Debug.Assert(Capacity <= 100);
            Debug.Assert(GreatnessByConstruction >= 0);
            Debug.Assert(GreatnessPerDay >= 0);
            Debug.Assert(BuildersPerDay >= 0);
        }

        internal int Number { get; }
        internal int Builders { get; }// Количество требуемых строителей
        internal int MaxInhabitant { get; }
        internal int Capacity { get; }
        internal int GreatnessByConstruction { get; }// Дает очков Величия при постройке
        internal int GreatnessPerDay { get; }// Дает очков Величия в день
        internal int BuildersPerDay { get; }// Дает строителей в день
        internal ListSmallEntity Extensions { get; }// Сущности, относящиеся к уровню
        internal DescriptorConstructionVisit DescriptorVisit { get; }// Товар для посещения сооружения
        internal ListDescriptorPerks ListPerks { get; }// Перки, которые дает уровень сооружения

        internal override string GetTypeEntity()
        {
            return "Повышение уровня";
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Extensions.TuneDeferredLinks();
            ListPerks.TuneDeferredLinks();
        }
    }
}
