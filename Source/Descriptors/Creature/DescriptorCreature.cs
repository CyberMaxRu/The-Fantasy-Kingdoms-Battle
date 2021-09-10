using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal enum CategoryCreature { Citizen, Hero, Monster };    
    internal enum PriorityBuy { None = 0, Min = 100, Low = 200, Average = 300, High = 400, Max = 500 };// Приоритет обработки сооружений героями


    // Базовый тип существа
    internal sealed class DescriptorCreature : DescriptorEntity
    {
        private string nameMeleeWeapon;
        private string nameRangeWeapon;
        private string nameArmour;
        private string nameFromTypeHero = "";
        private string surnameFromTypeHero = "";

        public DescriptorCreature(XmlNode n) : base(n)
        {
            CategoryCreature = (CategoryCreature)Enum.Parse(typeof(CategoryCreature), n.SelectSingleNode("CategoryCreature").InnerText);
            KindCreature = FormMain.Config.FindKindCreature(XmlUtils.GetStringNotNull(n, "KindCreature"));
            MaxLevel = XmlUtils.GetInteger(n, "MaxLevel");
            DefaultPositionPriority = XmlUtils.GetInteger(n, "DefaultPositionPriority");
            if (CategoryCreature != CategoryCreature.Citizen)
            {
                TypeAttackMelee = FormMain.Config.FindTypeAttack(XmlUtils.GetString(n, "TypeAttackMelee"));
                Debug.Assert(TypeAttackMelee.KindAttack == KindAttack.Melee);
                string typeAttackRange = XmlUtils.GetString(n, "TypeAttackRange");
                if (typeAttackRange.Length > 0)
                {
                    TypeAttackRange = FormMain.Config.FindTypeAttack(typeAttackRange);
                    Debug.Assert(TypeAttackRange.KindAttack == KindAttack.Range);
                }
            }
            if (n.SelectSingleNode("PersistentState") != null)
                PersistentStateHeroAtMap = FormMain.Config.FindStateCreature(XmlUtils.GetStringNotNull(n, "PersistentState"));
            else
                PersistentStateHeroAtMap = FormMain.Config.FindStateCreature(NameStateCreature.Nothing.ToString());
            if (CategoryCreature == CategoryCreature.Hero)
            {
                Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
                Construction = FormMain.Config.FindConstruction(n.SelectSingleNode("Construction").InnerText);
                Construction.TrainedHero = this;
                CanBuild = Convert.ToBoolean(n.SelectSingleNode("CanBuild").InnerText);
                PrefixName = XmlUtils.GetString(n, "PrefixName");
                nameFromTypeHero = XmlUtils.GetString(n, "NameFromTypeHero");
                surnameFromTypeHero = XmlUtils.GetString(n, "SurnameFromTypeHero");
            }

            MaxConstructionForBuyPerDay = XmlUtils.GetInteger(n, "MaxConstructionForBuyPerDay");// NOT NULL

            //Debug.Assert(Cost > 0);


            Debug.Assert(MaxLevel >= 1);
            Debug.Assert(MaxLevel <= 100);
            Debug.Assert(DefaultPositionPriority >= 0);
            Debug.Assert(DefaultPositionPriority <= 1000);

            // Загружаем основные параметры
            if (n.SelectSingleNode("BaseParameters") != null)
            {
                ParametersByHire = new HeroParameters(n.SelectSingleNode("BaseParameters"));

                //
                if (n.SelectSingleNode("NextLevel") != null)
                    ConfigNextLevel = new ConfigNextLevelHero(n.SelectSingleNode("NextLevel"));
            }

            // Загружаем дефолтные способности
            XmlNode na = n.SelectSingleNode("Abilities");
            if (na != null)
            {
                DescriptorAbility a;

                foreach (XmlNode l in na.SelectNodes("Ability"))
                {
                    a = FormMain.Config.FindAbility(l.InnerText);

                    // Проверяем, что такая способность не повторяется
                    foreach (DescriptorAbility a2 in Abilities)
                    {
                        if (a.ID == a2.ID)
                            throw new Exception("Способность " + a.ID + " повторяется в списке способностей героя.");
                    }

                    Abilities.Add(a);
                }
            }

            // Загружаем дефолтный инвентарь
            XmlNode ni = n.SelectSingleNode("Inventory");
            if (ni != null)
            {
                DescriptorItem a;

                foreach (XmlNode l in ni.SelectNodes("Item"))
                {
                    a = FormMain.Config.FindItem(XmlUtils.GetStringNotNull(l, "ID"));

                    // Проверяем, что такая способность не повторяется
                    foreach (DescriptorAbility a2 in Abilities)
                    {
                        if (a.ID == a2.ID)
                            throw new Exception("Способность " + a.ID + " повторяется в списке способностей героя.");
                    }

                    Inventory.Add(new Item(a, XmlUtils.GetIntegerNotNull(l, "Quantity")));
                }
            }

            // Проверяем, что таких же ID и наименования нет
            foreach (DescriptorCreature h in FormMain.Config.Creatures)
            {
                if (h.ID == ID)
                {
                    throw new Exception("В конфигурации героев повторяется ID = " + ID);
                }

                if (h.Name == Name)
                {
                    throw new Exception("В конфигурации героев повторяется Name = " + Name);
                }

                if (h.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации героев повторяется ImageIndex = " + ImageIndex.ToString());
                }

                if (CategoryCreature == CategoryCreature.Hero)
                {
                    if (h.DefaultPositionPriority == DefaultPositionPriority)
                    {
                        throw new Exception("У героя " + h.Name + " уже указан приоритет " + DefaultPositionPriority.ToString());
                    }
                }
            }

            // Загружаем информацию о переносимых предметах
            XmlNode nc = n.SelectSingleNode("CarryItems");
            if (nc != null)
            {
                DescriptorItem item;
                int maxQuantity;

                foreach (XmlNode l in nc.SelectNodes("CarryItem"))
                {
                    item = FormMain.Config.FindItem(l.SelectSingleNode("Item").InnerText);
                    maxQuantity = Convert.ToInt32(l.SelectSingleNode("Item").Attributes[0]);

                    Debug.Assert(maxQuantity > 0);

                    // Проверка на уникальность обеспечена Dictionary?
                    //foreach (Item i in CarryItems)
                    //    if (i == item)
                    //        throw new Exception("Предмет " + item.ID + " уже ест в списке переносимых предметов.");

                    CarryItems.Add(item, maxQuantity);
                }
            }

            // Загружаем имена и фамилии
            if (CategoryCreature == CategoryCreature.Hero)
            {
                LoadName("Names", "Name", Names);
                LoadName("Surnames", "Surname", Surnames);

                if (Cost > 0)
                {
                    Debug.Assert(((Names.Count > 0) && (nameFromTypeHero.Length == 0)) || ((Names.Count == 0) && (nameFromTypeHero.Length > 0)));
                    Debug.Assert(((Surnames.Count > 0) && (surnameFromTypeHero.Length == 0)) || ((Surnames.Count == 0) && (surnameFromTypeHero.Length > 0)));
                }
            }

            // Загружаем дефолтное оружие и доспехи
            nameMeleeWeapon = XmlUtils.GetString(n, "MeleeWeapon");
            nameRangeWeapon = XmlUtils.GetString(n, "RangeWeapon");
            nameArmour = XmlUtils.GetString(n, "Armour");

            //Debug.Assert(nameMeleeWeapon != "");
            //Debug.Assert(nameArmour != "");

            if (nameRangeWeapon != null)
            {
                //Debug.Assert(QuantityArrows > 0);
            }
            else
            {
                //Debug.Assert(QuantityArrows == 0);
            }

            // Загружаем награду
            TypeReward = new DescriptorReward(n.SelectSingleNode("Reward"));

            // Загружаем приоритеты обхода сооружений для покупок
            XmlNode priorityConstr = n.SelectSingleNode("ConstructionsForShopping");
            if (priorityConstr != null)
            {
                foreach (XmlNode constr in priorityConstr.SelectNodes("Construction"))
                {
                    PriorityConstructionForShoppings.Add(new PriorityConstructionForShopping(constr, PriorityConstructionForShoppings));
                }
            }

            // Проверки корректности данных
            if (CategoryCreature == CategoryCreature.Hero)
            {
                Debug.Assert(MaxConstructionForBuyPerDay > 0);
            }
            else
            {
                Debug.Assert(MaxConstructionForBuyPerDay == 0);
                Debug.Assert(PriorityConstructionForShoppings.Count == 0);
            }

            void LoadName(string nodes, string node, List<string> list)
            {
                nc = n.SelectSingleNode(nodes);
                string name;
                if (nc != null)
                {
                    foreach (XmlNode l in nc.SelectNodes(node))
                    {
                        name = l.InnerText;

                        Debug.Assert(name != null);
                        Debug.Assert(name.Length > 1);
                        Debug.Assert(list.IndexOf(name) == -1);

                        list.Add(name);
                    }
                }
            }
        }

        internal CategoryCreature CategoryCreature { get; }// Категория существа
        internal TypeCreature KindCreature { get; }// Вид существа
        internal int MaxLevel { get; }// Максимальный уровень существа
        internal StateCreature PersistentStateHeroAtMap { get; set; }
        internal HeroParameters ParametersByHire { get; }// Параметры при создании существа
        internal ConfigNextLevelHero ConfigNextLevel { get; }
        internal List<DescriptorAbility> Abilities { get; } = new List<DescriptorAbility>();// Способности существа
        internal int DefaultPositionPriority { get; private set; }// Приоритет расположения на поле боя по умолчанию
        internal DescriptorAttack TypeAttackMelee { get;}// Тип рукопашной атаки
        internal DescriptorAttack TypeAttackRange { get; }// Тип дистанционной атаки
        internal DescriptorItem WeaponMelee { get; private set; }// Рукопашное оружие
        internal DescriptorItem WeaponRange { get; private set; }// Стрелковое оружие
        internal DescriptorItem Armour { get; private set; }// Доспех по умолчанию
        internal DescriptorReward TypeReward { get; }// Награда за убийство существа
        internal List<PriorityConstructionForShopping> PriorityConstructionForShoppings { get; } = new List<PriorityConstructionForShopping>();
        //internal (string, int)[] PriorityConstructionsForBuy;
        internal int Cost { get; }
        internal DescriptorConstruction Construction { get; }
        internal bool CanBuild { get; }
        internal Dictionary<DescriptorItem, int> CarryItems { get; } = new Dictionary<DescriptorItem, int>();
        internal List<Item> Inventory { get; } = new List<Item>();// Инвентарь
        internal int MaxConstructionForBuyPerDay { get; }// Максимальное количество посещений сооружений для покупок в день
        internal string PrefixName { get; }
        internal List<string> Names { get; } = new List<string>();
        internal List<string> Surnames { get; } = new List<string>();
        internal DescriptorCreature NameFromTypeHero { get; private set; }
        internal DescriptorCreature SurnameFromTypeHero { get; private set; }

        internal int MaxQuantityItem(DescriptorItem i)
        {
            return CarryItems.ContainsKey(i) ? CarryItems[i] : 0;
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            if (nameFromTypeHero.Length > 0)
                NameFromTypeHero = FormMain.Config.FindTypeCreature(nameFromTypeHero);
            if (surnameFromTypeHero.Length > 0)
                SurnameFromTypeHero = FormMain.Config.FindTypeCreature(surnameFromTypeHero);

            nameFromTypeHero = "";
            surnameFromTypeHero = "";

            /*foreach (Ability a in Abilities)
                if (a.ClassesHeroes.IndexOf(this) == -1)
                    throw new Exception("Класс героя " + ID + " отсутствует в списке доступных для способности " + a.ID);
            */

            foreach (DescriptorAbility a in Abilities)
            {
                Debug.Assert(a.ClassesHeroes.IndexOf(this) != -1, $"Типу существа {ID} не доступна стартовая способность {a.ID}.");
            }

            // Загружаем дефолтное оружие и доспехи
            if (nameMeleeWeapon.Length > 0)
            {
                WeaponMelee = FormMain.Config.FindItem(nameMeleeWeapon);
                nameMeleeWeapon = "";

                //Debug.Assert(WeaponMelee.ClassHero == this);
            }

            if (nameRangeWeapon.Length > 0)
            {
                WeaponRange = FormMain.Config.FindItem(nameRangeWeapon);
                nameRangeWeapon = "";

                //Debug.Assert(WeaponMelee.ClassHero == this);
            }

            if (nameArmour.Length > 0)
            {
                Armour = FormMain.Config.FindItem(nameArmour);
                nameArmour = "";

                //Debug.Assert(Armour.ClassHero == this);
            }

            foreach (PriorityConstructionForShopping pc in PriorityConstructionForShoppings)
            {
                pc.TuneDeferredLinks();
            }

            /*foreach (Ability a in Abilities)
                if (a.ClassesHeroes.IndexOf(this) == -1)
                    throw new Exception("Класс героя " + ID + " отсутствует в списке доступных для способности " + a.ID);
            */
        }
    }

    internal sealed class PriorityConstructionForShopping : Descriptor
    {
        private string nameConstruction;

        public PriorityConstructionForShopping(XmlNode n, List<PriorityConstructionForShopping> list) : base()
        {
            nameConstruction = GetStringNotNull(n, "ID");
            Priority = (PriorityBuy)Enum.Parse(typeof(PriorityBuy), n.SelectSingleNode("Priority").InnerText);

            foreach (PriorityConstructionForShopping pc in list)
            {
                Debug.Assert(pc.nameConstruction != nameConstruction);
            }
        }

        internal DescriptorConstruction Descriptor { get; private set; }
        internal PriorityBuy Priority { get; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Descriptor = Config.FindConstruction(nameConstruction);
            nameConstruction = "";
        }
    }
} 