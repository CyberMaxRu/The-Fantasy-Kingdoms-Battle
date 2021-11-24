using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс описателя с информацией о создании сущности
    internal sealed class DescriptorComponentCreating : Descriptor
    {
        public DescriptorComponentCreating(DescriptorWithID entity, XmlNode n) : base()
        {
            Builders = entity.GetIntegerFromXmlNode(n, "Builders", 0, 10);
            DaysProcessing = entity.GetIntegerFromXmlNode(n, "DaysProcessing", 0, 100);
            CostResources = new ListBaseResources(n.SelectSingleNode("Cost"));
            Requirements = new ListDescriptorRequirements(entity, n.SelectSingleNode("Requirements"));
        }

        internal int Builders { get; }// Количество требуемых строителей
        internal int DaysProcessing { get; }// Количество дней для выполнения действия
        internal ListBaseResources CostResources { get; }// Стоимость (в базовых ресурсах)
        internal ListDescriptorRequirements Requirements { get; }// Список требований для выполнения действия

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Requirements.TuneLinks();
        }
    }
}