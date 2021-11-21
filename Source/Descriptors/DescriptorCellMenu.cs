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
    // Класс описателя ячейки меню
    internal sealed class DescriptorCellMenu : Descriptor
    {
        public DescriptorCellMenu(DescriptorActiveEntity forEntity, XmlNode n) : base()
        {
            ForEntity = forEntity;

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
                Debug.Assert(IDCreatedEntity.Length == 0, $"В {forEntity.ID} указано действие {Action} и указана сущность {IDCreatedEntity}.");
            }

            XmlNode next = n.SelectSingleNode("CellMenu");
            if (next != null)
            {
                NextCell = new DescriptorCellMenu(ForEntity, next);
                Debug.Assert(Coord.Equals(NextCell.Coord), $"У {IDCreatedEntity} в ячейку {Coord} вложена ячейка {NextCell.Coord}.");
            }

            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
            Debug.Assert(DaysCooldown >= -1);
            Debug.Assert(DaysCooldown <= 100);
        }

        public DescriptorCellMenu(DescriptorActiveEntity forEntity, Point coord) : base()
        {
            ForEntity = forEntity;
            Coord = coord;

            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        internal DescriptorActiveEntity ForEntity { get; }// Для какой активной сущности
        internal Point Coord { get; }// Координаты в меню
        internal string Action { get; }// Действие
        internal string IDCreatedEntity { get; private set; }// ID создаваемой сущности (если есть)
        internal DescriptorWithID CreatedEntity { get; set; }// Описатель создаваемой сущности
        internal int DaysCooldown { get; }// Количество дней до возобновления действия
        internal DescriptorCellMenu NextCell { get; }// Следующая ячейка

        internal override void TuneLinks()
        {
            base.TuneLinks();

            CreatedEntity = Descriptors.FindEntity(IDCreatedEntity);
            IDCreatedEntity = "";
        }
    }
}