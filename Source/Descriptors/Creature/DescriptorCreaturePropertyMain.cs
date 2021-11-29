using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настройки свойства для типа существ
    internal class DescriptorCreaturePropertyMain : Descriptor
    {
        public DescriptorCreaturePropertyMain(XmlNode n) : base()
        {
            MinValueOnHire = GetIntegerNotNull(n, "MinValueOnHire");
            MaxValueOnHire = GetIntegerNotNull(n, "MaxValueOnHire");
            ChangePerLevel = GetInteger(n, "ChangePerLevel");

            Debug.Assert(MinValueOnHire >= -Config.MaxValueProperty);
            Debug.Assert(MinValueOnHire <= Config.MaxValueProperty);
            Debug.Assert(MaxValueOnHire >= -Config.MaxValueProperty);
            Debug.Assert(MaxValueOnHire <= Config.MaxValueProperty);
            Debug.Assert(MinValueOnHire <= MaxValueOnHire);
            Debug.Assert(ChangePerLevel >= -Config.MaxValueProperty);
            Debug.Assert(ChangePerLevel <= Config.MaxValueProperty);
        }

        internal int MinValueOnHire { get; }// Минимальное значение при найме
        internal int MaxValueOnHire { get; }// Максимальное значение при найме
        internal int ChangePerLevel { get; }// Изменение при повышении уровня
    }
}