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
            Action = GetStringNotNull(n, "Action");
            IDCreatedEntity = GetStringNotNull(n, "Entity");
            DaysProcessing = GetInteger(n, "DaysProcessing");
            DaysCooldown = GetInteger(n, "DaysCooldown");
            CostResources = new ListBaseResources(n.SelectSingleNode("Cost"));
            Requirements = new ListDescriptorRequirements(forEntity, n.SelectSingleNode("Requirements"));

            XmlNode next = n.SelectSingleNode("CellMenu");
            if (next != null)
            {
                NextCell = new DescriptorCellMenu(ForEntity, next);
                Debug.Assert(Coord.Equals(NextCell.Coord), $"У {IDCreatedEntity} в ячейку {Coord} вложена ячейка {NextCell.Coord}.");
            }


            Debug.Assert(Coord.X <= Descriptors.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Descriptors.PlateHeight - 1);
            Debug.Assert(IDCreatedEntity.Length > 0);
            Debug.Assert(DaysProcessing >= 0, $"В {ForEntity.ID} отрицательное число дней процесса у {IDCreatedEntity}");
            Debug.Assert(DaysProcessing <= 100);
            Debug.Assert(DaysCooldown >= -1);
            Debug.Assert(DaysCooldown <= 100);
        }

        public DescriptorCellMenu(DescriptorActiveEntity forEntity, Point coord) : base()
        {
            ForEntity = forEntity;
            Coord = coord;

            CostResources = new ListBaseResources();
            Requirements = new ListDescriptorRequirements(forEntity, null);

            Debug.Assert(Coord.X <= Descriptors.PlateWidth - 1);
            Debug.Assert(Coord.Y <= Descriptors.PlateHeight - 1);
        }

        internal DescriptorActiveEntity ForEntity { get; }// Для какой активной сущности
        internal Point Coord { get; }// Координаты в меню
        internal string Action { get; }// Действие
        internal string IDCreatedEntity { get; }// ID создаваемой сущности (если есть)
        internal DescriptorEntity CreatedEntity { get; set; }// Описатель создаваемой сущности
        internal int DaysProcessing { get; }// Количество дней для выполнения действия
        internal int DaysCooldown { get; }// Количество дней до возобновления действия
        internal ListBaseResources CostResources { get; }// Стоимость
        internal ListDescriptorRequirements Requirements { get; }// Список требований для создания
        internal DescriptorCellMenu NextCell { get; }

        internal override void TuneLinks()
        {
            base.TuneLinks();

        }
    }
}