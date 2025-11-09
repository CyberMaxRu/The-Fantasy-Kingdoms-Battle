using System;
using System.Xml;
using static Fantasy_Kingdoms_Battle.Utils;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс описателя с информацией о создании сущности
    internal enum TypeCreating { Building, Research, Hire, MassEvent, Tournament, Extra };

    internal sealed class DescriptorComponentCreating : Descriptor
    {
        public DescriptorComponentCreating(DescriptorWithID entity, XmlNode n) : base()
        {
            Entity = entity;
            Cost = GetInteger(n, "Cost");
            Requirements = new ListDescriptorRequirements(entity, n.SelectSingleNode("Requirements"));

            if (Entity is DescriptorCreature)
            {
                //Assert((ConstructionPoints == 0) && (ResearchPoints == 0), $"ID: {entity.ID}, ConstructionPoints: {ConstructionPoints}, ResearchPoints: {ResearchPoints}");
            }

            if (Entity is DescriptorConstructionLevel)
                TypeCreating = TypeCreating.Building;
            else if (Entity is DescriptorConstructionExtension)
                TypeCreating = TypeCreating.Building;
            else if (Entity is DescriptorAbility)
                TypeCreating = TypeCreating.Research;
            else if (Entity is DescriptorProduct)
                TypeCreating = TypeCreating.Research;
            else if (Entity is DescriptorConstructionService)
                TypeCreating = TypeCreating.Research;
            else if (Entity is DescriptorConstructionMassEvent)
                TypeCreating = TypeCreating.Research;
            else if (Entity is DescriptorConstructionImprovement)
                TypeCreating = TypeCreating.Research;
            else if (Entity is DescriptorCreature)
                TypeCreating = TypeCreating.Hire;
            else
                DoException("Неизвестный тип создаваемой сущности: " + Entity.ToString());
        }

        internal DescriptorWithID Entity { get; }
        internal TypeCreating TypeCreating { get; }// Тип создаваемой сущности
        internal int Cost { get; }// Стоимость
        internal ListDescriptorRequirements Requirements { get; }// Список требований для выполнения действия

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Requirements.TuneLinks();
        }
    }
}