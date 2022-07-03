using System.Xml;
using static Fantasy_Kingdoms_Battle.Utils;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс описателя с информацией о создании сущности
    internal sealed class DescriptorComponentCreating : Descriptor
    {
        public DescriptorComponentCreating(DescriptorWithID entity, XmlNode n) : base()
        {
            Entity = entity;
            ConstructionPoints = GetInteger(n, "ConstructionPoints");
            ResearchPoints = GetInteger(n, "ResearchPoints");
            CostResources = new ListBaseResources(n.SelectSingleNode("Cost"));
            Requirements = new ListDescriptorRequirements(entity, n.SelectSingleNode("Requirements"));

            Assert(ConstructionPoints >= 0, $"ID: {entity.ID}, ConstructionPoints: {ConstructionPoints}");
            Assert(ConstructionPoints <= FormMain.Config.DefaultConstructionPoints * 50, $"ID: {entity.ID}, ConstructionPoints: {ConstructionPoints}");
            Assert(ResearchPoints >= 0, $"ID: {entity.ID}, ResearchPoints: {ResearchPoints}");
            Assert(ResearchPoints <= FormMain.Config.DefaultResearchPoints * 50, $"ID: {entity.ID}, ResearchPoints: {ResearchPoints}");


            if (Entity is DescriptorCreature)
            { 
                //Assert((ConstructionPoints == 0) && (ResearchPoints == 0), $"ID: {entity.ID}, ConstructionPoints: {ConstructionPoints}, ResearchPoints: {ResearchPoints}");
            }
            else
            {
                Assert(((ConstructionPoints > 0) && (ResearchPoints == 0)) || ((ConstructionPoints == 0) && (ResearchPoints > 0)), $"ID: {entity.ID}, ConstructionPoints: {ConstructionPoints}, ResearchPoints: {ResearchPoints}");
            }
        }

        internal DescriptorWithID Entity { get; }
        internal int ConstructionPoints { get; }// Требуемые очки строительства
        internal int ResearchPoints { get; }// Требуемые очки исследования
        internal int CalcConstructionPoints(Player p) => p.CheatingIgnoreBuilders ? 0 : ConstructionPoints;// Количество требуемых очков строительства
        internal ListBaseResources CostResources { get; }// Стоимость (в базовых ресурсах)
        internal ListDescriptorRequirements Requirements { get; }// Список требований для выполнения действия
        internal int DaysProcessing { get; } = 1;

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Requirements.TuneLinks();
        }
    }
}