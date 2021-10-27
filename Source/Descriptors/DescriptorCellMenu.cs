using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Deployment.Internal;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{

    // Класс описателя ячейки меню
    internal abstract class DescriptorCellMenu : Descriptor
    {
        public DescriptorCellMenu(DescriptorEntity forEntity, XmlNode n) : base()
        {
            ForEntity = forEntity;

            Coord = GetPoint(n, "Pos");
            Cost = GetInteger(n, "Cost");
            LoadRequirements(this, Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        public DescriptorCellMenu(DescriptorEntity forEntity, Point coord, XmlNode n) : base()
        {
            ForEntity = forEntity;

            Coord = coord;
            Cost = GetInteger(n, "Cost");
            LoadRequirements(this, Requirements, n);

            Debug.Assert(Coord.X >= 0);
            Debug.Assert(Coord.X <= Config.PlateWidth - 1);
            Debug.Assert(Coord.Y >= 0);
            Debug.Assert(Coord.Y <= Config.PlateHeight - 1);
        }

        internal DescriptorEntity ForEntity { get; }
        internal Point Coord { get; }// Координаты ячейки
        internal int Cost { get; }// Стоимость
        internal List<Requirement> Requirements { get; } = new List<Requirement>();// Список требований

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            foreach (Requirement r in Requirements)
                r.TuneDeferredLinks();
        }
    }

    internal enum TypeCellMenuForConstruction { Research, Event, HireCreature, Build, LevelUp, Action, Extension, Tournament, Extra };

    internal class DescriptorCellMenuForConstruction : DescriptorCellMenu
    {
        public DescriptorCellMenuForConstruction(DescriptorConstruction forConstruction, XmlNode n) : base(forConstruction, n)
        {
            ForConstruction = forConstruction;

            Type = (TypeCellMenuForConstruction)Enum.Parse(typeof(TypeCellMenuForConstruction), n.SelectSingleNode("Type").InnerText);
            NameEntity = GetStringNotNull(n, "Entity");
            Cooldown = GetInteger(n, "Cooldown");
            Income = GetInteger(n, "Income");

            Debug.Assert(NameEntity.Length > 0);
            Debug.Assert(Income >= 0);
            Debug.Assert(Cooldown >= 0);

            XmlNode next = n.SelectSingleNode("CellMenu");
            if (next != null)
            {
                NextCell = new DescriptorCellMenuForConstruction(ForConstruction, next);
                Debug.Assert(Coord.Equals(NextCell.Coord), $"У {NameEntity} в ячейку {Coord} вложена ячейка {NextCell.Coord}.");
            }
        }

        public DescriptorCellMenuForConstruction(DescriptorConstruction forConstruction, Point coord, XmlNode n) : base(forConstruction, coord, n)
        {
            ForConstruction = forConstruction;

            Type = TypeCellMenuForConstruction.LevelUp;
            Income = GetInteger(n, "Income");

            Debug.Assert(Income >= 0);
        }

        internal DescriptorConstruction ForConstruction { get; }
        internal TypeCellMenuForConstruction Type { get; }
        internal string NameEntity { get; set; }
        internal DescriptorEntity Entity { get; private set; }
        internal int Income { get; }// Прибавление дохода
        internal int Cooldown { get; }
        internal DescriptorCellMenu NextCell { get; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            if ((NameEntity != null) && (Type != TypeCellMenuForConstruction.Extra))
            {
                switch (Type)
                {
                    case TypeCellMenuForConstruction.Tournament:
                    case TypeCellMenuForConstruction.Research:
                        Entity = FormMain.Config.FindAbility(NameEntity, false);
                        if (Entity is null)
                            Entity = FormMain.Config.FindItem(NameEntity, false);
                        if (Entity is null)
                            Entity = FormMain.Config.FindGroupItem(NameEntity, false);
                        break;
                    case TypeCellMenuForConstruction.Extension:
                        Entity = ForConstruction.FindExtension(NameEntity, true);
                        break;
                    case TypeCellMenuForConstruction.Event:
                        Entity = FormMain.Config.FindConstructionEvent(NameEntity, true);
                        break;
                    case TypeCellMenuForConstruction.HireCreature:
                        Entity = FormMain.Config.FindCreature(NameEntity);
                        break;
                    case TypeCellMenuForConstruction.Build:
                        Entity = FormMain.Config.FindConstruction(NameEntity, true);
                        break;
                    default:
                        throw new Exception($"Неизвестное действие: {Type}.");
                }

                Debug.Assert(Entity != null);
            }
        }
    }

    internal sealed class DescriptorCellMenuForConstructionLevel : DescriptorCellMenuForConstruction
    {
        public DescriptorCellMenuForConstructionLevel(DescriptorConstruction forConstruction, int number, Point coord, XmlNode n) : base(forConstruction, coord, n)
        {
            Number = number;
            Builders = GetIntegerNotNull(n, "Builders");
            MaxInhabitant = GetInteger(n, "MaxInhabitant");
            Capacity = GetInteger(n, "Capacity");
            GreatnessByConstruction = GetInteger(n, "GreatnessByConstruction");
            GreatnessPerDay = GetInteger(n, "GreatnessPerDay");
            BuildersPerDay = GetInteger(n, "BuildersPerDay");

            // Посещение
            XmlNode nv = n.SelectSingleNode("Visit");
            if (nv != null)
            {
                DescriptorVisit = new DescriptorConstructionVisit(this, nv);
            }

            Extensions = new ListSmallEntity(forConstruction, n.SelectSingleNode("Entities"));

            // Загружаем перки, которые дает сооружение
            ListPerks = new ListDescriptorPerks(n.SelectSingleNode("Perks"));

            Debug.Assert(Number >= 0);
            Debug.Assert(Number <= 5);
            Debug.Assert(MaxInhabitant >= 0);
            Debug.Assert(MaxInhabitant <= 100);
            Debug.Assert(Capacity >= 0);
            Debug.Assert(Capacity <= 100);
            Debug.Assert(GreatnessByConstruction >= 0);
            Debug.Assert(GreatnessPerDay >= 0);
            Debug.Assert(BuildersPerDay >= 0);
        }

        internal int Number { get; }
        internal int Builders { get; }
        internal int MaxInhabitant { get; }
        internal int Capacity { get; }
        internal int GreatnessByConstruction { get; }// Дает очков Величия при постройке
        internal int GreatnessPerDay { get; }// Дает очков Величия в день
        internal int BuildersPerDay { get; }// Дает строителей в день
        internal ListSmallEntity Extensions { get; }// Сущности, относящиеся к уровню
        internal DescriptorConstructionVisit DescriptorVisit { get;  }// Товар для посещения сооружения
        internal ListDescriptorPerks ListPerks { get; }// Перки, которые дает уровень сооружения

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Extensions.TuneDeferredLinks();
            ListPerks.TuneDeferredLinks();
        }
    }
}
