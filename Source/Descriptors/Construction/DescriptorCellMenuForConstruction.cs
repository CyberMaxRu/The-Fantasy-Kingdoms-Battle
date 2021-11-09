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

            XmlNode nodeProduct = n.SelectSingleNode("Product");
            if (nodeProduct != null)
            {
                Product = new DescriptorProduct(nodeProduct);
            }

            XmlNode next = n.SelectSingleNode("CellMenu");
            if (next != null)
            {
                NextCell = new DescriptorCellMenuForConstruction(ForConstruction, next);
                Debug.Assert(Coord.Equals(NextCell.Coord), $"У {NameEntity} в ячейку {Coord} вложена ячейка {NextCell.Coord}.");
            }

            Debug.Assert(DaysProcessing >= 0, $"В {forConstruction.ID} отрицательное число дней процесса у {NameEntity}");

            if ((Type != TypeCellMenuForConstruction.HireCreature) && (Type != TypeCellMenuForConstruction.Extra) && (Type != TypeCellMenuForConstruction.Build))
            {
                Debug.Assert(DaysProcessing > 0, $"В {forConstruction.ID} не указано дней процесса у {NameEntity}");
                Debug.Assert(DaysProcessing <= 50);
            }
            else
            {
                if (Type != TypeCellMenuForConstruction.HireCreature)
                {
                    Debug.Assert(DaysProcessing == 0, $"В {forConstruction.ID} указано дней процесса у {NameEntity}");
                }
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
        internal DescriptorProduct Product { get; set; }
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
                        Entity = ForConstruction.FindConstructionEvent(NameEntity, true);
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

            Product?.TuneDeferredLinks();
        }
    }

    internal sealed class DescriptorCellMenuForConstructionLevel : DescriptorCellMenuForConstruction
    {
        public DescriptorCellMenuForConstructionLevel(DescriptorConstruction forConstruction, int number, Point coord, XmlNode n) : base(forConstruction, coord, n)
        {
            Number = number;
            Builders = GetInteger(n, "Builders");
            MaxInhabitant = GetInteger(n, "MaxInhabitant");
            Capacity = GetInteger(n, "Capacity");
            GreatnessByConstruction = GetInteger(n, "GreatnessByConstruction");
            GreatnessPerDay = GetInteger(n, "GreatnessPerDay");
            BuildersPerDay = GetInteger(n, "BuildersPerDay");

            // Посещение
            XmlNode nv = n.SelectSingleNode("Visit");
            if (nv != null)
            {
                DescriptorVisit = new DescriptorVisitToConstruction(this, nv);
            }

            Extensions = new ListSmallEntity(forConstruction, n.SelectSingleNode("Entities"));

            // Загружаем перки, которые дает сооружение
            ListPerks = new ListDescriptorPerks(n.SelectSingleNode("Perks"));

            if ((forConstruction.Category == CategoryConstruction.Lair) || (forConstruction.Category == CategoryConstruction.Place))
            {
                //Debug.Assert(DaysBuilding == 0);
                //Debug.Assert(Builders == 0);
            }
            else
            {
                //Debug.Assert(DaysBuilding >= 1, $"У {forConstruction.ID}.{number} DaysBuilding = 0.");
                //Debug.Assert(Builders >= 0);
            }

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
        internal int Builders { get; }// Количество требуемых строителей
        internal int MaxInhabitant { get; }
        internal int Capacity { get; }
        internal int GreatnessByConstruction { get; }// Дает очков Величия при постройке
        internal int GreatnessPerDay { get; }// Дает очков Величия в день
        internal int BuildersPerDay { get; }// Дает строителей в день
        internal ListSmallEntity Extensions { get; }// Сущности, относящиеся к уровню
        internal DescriptorVisitToConstruction DescriptorVisit { get; }// Товар для посещения сооружения
        internal ListDescriptorPerks ListPerks { get; }// Перки, которые дает уровень сооружения

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Extensions.TuneDeferredLinks();
            ListPerks.TuneDeferredLinks();
        }
    }
}
