using System;
using System.Xml;
using static Fantasy_Kingdoms_Battle.Utils;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс описателя с информацией о создании сущности
    internal enum TypeCreating { Building, Research, Hire };

    internal sealed class DescriptorComponentCreating : Descriptor
    {
        public DescriptorComponentCreating(DescriptorWithID entity, XmlNode n) : base()
        {
            Entity = entity;
            Time = GetIntegerNotNull(n, "Time");
            Builders = GetInteger(n, "Builders");
            CostResources = new ListBaseResources(n.SelectSingleNode("Cost"));
            Requirements = new ListDescriptorRequirements(entity, n.SelectSingleNode("Requirements"));

            Assert(Time > 0, $"ID: {entity.ID}, Time: {Time}");
            Assert(Builders >= 0, $"ID: {entity.ID}, Builders: {Builders}");

            if (Entity is DescriptorCreature)
            {
                //Assert((ConstructionPoints == 0) && (ResearchPoints == 0), $"ID: {entity.ID}, ConstructionPoints: {ConstructionPoints}, ResearchPoints: {ResearchPoints}");
            }

            if (Entity is DescriptorConstruction)
                TypeCreating = TypeCreating.Building;
            else if (Entity is DescriptorAbility)
                TypeCreating = TypeCreating.Research;
            else if (Entity is DescriptorCreature)
                TypeCreating = TypeCreating.Hire;
            else
                DoException("Неизвестный тип создаваемой сущности: " + Entity.ToString());
        }

        internal DescriptorWithID Entity { get; }
        internal TypeCreating TypeCreating { get; }// Тип создаваемой сущности
        internal int Time { get; }//Время создания (в секундах)
        internal int Builders { get; }//Количество одновременно требуемых строителей
        internal ListBaseResources CostResources { get; }// Стоимость (в базовых ресурсах)
        internal ListDescriptorRequirements Requirements { get; }// Список требований для выполнения действия

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Requirements.TuneLinks();
        }
    }
}