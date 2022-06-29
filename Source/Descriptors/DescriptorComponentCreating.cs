using System.Xml;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс описателя с информацией о создании сущности
    internal sealed class DescriptorComponentCreating : Descriptor
    {
        private int constructionPoints;

        public DescriptorComponentCreating(DescriptorWithID entity, XmlNode n) : base()
        {
            Entity = entity;
            cons
            CostResources = new ListBaseResources(n.SelectSingleNode("Cost"));
            Requirements = new ListDescriptorRequirements(entity, n.SelectSingleNode("Requirements"));
        }

        internal DescriptorWithID Entity { get; }
        //internal int ConstructionPoints { get; }
        internal int CalcConstructionPoints(Player p) => p.CheatingIgnoreBuilders ? 0 : constructionPoints;// Количество требуемых очков строительства
        internal ListBaseResources CostResources { get; }// Стоимость (в базовых ресурсах)
        internal ListDescriptorRequirements Requirements { get; }// Список требований для выполнения действия
        internal int DaysProcessing { get; } = 1;

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if (Entity is DescriptorConstructionStructure dcs)
            {
                constructionPoints = dcs.Durability;
            }

            Requirements.TuneLinks();
        }
    }
}