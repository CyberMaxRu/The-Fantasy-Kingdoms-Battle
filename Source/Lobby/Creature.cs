using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс существа
    internal abstract class Creature : BigEntity
    {
        private static int sequenceID = 0;// Генератор уникального кода героя

        public Creature(DescriptorCreature tc, BattleParticipant bp)
        {
            TypeCreature = tc;
            BattleParticipant = bp;
            IDCreature = ++sequenceID;

            StateCreature = TypeCreature.PersistentStateHeroAtMap;

            // Применяем дефолтные способности
            foreach (DescriptorAbility ta in TypeCreature.Abilities)
                AddAbility(ta);
            SortAbilities();

            Specialization = new Specialization(this, FormMain.Descriptors.FindSpecialization("SpeedMove"));
            AddSecondarySkill(FormMain.Descriptors.FindSecondarySkill("HealthSecSkill"));

            // Загружаем дефолтный инвентарь
            foreach ((DescriptorItem, int) inv in TypeCreature.Inventory)
            {
                AddItemToInventory(CreateItem(inv.Item1, inv.Item2));
            }

            // Создаем свойства
            Properties = new CreatureProperty[FormMain.Descriptors.PropertiesCreature.Count];
            
            if (TypeCreature.Properties != null)
            {
                foreach (DescriptorCreatureProperty dcp in TypeCreature.Properties)
                {
                    Properties[dcp.Descriptor.Index] = new CreatureProperty(this, dcp.Descriptor);
                }
            }

            // Создаем потребности
            Needs = new CreatureNeed[FormMain.Descriptors.NeedsCreature.Count];

            foreach (DescriptorCreatureNeed dcn in TypeCreature.Needs)
            {
                Needs[dcn.Descriptor.Index] = new CreatureNeed(this, dcn);
            }

            // Создаем интересы
            Interests = new CreatureInterest[FormMain.Descriptors.InterestCreature.Count];

            foreach (DescriptorCreatureInterest dci in TypeCreature.Interests)
            {
                Interests[dci.Descriptor.Index] = new CreatureInterest(this, dci);
            }

            // Создаем перк существа по его характеристикам
            if (TypeCreature.Properties != null)
            {
                MainPerk = new Perk(this, TypeCreature.Properties);
                Perks.Add(MainPerk);
            }

            // Берем дефолтные перки
            foreach (DescriptorPerk dp in TypeCreature.Perks)
            {
                AddPerk(dp, this);
            }

            // Берем оружие и доспехи
            if (TypeCreature.WeaponMelee != null)
            {
                MeleeWeapon = new Item(this, TypeCreature.WeaponMelee, 1);
                //MeleeWeapon.AddModificator(FormMain.Config.FindItem("EnchantWeaponAttack"));
                //MeleeWeapon.AddModificator(FormMain.Config.FindItem("EnchantWeaponPoison"));
            }
            if (TypeCreature.WeaponRange != null)
                RangeWeapon = new Item(this, TypeCreature.WeaponRange, 1);
            if (TypeCreature.Armour != null)
                Armour = new Item(this, TypeCreature.Armour, 1);
            FindQuiver();
            
            if (TypeCreature.CategoryCreature != CategoryCreature.Citizen)
            {
                Level = 0;

                ParametersBase = new HeroParameters(TypeCreature.ParametersByHire);

                // Переходим на 1 уровень
                LevelUp();
                ParametersWithAmmunition = new HeroParameters(ParametersBase);

                //
                UpdateBaseParameters();
            }
            else
                Level = 1;

            //
            Initialize();
        }
        internal int IDCreature { get; }// Уникальный код существа
        internal DescriptorCreature TypeCreature { get; }
        internal BattleParticipant BattleParticipant { get; }
        internal int Level { get; private set; }// Уровень существа
        internal HeroParameters ParametersBase { get; }// Свои параметры, без учета амуниции
        internal HeroParameters ParametersWithAmmunition { get; }// Параметры с учетом амуниции
        internal Specialization Specialization { get; }// Специализация
        internal List<SecondarySkill> SecondarySkills { get; } = new List<SecondarySkill>();
        internal List<Item> Inventory { get; } = new List<Item>();
        internal List<Ability> Abilities { get; } = new List<Ability>();// Cпособности
        internal Perk MainPerk { get; }// Основной перк существа 
        internal List<Perk> Perks { get; } = new List<Perk>();// Перки
        internal Item MeleeWeapon { get; private set; }// Рукопашное оружие (ближнего боя)
        internal Item RangeWeapon { get; private set; }// Стрелковое оружие (дальнего боя)
        internal Item Armour { get; private set; }// Доспех        
        internal Item Quiver { get; private set; }// Колчан
        internal DescriptorStateCreature StateCreature { get; private set; }// Состояние (на карте)

        // Индивидуальные свойства существа
        internal CreatureProperty[] Properties { get; }// Характеристики        
        internal CreatureNeed[] Needs { get; }// Потребности
        internal CreatureInterest[] Interests { get; }// Интересы
        //
        internal bool IsLive { get; private set; } = true;// Существо живо
        internal int DayOfDeath { get; private set; }// День смерти
        internal DescriptorReasonOfDeath ReasonOfDeath { get; private set; }// Причина смерти

        // Повышение уровня
        private void LevelUp()
        {
            Debug.Assert(IsLive);
            Debug.Assert(Level < TypeCreature.MaxLevel);

            //
            Level++;

            if (Level > 1)
            {
                // Прибавляем безусловные параметры
                if (TypeCreature.ConfigNextLevel != null)
                {
                    ParametersBase.Health += TypeCreature.ConfigNextLevel.Health;
                    ParametersBase.Mana += TypeCreature.ConfigNextLevel.Mana;
                    ParametersBase.Stamina += TypeCreature.ConfigNextLevel.Stamina;

                    // Прибавляем очки характеристик
                    int t;
                    for (int i = 0; i < TypeCreature.ConfigNextLevel.StatPoints; i++)
                    {
                        t = BattleParticipant.Lobby.Rnd.Next(100);

                        if (t < TypeCreature.ConfigNextLevel.WeightStrength)
                            ParametersBase.Strength++;
                        else if (t < TypeCreature.ConfigNextLevel.WeightStrength + TypeCreature.ConfigNextLevel.WeightDexterity)
                            ParametersBase.Dexterity++;
                        else if (t < TypeCreature.ConfigNextLevel.WeightStrength + TypeCreature.ConfigNextLevel.WeightDexterity + TypeCreature.ConfigNextLevel.WeightMagic)
                            ParametersBase.Magic++;
                        else
                            ParametersBase.Vitality++;
                    }
                }

                // Делаем изменение характеристик
                foreach (DescriptorCreatureProperty cp in TypeCreature.Properties.Where(d => d.ChangePerLevel != 0))
                {
                    MainPerk.ListProperty[cp.Descriptor.Index] = ValidateValueProperty(MainPerk.ListProperty[cp.Descriptor.Index], cp.ChangePerLevel);
                }

                // Делаем изменение изменения потребностей
                foreach (DescriptorCreatureNeed dcn in TypeCreature.Needs.Where(d => d.ChangePerLevel != 0))
                {
                    Needs[dcn.Descriptor.Index].IncreasePerDay = ValidateValueProperty(Needs[dcn.Descriptor.Index].IncreasePerDay, dcn.ChangePerLevel);
                    Utils.Assert(Needs[dcn.Descriptor.Index].IncreasePerDay > 0);
                }

                // Делаем изменение интересов
                foreach (DescriptorCreatureInterest dci in TypeCreature.Interests.Where(d => d.ChangePerLevel != 0))
                {
                    Interests[dci.Descriptor.Index].Value = ValidateValueProperty(Interests[dci.Descriptor.Index].Value, dci.ChangePerLevel);
                }
            }
        }

        private int ValidateValueProperty(int val, int change)
        {
            val += change;
            if (val > FormMain.Config.MaxValueProperty)
                val = FormMain.Config.MaxValueProperty;
            if (val < -FormMain.Config.MaxValueProperty)
                val = -FormMain.Config.MaxValueProperty;

            return val;
        }

        internal void UpdateBaseParameters()
        {
            ParametersBase.Health = (ParametersBase.Vitality * ParametersBase.CoefHealth) + (Level * TypeCreature.ConfigNextLevel.Health);
            ParametersBase.Mana = (ParametersBase.Magic * ParametersBase.CoefMana) + (Level * TypeCreature.ConfigNextLevel.Mana);
            ParametersBase.Stamina = (ParametersBase.Vitality * ParametersBase.CoefStamina) + (Level * TypeCreature.ConfigNextLevel.Stamina);

            UpdateParamsWithAmmunition();
        }

        protected virtual void UpdateParamsWithAmmunition()
        {
            // Копируем базовые параметры
            ParametersWithAmmunition.GetFromParams(ParametersBase);
        }

        internal int Priority()
        {
            Debug.Assert(IsLive);

            int posInPlayer = BattleParticipant.CombatHeroes.IndexOf(this);
            Debug.Assert(posInPlayer != -1);
            return TypeCreature.DefaultPositionPriority * 1000 + posInPlayer;
        }

        protected virtual void DoCustomDraw(Graphics g, int x, int y, bool drawState)
        {

        }

        internal void SetState(NameStateCreature state)
        {
            Debug.Assert(IsLive);

            StateCreature = FormMain.Descriptors.FindStateCreature(state.ToString());
        }

        internal int GetQuantityArrows()
        {
            return Quiver is null ? 0 : Quiver.Descriptor.QuantityShots;
        }

        internal virtual void SetIsDead(DescriptorReasonOfDeath reason)
        {
            Debug.Assert(IsLive);
            Debug.Assert(DayOfDeath == 0);
            Debug.Assert(ReasonOfDeath is null);

            IsLive = false;
            DayOfDeath = BattleParticipant.Lobby.Turn;
            ReasonOfDeath = reason;
        }

        internal override string GetLevel()
        {
            return Level.ToString();
        }

        internal override int GetQuantity()
        {
            return 0;
        }

        internal override int GetImageIndex()
        {
            return TypeCreature.ImageIndex;
        }

        private void FindQuiver()
        {
            Quiver = null;

            foreach (Item i in Inventory)
            {
                if (i.Descriptor.CategoryItem == CategoryItem.Quiver)
                {
                    Debug.Assert(Quiver == null);// Не может быть 2 колчана
                    Quiver = i;
                }
            }

            Debug.Assert(((RangeWeapon != null) && (Quiver != null)) || ((RangeWeapon is null) && (Quiver is null)));
        }

        // Реализация интерфейса
        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override void Click(VCCell pe)
        {
            Debug.Assert(IsLive);

            Program.formMain.SelectPlayerObject(this);
        }

        internal override void CustomDraw(Graphics g, int x, int y, bool drawState)
        {
            DoCustomDraw(g, x, y, drawState);
        }

        protected void AddAbility(DescriptorAbility descriptor)
        {
            Debug.Assert(Abilities.Count < FormMain.Config.MaxCreatureAbilities);

            Abilities.Add(new Ability(this, descriptor));
        }

        protected void AddAbility(Ability ability)
        {
            Debug.Assert(Abilities.Count < FormMain.Config.MaxCreatureAbilities);

            Abilities.Add(ability);
        }

        private void AddSecondarySkill(DescriptorSecondarySkill descriptor)
        {
            Debug.Assert(SecondarySkills.Count < FormMain.Config.MaxCreatureSecondarySkills);

            SecondarySkills.Add(new SecondarySkill(this, descriptor));
        }
        internal bool AbilityExists(DescriptorAbility da)
        {
            foreach (Ability a in Abilities)
            {
                if (a.Descriptor == da)
                    return true;
            }

            return false;
        }

        private static int CompareAbility(Ability a1, Ability a2)
        {
            Debug.Assert(a1.Pos != a2.Pos);

            DescriptorAbility d1 = a1.Descriptor;
            DescriptorAbility d2 = a2.Descriptor;

            if (d1.TypeAbility.ID == d2.TypeAbility.ID)
            {
                if (d1.MinUnitLevel == d2.MinUnitLevel)
                    return a1.Pos > a2.Pos ? 1 : -1;
                else
                    return d1.MinUnitLevel > d2.MinUnitLevel ? 1 : -1;
            }
            else
                return d1.TypeAbility.Pos > d2.TypeAbility.Pos ? 1: -1;
        }

        protected void SortAbilities()
        {
            Abilities.Sort(CompareAbility);
        }

        internal Item CreateItem(DescriptorItem di, int quantity)
        {
            Debug.Assert(quantity > 0);

            Hero signer = null;
            if (di.Signer.Length > 0)
            {
                if (di.Signer == "King")
                {
                    if (BattleParticipant is Player p)
                    {
                        foreach (Hero c in p.AllHeroes)
                        {
                            if (c.TypeCreature.ID == "King")
                            {
                                signer = c;
                                break;
                            }
                        }

                        Debug.Assert(signer != null);
                    }
                    else
                        throw new Exception("Не игрок.");
                }
            }

            return new Item(this, di, quantity, signer);
        }

        internal void AddItemToInventory(Item i)
        {
            Debug.Assert(i.Owner == this);

            Inventory.Add(i);

            // Добавляем перки
            if (i.Descriptor.Perks != null)
            {
                foreach (DescriptorPerk dp in i.Descriptor.Perks)
                {
                    AddPerk(dp, i);
                }
            }
        }

        internal void AddPerk(DescriptorPerk dp, Entity fromEntity)
        {
            foreach (Perk p in Perks)
            {
                if (p.Descriptor != null)
                    Debug.Assert(p.Descriptor.ID != dp.ID);
            }

            Perks.Add(new Perk(this, dp, fromEntity, -1));
        }

        internal void RemovePerk(DescriptorPerk dp)
        {
            Perk removedPerk = null;
            foreach (Perk p in Perks)
            {
                if (p.Descriptor.ID == dp.ID)
                {
                    removedPerk = p;
                    break;
                }
            }

            if (removedPerk != null)
                Perks.Remove(removedPerk);
        }

        private void CalcProperties()
        {
            for (int i = 0; i < Properties.Length; i++)
                if (Properties[i] != null)
                    CalcProperty(Properties[i]);
        }

        internal virtual void PerksChanged()
        {
            CalcProperties();
        }

        internal override void MakeMenu(VCMenuCell[,] menu)
        {

        }

        private void CalcProperty(CreatureProperty cp)
        {
            cp.ListSource.Clear();
            cp.Value = 0;
            int value;

            foreach (Perk p in Perks)
            {
                value = p.ListProperty[cp.Property.Index];
                if (value != 0)
                {
                    cp.ListSource.Add(p);
                    cp.Value += value;
                }
            }

            if (cp.Value > FormMain.Config.MaxValueProperty)
                cp.Value = FormMain.Config.MaxValueProperty;
            else if (cp.Value < -FormMain.Config.MaxValueProperty)
                cp.Value = -FormMain.Config.MaxValueProperty;
        }

        internal virtual void PrepareTurn()
        {
            Initialize();

            // Расчет потребностей
            if (BattleParticipant.Lobby.Turn > 0)
            {
                foreach (CreatureNeed cn in Needs)
                {
                    if (cn != null)
                    {
                        cn.Value += cn.IncreasePerDay - cn.Satisfacted;
                        if (cn.Value < 0)
                            cn.Value = 0;
                        else if (cn.Value >= 10)
                        {
                            cn.DaysMax++;
                            if (cn.DaysMax > 3)
                            {
                                SetIsDead(cn.Need.Descriptor.ReasonOfDeath);
                                break;
                            }
                        }
                    }
                }
            }
        }

        internal virtual void Initialize()
        {

            // 
            PerksChanged();
        }
    }
}
