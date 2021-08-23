using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal enum CategoryItem { Potion, Enchant, Artifact, Elixir, Thing }

    // Класс предмета
    internal sealed class Item : Entity
    {
        public Item(XmlNode n) : base(n)
        {
            CategoryItem = (CategoryItem)Enum.Parse(typeof(CategoryItem), n.SelectSingleNode("CategoryItem").InnerText);
            TimeHit = n.SelectSingleNode("TimeHit") == null ? 0 : Convert.ToInt32(n.SelectSingleNode("TimeHit").InnerText);

            DamageMelee = n.SelectSingleNode("DamageMelee") != null ? Convert.ToInt32(n.SelectSingleNode("DamageMelee").InnerText) : 0;
            DamageArcher = n.SelectSingleNode("DamageArcher") != null ? Convert.ToInt32(n.SelectSingleNode("DamageArcher").InnerText) : 0;
            DamageMagic = n.SelectSingleNode("DamageMagic") != null ? Convert.ToInt32(n.SelectSingleNode("DamageMagic").InnerText) : 0;
            DefenseMelee = n.SelectSingleNode("DefenseMelee") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseMelee").InnerText) : 0;
            DefenseArcher = n.SelectSingleNode("DefenseArcher") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseArcher").InnerText) : 0;
            DefenseMagic = n.SelectSingleNode("DefenseMagic") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseMagic").InnerText) : 0;


            UsedByTypeCreature = new List<TypeCreature>();
            /*switch (TypeAttack)
            {
                case TypeAttack.None:
                    Debug.Assert((DamageMelee == 0) && (DamageArcher == 0) && (DamageMagic == 0));
                    Debug.Assert(TimeHit == 0);

                    break;
                case TypeAttack.Melee:
                    Debug.Assert(DamageMelee > 0);
                    Debug.Assert(TimeHit > 0);
                    Debug.Assert(TimeHit * 10 % Config.STEP_IN_MSEC == 0);

                    break;
                case TypeAttack.Archer:
                    Debug.Assert((DamageArcher > 0) || (DamageMagic > 0));
                    Debug.Assert(TimeHit > 0);
                    Debug.Assert(TimeHit * 10 % Config.STEP_IN_MSEC == 0);

                    break;
                default:
                    throw new Exception("Неизвестный тип атаки.");
            }*/

            Position = FormMain.Config.Items.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Item i in FormMain.Config.Items)
            {
                if (i.ID == ID)
                    throw new Exception("В конфигурации предметов повторяется ID = " + ID);

                if (i.Name == Name)
                    throw new Exception("В конфигурации предметов повторяется Name = " + Name);

                if (i.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации предметов повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal CategoryItem CategoryItem { get; }
        internal List<TypeCreature> UsedByTypeCreature { get; }
        internal int TimeHit { get; }
        internal int Position { get; }
        internal int DamageMelee { get; }
        internal int DamageArcher { get; }
        internal int DamageMagic { get; }
        internal int DefenseMelee { get; }
        internal int DefenseArcher { get; }
        internal int DefenseMagic { get; }

        protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost() => null;

        protected override void DoPrepareHint()
        {
            base.DoPrepareHint();
        }
    }
}