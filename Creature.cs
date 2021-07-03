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
    internal abstract class Creature : PlayerObject, ICell
    {
        private VCCell panelEntity;
        private static int sequenceID = 0;// Генератор уникального кода героя

        public Creature(TypeCreature tc, BattleParticipant bp)
        {
            TypeCreature = tc;
            BattleParticipant = bp;
            ID = ++sequenceID;

            StateCreature = TypeCreature.PersistentStateHeroAtMap;

            // Применяем дефолтные способности
            Abilities.AddRange(TypeCreature.Abilities);
            Specialization = FormMain.Config.FindSpecialization("SpeedMove");
            SecondarySkills.Add(FormMain.Config.FindSecondarySkill("Health"));

            // Берем оружие и доспехи
            MeleeWeapon = TypeCreature.WeaponMelee;
            RangeWeapon = TypeCreature.WeaponRange;
            Armour = TypeCreature.Armour;

            if (TypeCreature.MaxLevel > 1)
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
        internal int ID { get; }// Уникальный код существа
        internal TypeCreature TypeCreature { get; }
        internal BattleParticipant BattleParticipant { get; }
        internal int Level { get; private set; }// Уровень существа
        internal HeroParameters ParametersBase { get; }// Свои параметры, без учета амуниции
        internal HeroParameters ParametersWithAmmunition { get; }// Параметры с учетом амуниции
        internal Specialization Specialization { get; }// Специализация
        internal List<SecondarySkill> SecondarySkills { get; } = new List<SecondarySkill>();
        internal List<PlayerItem> Inventory { get; } = new List<PlayerItem>();
        internal List<Ability> Abilities { get; } = new List<Ability>();// Cпособности
        internal Weapon MeleeWeapon { get; private set; }// Рукопашное оружие 
        internal Weapon RangeWeapon { get; private set; }// Стрелковое оружие 
        internal Armour Armour { get; private set; }// Доспех        
        internal StateCreature StateCreature { get; private set; }// Состояние (на карте)

        internal Point CoordInPlayer { get; set; }// Координаты героя в слотах
        internal bool IsLive { get; private set; } = true;// Существо живо

        protected abstract int GetImageIndex();

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

        internal void SetIsDead()
        {
            Debug.Assert(IsLive);

            IsLive = false;
        }

        // Реализация интерфейса
        BitmapList ICell.BitmapList() => Program.formMain.imListObjectsCell;
        int ICell.ImageIndex() => GetImageIndex();
        bool ICell.NormalImage() => true;
        int ICell.Level() => Level;
        int ICell.Quantity() => 0;
        void ICell.PrepareHint()
        {
            PrepareHint();
        }

        void ICell.Click(VCCell pe)
        {
            Debug.Assert(IsLive);

            Program.formMain.SelectPlayerObject(this);
        }

        void ICell.CustomDraw(Graphics g, int x, int y, bool drawState)
        {
            DoCustomDraw(g, x, y, drawState);
        }
    }
}
