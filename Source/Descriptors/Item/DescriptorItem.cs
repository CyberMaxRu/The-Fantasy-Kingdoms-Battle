using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal enum CategoryItem { Potion, Enchant, Artifact, Elixir, Food, Thing, Weapon, Armour, Quiver, Monster, Game, Tournament, Resource }

    // Класс предмета
    internal sealed class DescriptorItem : DescriptorSmallEntity
    {
        private string nameGroupItem;

        public DescriptorItem(XmlNode n) : base(n)
        {
            CategoryItem = (CategoryItem)Enum.Parse(typeof(CategoryItem), n.SelectSingleNode("CategoryItem").InnerText);
            nameGroupItem = XmlUtils.GetString(n, "GroupItem");
            Signer = XmlUtils.GetString(n, "Signer");

            TimeHit = n.SelectSingleNode("TimeHit") == null ? 0 : Convert.ToInt32(n.SelectSingleNode("TimeHit").InnerText);
            if (n.SelectSingleNode("VelocityMissile") != null)
                VelocityMissile = Convert.ToDouble(n.SelectSingleNode("VelocityMissile").InnerText.Replace(".", ","));

            DamageMelee = n.SelectSingleNode("DamageMelee") != null ? Convert.ToInt32(n.SelectSingleNode("DamageMelee").InnerText) : 0;
            DamageRange = n.SelectSingleNode("DamageRange") != null ? Convert.ToInt32(n.SelectSingleNode("DamageRange").InnerText) : 0;
            DamageMagic = n.SelectSingleNode("DamageMagic") != null ? Convert.ToInt32(n.SelectSingleNode("DamageMagic").InnerText) : 0;
            DefenseMelee = n.SelectSingleNode("DefenseMelee") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseMelee").InnerText) : 0;
            DefenseRange = n.SelectSingleNode("DefenseRange") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseRange").InnerText) : 0;
            DefenseMagic = n.SelectSingleNode("DefenseMagic") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseMagic").InnerText) : 0;
            QuantityShots = XmlUtils.GetInteger(n, "QuantityShots");
            TimeOfAction = XmlUtils.GetInteger(n, "TimeOfAction");

            if (CategoryItem == CategoryItem.Quiver)
            {
                Debug.Assert(QuantityShots > 0);
                Debug.Assert(QuantityShots <= 100);
            }
            else
            {
                Debug.Assert(QuantityShots == 0);
            }

            if (CategoryItem == CategoryItem.Weapon)
            {
                //Debug.Assert(DamageRange == 0);
            }

            if (CategoryItem == CategoryItem.Elixir)
            {
                Debug.Assert(TimeOfAction > 0);
                Debug.Assert(TimeOfAction <= 60);
            }
            else
            {
                Debug.Assert(TimeOfAction == 0);
            }

            if (CategoryItem == CategoryItem.Food)
            {
                //Debug.Assert(Food > 0);
                //Debug.Assert(Food <= 100);
            }
            else
            {
                //Debug.Assert(Food == 0);
            }

            Perks = new ListDescriptorPerks(n.SelectSingleNode("Perks"));
            ListNeeds = new ListNeeds(n.SelectSingleNode("Needs"));

            /*if (CategoryItem == CategoryItem.RangeWeapon)
            {
                Debug.Assert(DamageMelee == 0);
            }*/

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

            Position = Config.Items.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (DescriptorItem i in Config.Items)
            {
                if (i.ID == ID)
                    throw new Exception("В конфигурации предметов повторяется ID = " + ID);

                //if (i.Name == Name)
                //    throw new Exception("В конфигурации предметов повторяется Name = " + Name);

                //if (i.ImageIndex == ImageIndex)
                //    throw new Exception("В конфигурации предметов повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal CategoryItem CategoryItem { get; }
        internal DescriptorGroupItems GroupItem { get; private set; }
        internal ListDescriptorPerks Perks { get; }// Перки, даваемые предметом
        internal string Signer { get; }//
        internal int Position { get; }
        internal int TimeHit { get; }
        internal double VelocityMissile { get; }
        internal int DamageMelee { get; }
        internal int DamageRange { get; }
        internal int DamageMagic { get; }
        internal int DefenseMelee { get; }
        internal int DefenseRange { get; }
        internal int DefenseMagic { get; }
        internal int QuantityShots { get; }
        internal int Distance { get; }
        internal int TimeOfAction { get; }// Время действия (эликсира)
        internal ListNeeds ListNeeds { get; }

        /*protected override void DoPrepareHint()
        {
            base.DoPrepareHint();

            //Program.formMain.formHint.AddStep7Weapon(this);
            //Program.formMain.formHint.AddStep8Armour(this);
            //Program.formMain.formHint.AddStep1Header(GroupQuiver.Name, "", GroupQuiver.Description
            //    + Environment.NewLine + Environment.NewLine + "Боезапас: " + QuantityShots.ToString());

        }*/

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            if (nameGroupItem.Length > 0)
            {
                GroupItem = Config.FindGroupItem(nameGroupItem);
                GroupItem.Items.Add(this);
                nameGroupItem = "";
            }

            Perks.TuneDeferredLinks();
            ListNeeds.TuneDeferredLinks();
        }

        internal override void AfterTune()
        {
            base.AfterTune();

            if (UseForResearch.Count > 0)
            {
                Description += Environment.NewLine + "- Необходимо для:";

                foreach (DescriptorCellMenu cm in UseForResearch)
                {
                    if (cm is DescriptorCellMenuForConstructionLevel cmcl)
                        Description += Environment.NewLine + "    - { " + cmcl.ForEntity.Name + " (" + cmcl.Number.ToString() + " ур.)}";
                    else if (cm is DescriptorCellMenuForConstruction cmc)
                        Description += Environment.NewLine + "    - {" + cmc.Entity.Name + "} ({" + cm.ForEntity.Name + "})";
                }
            }
        }

        protected override bool ForHeroes() => CategoryItem != CategoryItem.Monster;
    }
}