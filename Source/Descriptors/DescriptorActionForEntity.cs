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
    // Класс описателя действия для сущности
    internal sealed class DescriptorActionForEntity : Descriptor
    {
        public DescriptorActionForEntity(XmlNode n) : base()
        {
            Coord = GetPoint(n, "Pos");
            Action = GetString(n, "Action");
            IDCreatedEntity = GetString(n, "Entity");
            DaysCooldown = GetInteger(n, "DaysCooldown");

            if (Action.Length == 0)
            {
                Debug.Assert(IDCreatedEntity.Length > 0);
            }
            else
            {
                Debug.Assert(IDCreatedEntity.Length == 0, $"Указано действие {Action} и указана сущность {IDCreatedEntity}.");
            }

            XmlNode next = n.SelectSingleNode("CellMenu");
            if (next != null)
            {
                NextCell = new DescriptorActionForEntity(next);
                Debug.Assert(Coord.Equals(NextCell.Coord), $"У {IDCreatedEntity} в ячейку {Coord} вложена ячейка {NextCell.Coord}.");
            }

            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
            Debug.Assert(DaysCooldown >= -1);
            Debug.Assert(DaysCooldown <= 100);
        }

        public DescriptorActionForEntity(Point coord) : base()
        {
            Coord = coord;

            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        internal Point Coord { get; }// Координаты в меню
        internal string Action { get; }// Действие (если есть)
        internal string IDCreatedEntity { get; private set; }// ID создаваемой сущности (если есть)
        internal DescriptorWithID CreatedEntity { get; set; }// Описатель создаваемой сущности
        internal int DaysCooldown { get; }// Количество дней до возобновления действия
        internal DescriptorActionForEntity NextCell { get; }// Следующее действие в этих же координатах

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if (IDCreatedEntity.Length > 0)
            {
                CreatedEntity = Descriptors.FindEntity(IDCreatedEntity);
                IDCreatedEntity = "";
            }
        }
    }
}