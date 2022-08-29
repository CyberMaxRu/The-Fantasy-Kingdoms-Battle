using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal enum CategoryGroupItems { MeleeWeapons, RangeWeapons, Staffs, PlateArmors, Armors, Robes, Quivers };

    // Группа предметов
    internal sealed class DescriptorGroupItems : DescriptorSmallEntity
    {
        public DescriptorGroupItems(XmlNode n) : base(n)
        {
            CategoryGroupItems = (CategoryGroupItems)Enum.Parse(typeof(CategoryGroupItems), n.SelectSingleNode("CategoryGroupItems").InnerText);

            // Проверяем, что таких ID, Name и ImageIndex нет
            foreach (DescriptorGroupItems gi in Descriptors.GroupItems)
            {
                Debug.Assert(gi.ID != ID);
                Debug.Assert(gi.Name != Name);
                Debug.Assert(gi.ImageIndex != ImageIndex);
                //Debug.Assert(gi.Description != Description);
            }

            switch (CategoryGroupItems)
            {
                case CategoryGroupItems.MeleeWeapons:
                    ShortName = "Ближ.";
                    break;
                case CategoryGroupItems.RangeWeapons:
                    ShortName = "Даль.";
                    break;
                case CategoryGroupItems.Staffs:
                    ShortName = "Пос.";
                    break;
                case CategoryGroupItems.PlateArmors:
                    ShortName = "Латы";
                    break;
                case CategoryGroupItems.Armors:
                    ShortName = "Досп.";
                    break;
                case CategoryGroupItems.Robes:
                    ShortName = "Робы";
                    break;
                case CategoryGroupItems.Quivers:
                    ShortName = "Колч.";
                    break;
                default:
                    throw new Exception($"Неизвестная категория групп предметов {CategoryGroupItems}");
            }
        }

        internal string ShortName { get; }//
        internal CategoryGroupItems CategoryGroupItems { get; }
        internal List<DescriptorItem> Items { get; } = new List<DescriptorItem>();

        internal override string GetTypeEntity() => GetNameCategory();

        internal string GetNameCategory()
        {
            switch (CategoryGroupItems)
            {
                case CategoryGroupItems.MeleeWeapons:
                    return "Оружия ближнего боя";
                case CategoryGroupItems.RangeWeapons:
                    return "Оружия дальнего боя";
                case CategoryGroupItems.Staffs:
                    return "Посохи";
                case CategoryGroupItems.PlateArmors:
                    return "Латы";
                case CategoryGroupItems.Armors:
                    return "Доспехи";
                case CategoryGroupItems.Robes:
                    return "Робы";
                case CategoryGroupItems.Quivers:
                    return "Колчаны";
                default:
                    throw new Exception($"Неизвестная категория групп предметов {CategoryGroupItems}");
            }
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            // Проверяем, что списки доступных героев в списках группы и у предметов совпадают
            if (AvailableForAllHeroes)
            {
                foreach (DescriptorItem di in Items)
                {
                    Debug.Assert(di.AvailableForAllHeroes, $"У {ID} указано, что доступна всем героям, но у {di.ID} не указано, что доступно всем героям.");
                }
            }
            else
            {
                List<DescriptorCreature> creaturesInItems = new List<DescriptorCreature>();

                foreach (DescriptorItem di in Items)
                {
                    Debug.Assert(!di.AvailableForAllHeroes);

                    foreach (DescriptorCreature dc in di.AvailableForHeroes)
                    {
                        Debug.Assert(AvailableForHeroes.IndexOf(dc) >= 0, $"У {ID} герой {dc.ID} недоступен, но он доступен у {di.ID}.");

                        if (creaturesInItems.IndexOf(dc) == -1)
                            creaturesInItems.Add(dc);
                    }
                }

                foreach (DescriptorCreature dc in AvailableForHeroes)
                {
                    Debug.Assert(creaturesInItems.IndexOf(dc) >= 0, $"У {ID} герой {dc.ID} доступен, но он не найден в списке доступных героев у предметов группы.");
                }
            }
        }

        /*protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost() => ShortName;*/
    }
}