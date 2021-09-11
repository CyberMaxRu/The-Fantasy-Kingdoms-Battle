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

            Specialization = new Specialization(this, FormMain.Config.FindSpecialization("SpeedMove"));
            AddSecondarySkill(FormMain.Config.FindSecondarySkill("HealthSecSkill"));

            // Загружаем дефолтный инвентарь
            foreach (Item i in TypeCreature.Inventory)
            {
                Inventory.Add(new Item(i.Descriptor, i.Quantity));
            }

            // Берем оружие и доспехи
            if (TypeCreature.WeaponMelee != null)
            {
                MeleeWeapon = new Item(TypeCreature.WeaponMelee, 1);
                //MeleeWeapon.AddModificator(FormMain.Config.FindItem("EnchantWeaponAttack"));
                //MeleeWeapon.AddModificator(FormMain.Config.FindItem("EnchantWeaponPoison"));
            }
            if (TypeCreature.WeaponRange != null)
                RangeWeapon = new Item(TypeCreature.WeaponRange, 1);
            if (TypeCreature.Armour != null)
                Armour = new Item(TypeCreature.Armour, 1);
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
        internal Item MeleeWeapon { get; private set; }// Рукопашное оружие (ближнего боя)
        internal Item RangeWeapon { get; private set; }// Стрелковое оружие (дальнего боя)
        internal Item Armour { get; private set; }// Доспех        
        internal Item Quiver { get; private set; }// Колчан
        internal StateCreature StateCreature { get; private set; }// Состояние (на карте)
        internal bool IsLive { get; private set; } = true;// Существо живо

        // Повышение уровня
        private void LevelUp()
        {
            Debug.Assert(IsLive);
            Debug.Assert(Level < TypeCreature.MaxLevel);

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

            Level++;
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

            StateCreature = FormMain.Config.FindStateCreature(state.ToString());
        }

        internal int GetQuantityArrows()
        {
            return Quiver is null ? 0 : Quiver.Descriptor.QuantityShots;
        }

        internal void SetIsDead()
        {
            Debug.Assert(IsLive);

            IsLive = false;
        }

        internal override int GetLevel()
        {
            return Level;
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

        private void AddAbility(DescriptorAbility descriptor)
        {
            Debug.Assert(Abilities.Count < FormMain.Config.MaxCreatureAbilities);

            Abilities.Add(new Ability(this, descriptor));
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
            DescriptorAbility d1 = a1.Descriptor;
            DescriptorAbility d2 = a2.Descriptor;

            if (d2.TypeAbility == d2.TypeAbility)
            {
                if (d1.MinUnitLevel == d2.MinUnitLevel)
                {
                    Debug.Assert(a1.Pos != a2.Pos);
                    return a1.Pos > a2.Pos ? 1 : -1;
                }
                else
                {
                    return d1.MinUnitLevel > d2.MinUnitLevel ? 1 : -1;
                }
            }
            else
                return d1.TypeAbility > d2.TypeAbility ? 1 : -1;
        }

        protected void SortAbilities()
        {
            Abilities.Sort(CompareAbility);
        }
    }
}
