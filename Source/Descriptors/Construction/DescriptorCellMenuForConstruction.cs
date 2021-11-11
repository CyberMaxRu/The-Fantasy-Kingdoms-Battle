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
    internal enum TypeCellMenuForConstruction { Research, Event, HireCreature, Build, LevelUp, Action, Extension, Tournament, Improvement, Extra };

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
                Product = new DescriptorProduct(forConstruction, nodeProduct);
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

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if ((NameEntity != null) && (Type != TypeCellMenuForConstruction.Extra))
            {
                switch (Type)
                {
                    case TypeCellMenuForConstruction.Tournament:
                    case TypeCellMenuForConstruction.Research:
                    case TypeCellMenuForConstruction.Event:
                        Entity = ForConstruction.FindProduct(NameEntity, true);
                        break;
                    case TypeCellMenuForConstruction.Extension:
                        Entity = ForConstruction.FindExtension(NameEntity, true);
                        break;
                    case TypeCellMenuForConstruction.Improvement:
                        Entity = ForConstruction.FindConstructionImprovement(NameEntity, true);
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

                Debug.Assert(Entity != null, $"В {ForConstruction.ID} не найдено {NameEntity}");
            }

            Product?.TuneLinks();
        }
    }
}
