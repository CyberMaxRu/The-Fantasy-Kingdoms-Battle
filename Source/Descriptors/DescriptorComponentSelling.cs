using System;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal enum IntervalRefresh { Day, Week };

    // Класс описателя для продажи товаров и услуг героям
    internal sealed class DescriptorComponentSelling : Descriptor
    {
        public DescriptorComponentSelling(DescriptorWithID entity, XmlNode n) : base()
        {
            Gold = entity.GetIntegerFromXmlNode(n, "Gold", 0, 100_000);
            Quantity = entity.GetIntegerFromXmlNode(n, "Quantity", 1, 1_000);
            Duration = entity.GetIntegerFromXmlNode(n, "Duration", 0, 1_000);
            DaysProcessing = entity.GetIntegerFromXmlNode(n, "DaysProcessing", 0, 100);
            IntervalRefresh = (IntervalRefresh)Enum.Parse(typeof(IntervalRefresh), entity.GetStringFromXmlNode(n, "IntervalRefresh"));
        }

        internal int Gold { get; }// Количество золота для покупки товара/услуги
        internal int Quantity { get; }// Количество товара/услуги после обновления
        internal int Duration { get; }// Длительность существования товара/услуги. 0 - бесконечно
        internal int DaysProcessing { get; }// Количество дней для покупки товара/услуги (делится на 10)
        internal IntervalRefresh IntervalRefresh { get; }// Интервал обновления количества товара/услуги
    }
}