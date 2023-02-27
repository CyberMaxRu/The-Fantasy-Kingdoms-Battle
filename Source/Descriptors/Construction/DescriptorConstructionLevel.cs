using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class DescriptorConstructionLevel : DescriptorConstructionStructure
    {
        public DescriptorConstructionLevel(DescriptorConstruction forConstruction, XmlNode n) : base(forConstruction, n)
        {
            //Debug.Assert(Creating != null);// Для логов не надо

            Number = GetIntegerNotNull(n, "Number", ID, 1, 5);
            MaxInhabitant = GetInteger(n, "MaxInhabitant");
            Tax = GetInteger(n, "Tax");
            Capacity = GetInteger(n, "Capacity");
            GreatnessByConstruction = GetInteger(n, "GreatnessByConstruction");
            GreatnessPerDay = GetInteger(n, "GreatnessPerDay");
            AddConstructionPoints = GetInteger(n, "AddConstructionPoints");

            // Посещение
            XmlNode nv = n.SelectSingleNode("Visit");
            if (nv != null)
            {
                DescriptorVisit = new DescriptorConstructionVisitSimple(this, nv);
            }
            else
            {
                DescriptorVisit = new DescriptorConstructionVisitSimple(this);
            }
            Extensions = new ListSmallEntity(forConstruction, n.SelectSingleNode("Entities"));

            // Загружаем перки, которые дает сооружение
            ListPerks = new ListDescriptorPerks(n.SelectSingleNode("Perks"));

            // Загружаем характеристики
            XmlNode np = n.SelectSingleNode("Properties");
            if (np != null)
            {
                Properties = new ListDefaultProperties(np);
            }

            //
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

            if (Number > 1)
                GetCreating().Requirements.Insert(0, new RequirementConstruction(this, forConstruction.ID, Number - 1, GetCreating().Requirements));

            XmlNode nm = n.SelectSingleNode("Mining");
            if (nm != null)
                Mining = new ListCoefMining(nm);

            XmlNode nsp = n.SelectSingleNode("SettlementParameters");
            if (nsp != null)
                SettlementParameters = new ListSettlementParameters(nsp);
            
            XmlNode nir = n.SelectSingleNode("IncomeResources");
            if (nir != null)
                IncomeResources = new ListBaseResources(nir);

            if (Number > 1)
            {
                Debug.Assert(Durability.Value > forConstruction.Levels[Number - 1].Durability.Value);
            }
            else
            {
                ImageIndex = forConstruction.ImageIndex;
            }

            Debug.Assert(MaxInhabitant >= 0);
            Debug.Assert(MaxInhabitant <= 100);
            Debug.Assert(Tax >= 0);
            Debug.Assert(Tax <= 90);
            Debug.Assert(Capacity >= 0);
            Debug.Assert(Capacity <= 100);
            Debug.Assert(GreatnessByConstruction >= 0);
            Debug.Assert(GreatnessPerDay >= 0);
            Debug.Assert(AddConstructionPoints >= 0);

            if ((forConstruction.Category != CategoryConstruction.Guild) && (forConstruction.Category != CategoryConstruction.Temple))
            {
                Debug.Assert(Tax == 0);
            }

            // Нельзя давать ресурсы и одновременно добывать их
            Debug.Assert(!((Mining != null) && (IncomeResources != null)));
        }

        internal int Number { get; }
        internal int MaxInhabitant { get; }
        internal int Tax { get; }// Процент налога с дохода членов гильдии
        internal int Capacity { get; }
        internal int GreatnessByConstruction { get; }// Дает очков Величия при постройке
        internal int GreatnessPerDay { get; }// Дает очков Величия в день
        internal int AddConstructionPoints { get; }// Дополнительное количество очков строительства в день
        internal ListSmallEntity Extensions { get; }// Сущности, относящиеся к уровню
        internal DescriptorConstructionVisitSimple DescriptorVisit { get; }// Товар для посещения сооружения
        internal ListDescriptorPerks ListPerks { get; }// Перки, которые дает уровень сооружения
        internal ListDefaultProperties Properties { get; }// Список характеристик
        internal ListCoefMining Mining { get; }// Коэффициенты добычи ресурса
        internal ListSettlementParameters SettlementParameters { get; }// Изменение параметров нас. пункта
        internal ListBaseResources IncomeResources { get; }// Сколько и каких ресурсов приносит сооружение в день


        protected override string GetName(XmlNode n)
        {
            return "Уровень " + GetIntegerNotNull(n, "Number");
        }

        internal override string GetTypeEntity()
        {
            return "Повышение уровня";
        }

        internal override int GetImageIndex(XmlNode n) => Config.Gui48_LevelUp;

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if (Number == 1)
                ImageIndex = ActiveEntity.ImageIndex;

            Extensions.TuneLinks();
            ListPerks.TuneLinks();

            if ((Number == 1) && (GetCreating() != null))
            {
                //Assert(GetCreating().ConstructionPoints == Durability.Value, $"ID: {ID}, ConstructionPoints = {GetCreating().ConstructionPoints}, Durability = {Durability}");
            }
        }
    }
}
