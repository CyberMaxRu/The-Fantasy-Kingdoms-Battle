using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum IntervalRefresh { Day, Week, TwoWeek, Month };

    internal class DescriptorSelling : Descriptor
    {
        public DescriptorSelling(DescriptorWithID entity, XmlNode n) : base()
        {
            Entity = entity;

            Gold = GetIntegerNotNull(n, "Gold");
            Quantity = GetIntegerNotNull(n, "Quantity");
            Duration = GetIntegerNotNull(n, "Duration");
            MovePoints = GetIntegerNotNull(n, "MovePoints");
            IntervalRefresh = (IntervalRefresh)Enum.Parse(typeof(IntervalRefresh), GetStringNotNull(n, "IntervalRefresh"));

            Debug.Assert(Gold >= 0, $"У {entity} отрицательное число стоимости ({Gold}).");
            Debug.Assert(Gold <= 100_000);
            Debug.Assert(Quantity > 0, $"У {entity} ошибк в количестве ({Quantity}).");
            Debug.Assert(Quantity <= 1_000, $"У {entity} ошибк в количестве ({Quantity}).");
            Debug.Assert(Duration > 0, $"У {entity} ошибк в количестве ({Duration}).");
            Debug.Assert(Duration <= 1_000, $"У {entity} ошибк в количестве ({Duration}).");
            Debug.Assert(MovePoints > 0, $"У {entity} ошибк в количестве ({MovePoints}).");
            Debug.Assert(MovePoints <= 1_000, $"У {entity} ошибк в количестве ({MovePoints}).");
        }

        internal DescriptorWithID Entity { get; }// Для какой активной сущности
        internal int Gold { get; }// Количество требуемых строителей
        internal int Quantity { get; }// Количество требуемых строителей
        internal int Duration { get; }// Количество требуемых строителей
        internal int MovePoints { get; }// Количество требуемых строителей
        internal IntervalRefresh IntervalRefresh { get; }
    }
}