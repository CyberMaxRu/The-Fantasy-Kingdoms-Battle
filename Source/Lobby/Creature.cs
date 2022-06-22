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
    internal class Creature : BigEntity
    {
        public Creature(Construction pb, DescriptorCreature tc, BattleParticipant bp, Player p, int level) : base(tc, bp.Lobby, p)
        {
            Debug.Assert(tc != null);
            Debug.Assert(level > 0);
            Construction = pb;
            Abode = Construction;
            TurnOfTrain = Player.Lobby.Turn;

            TypeCreature = tc;
            BattleParticipant = bp;

            StateCreature = TypeCreature.PersistentStateHeroAtMap;

            if (TypeCreature.CategoryCreature == CategoryCreature.Hero)
            {
                FullName = (TypeCreature.PrefixName.Length > 0 ? TypeCreature.PrefixName + " " : "")
                    + GetRandomName(TypeCreature.NameFromTypeHero == null ? TypeCreature.Names : TypeCreature.NameFromTypeHero.Names, false)
                    + GetRandomName(TypeCreature.SurnameFromTypeHero == null ? TypeCreature.Surnames : TypeCreature.SurnameFromTypeHero.Surnames, true);
            }
            else
            {
                FullName = TypeCreature.Name;
            }

            string GetRandomName(List<string> list, bool isSurname)
            {
                string s = list.Count > 0 ? list[Player.Lobby.Rnd.Next(list.Count)] : "";
                if (isSurname && (s.Length > 0))
                    s = (s[0] != ',' ? " " : "") + s;
                return s;
            }


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
            Properties = new EntityProperties(this, TypeCreature.Properties);

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
            if (TypeCreature.DefaultWeaponMelee != null)
            {
                MeleeWeapon = new Item(this, TypeCreature.DefaultWeaponMelee, 1);
                //MeleeWeapon.AddModificator(FormMain.Config.FindItem("EnchantWeaponAttack"));
                //MeleeWeapon.AddModificator(FormMain.Config.FindItem("EnchantWeaponPoison"));
            }
            if (TypeCreature.DefaultWeaponRanged != null)
                RangeWeapon = new Item(this, TypeCreature.DefaultWeaponRanged, 1);
            if (TypeCreature.DefaultArmour != null)
                Armour = new Item(this, TypeCreature.DefaultArmour, 1);
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

        internal int Number { get; }
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
        internal DescriptorStateCreature StateCreature { get; private set; }// Состояние (на карте)


        // From Hero
        internal Construction Construction { get; }// Здание, которому принадлежит существо
        internal Construction Abode { get; private set; }// Текущая обитель существа
        internal Construction NeedMoveToAbode { get; set; }// Существо необходимо перенести в новую обитель

        //
        internal int PayForHire { get; set; }// Сколько заплачено за найм
        internal int TaxForGuild { get; set; }// Часть золота, отданная в гильдию
        internal int PayForHireWithoutTax { get; set; }// Часть золота, оставленная герою, после уплаты налога в гильдию

        // Выполнение флагов
        internal BigEntity TargetByFlag { get; set; }// Логово флага, который выполняется

        // Статистика за лобби
        internal int TurnOfTrain { get; }// На каком ходу нанят
        internal int Battles { get; }// Участвовал в сражениях
        internal int Wins { get; }// Побед        
        internal int Loses { get; }// Поражений
        internal int Draws { get; }// Ничьих
        internal int DoDamages { get; }// Нанес урона
        internal int DoKills { get; }// Убил героев противника
        internal int Dies { get; }// Сколько раз был убит
        internal int WinStreak { get; }// Побед подряд
        internal int LoseStreak { get; }// Поражений подряд
        internal int DrawStreak { get; }// Ничьих подряд
        internal ResultBattle PriorResultBattle { get; set; }// Предыдущий результат битвы для расчета страйков

        //internal bool Selected { get; set; }

        internal string FullName { get; }// Полное имя
        internal int Gold { get; private set; }// Количество золота у героя



        // Индивидуальные свойства существа
        internal CreatureNeed[] Needs { get; }// Потребности
        internal CreatureInterest[] Interests { get; }// Интересы
        //
        internal bool IsLive { get; private set; } = true;// Существо живо
        internal int DayOfDeath { get; private set; }// День смерти
        internal DescriptorReasonOfDeath ReasonOfDeath { get; private set; }// Причина смерти

        // Действия
        internal Location LocationForScout { get; set; }// Локация, назначенная существу для разведки
        internal int PercentLocationForScout { get; set; }// Сколько процентов локации будет разведано


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

        private void UpdateParamsWithAmmunition()
        {
            // Копируем базовые параметры
            ParametersWithAmmunition.GetFromParams(ParametersBase);

            // Применяем амуницию
            if ((MeleeWeapon == null) || (Armour == null))
                return;
            Debug.Assert(MeleeWeapon != null);
            Debug.Assert(Armour != null);

            ParametersWithAmmunition.MaxMeleeDamage = MeleeWeapon.Descriptor.DamageMelee + (MeleeWeapon.Descriptor.DamageMelee * ParametersWithAmmunition.Strength / 100);
            ParametersWithAmmunition.MinMeleeDamage = ParametersWithAmmunition.MaxMeleeDamage / 2;
            if (RangeWeapon != null)
            {
                ParametersWithAmmunition.MaxArcherDamage = RangeWeapon.Descriptor.DamageRange + (RangeWeapon.Descriptor.DamageRange * ParametersWithAmmunition.Strength / 100);
                ParametersWithAmmunition.MinArcherDamage = ParametersWithAmmunition.MaxArcherDamage / 2;
            }
            else
            {
                ParametersWithAmmunition.MaxArcherDamage = 0;
                ParametersWithAmmunition.MinArcherDamage = 0;
            }
            /*            if (Weapon.DamageMagic > 0)
                            ParametersWithAmmunition.MagicDamage = (ParametersWithAmmunition.Magic / 5) * Weapon.DamageMagic + Level;
                        else
                            ParametersWithAmmunition.MagicDamage = 0;*/
            ParametersWithAmmunition.DefenseMelee = Armour.Descriptor.DefenseMelee;
            ParametersWithAmmunition.DefenseArcher = Armour.Descriptor.DefenseRange;
            ParametersWithAmmunition.DefenseMagic = Armour.Descriptor.DefenseMagic;

            //Debug.Assert((ParametersWithAmmunition.MaxMeleeDamage > 0) || (ParametersWithAmmunition.MaxArcherDamage > 0) || (ParametersWithAmmunition.MagicDamage > 0));
        }

        internal int Priority()
        {
            Debug.Assert(IsLive);

            int posInPlayer = BattleParticipant.CombatHeroes.IndexOf(this);
            Debug.Assert(posInPlayer != -1);
            return TypeCreature.DefaultPositionPriority * 1000 + posInPlayer;
        }

        private void DoCustomDraw(Graphics g, int x, int y, bool drawState)
        {
            if (drawState && (Construction.Descriptor.ID != "Castle"))
                Program.formMain.ilStateHero.DrawImage(g, StateCreature.ImageIndex, true, false, x - 7, y - 3);
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

            Debug.Assert(Abode != null);
            Debug.Assert(Abode.Heroes.IndexOf(this) != -1);

            // Перемещаем героя из его сооружения на кладбище
            NeedMoveToAbode = Player.Graveyard;

            Player.AddNoticeForPlayer(this, TypeNoticeForPlayer.HeroIsDead);
        }

        internal override string GetLevel()
        {
            return Level.ToString();
        }

        internal override int GetQuantity()
        {
            return 0;
        }

        internal override int GetImageIndex() => Program.formMain.TreatImageIndex(TypeCreature.ImageIndex, Player);

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

            //Debug.Assert(((RangeWeapon != null) && (Quiver != null)) || ((RangeWeapon is null) && (Quiver is null)));
        }

        // Реализация интерфейса
        internal override bool GetNormalImage() => IsLive;

        internal override void Click(VCCell pe)
        {
            Debug.Assert(IsLive);

            Program.formMain.layerGame.SelectPlayerObject(this);
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
                if (a.DescriptorAbility == da)
                    return true;
            }

            return false;
        }

        private static int CompareAbility(Ability a1, Ability a2)
        {
            Debug.Assert(a1.Pos != a2.Pos);

            DescriptorAbility d1 = a1.DescriptorAbility;
            DescriptorAbility d2 = a2.DescriptorAbility;

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

            Creature signer = null;
            if (di.Signer.Length > 0)
            {
                if (di.Signer == "King")
                {
                    if (BattleParticipant is Player p)
                    {
                        foreach (Creature c in p.AllHeroes)
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
            Debug.Assert(dp != null, $"У {GetName()} ссылка на удаляемый перк пуста");

            Perk removedPerk = null;
            foreach (Perk p in Perks)
            {
                //Debug.Assert(p.Descriptor != null, $"У {GetName()} для перка {p.GetName()} не указан описатель");

                if (p.Descriptor != null)// У своего перка нет описателя
                {
                    if (p.Descriptor.ID == dp.ID)
                    {
                        removedPerk = p;
                        break;
                    }
                }
            }

            if (removedPerk != null)
                Perks.Remove(removedPerk);
        }

        internal override void MakeMenu(VCMenuCell[,] menu)
        {

        }

        internal virtual void PrepareNewDay()
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
                                //SetIsDead(cn.Need.Descriptor.ReasonOfDeath);
                                break;
                            }
                        }
                    }
                }
            }
        }

        internal void SetLocationForScout(Location l)
        {
            if (l != null)
            {
                Debug.Assert(l.Player == BattleParticipant);
                //Debug.Assert(StateCreature != FormMain.Descriptors.StateCreatureDoFlatScout);
                Debug.Assert(LocationForScout is null);
                Debug.Assert(PercentLocationForScout == 0);

                LocationForScout = l;
                StateCreature = FormMain.Descriptors.StateCreatureDoFlagScout;
                if (this is Creature h)
                {
                    h.TargetByFlag = l;
                    LocationForScout.PayForHire += h.PayForHire;
                }

                PercentLocationForScout = CalcPercentScoutArea(LocationForScout);
                LocationForScout.ListCreaturesForScout.Add(this);
                LocationForScout.CalcPercentScoutToday();
            }
            else
            {
                Debug.Assert(PercentLocationForScout > 0);

                StateCreature = TypeCreature.PersistentStateHeroAtMap;
                if (this is Creature h)
                {
                    LocationForScout.PayForHire -= h.PayForHire;
                    h.TargetByFlag = null;
                }
                LocationForScout.CalcPercentScoutToday();
                PercentLocationForScout = 0;
                LocationForScout.ListCreaturesForScout.Remove(this);
                LocationForScout = null;
            }
        }

        internal void ScoutExecuted()
        {
            StateCreature = TypeCreature.PersistentStateHeroAtMap;
            if (this is Creature h)
            {
                h.PayForHire = 0;
                h.TaxForGuild = 0;
                h.PayForHireWithoutTax = 0;
            }
    
            LocationForScout = null;
        }

        internal int CalcPercentScoutArea(Location l)
        {
            // Вычисляем процент локации, который разведывает существо согласно уровню разведки
            return Convert.ToInt32(Properties.PropertyScout * l.Settings.PercentScoutAreaByUnit / 100.0000);
        }

        internal override string GetTypeEntity() => TypeCreature.Name;

        // Увольнение героя
        internal void Dismiss()
        {
            Debug.Assert(Construction.Heroes.IndexOf(this) != -1);
            Debug.Assert(Construction.Player.CombatHeroes.IndexOf(this) != -1);

            Construction.Heroes.Remove(this);
            Construction.Player.CombatHeroes.Remove(this);

            Debug.Assert(Construction.Heroes.IndexOf(this) == -1);
            Debug.Assert(Construction.Player.CombatHeroes.IndexOf(this) == -1);
        }

        internal int FindSlotWithItem(DescriptorItem item)
        {
            // Сначала ищем слот, заполненный таким же предметом
            for (int i = 0; i < Inventory.Count; i++)
                if (Inventory[i].Descriptor == item)
                    return i;

            return -1;
        }

        internal int FindCellForItem(DescriptorItem item)
        {
            int number = FindSlotWithItem(item);
            if (number != -1)
                return number;

            // Ищем пустой слот, разрешенный для такого типа предметов
            //for (int i = 0; i < Inventory.Count; i++)
            //    if (Inventory[i].Item == item)
            //        return i;

            return -1;
        }

        internal void AcceptItem(Item pi, int quantity)
        {
            Debug.Assert(pi.Quantity > 0);
            Debug.Assert(quantity > 0);
            Debug.Assert(pi.Quantity >= quantity);

            int toCell = FindCellForItem(pi.Descriptor);
            if (toCell == -1)
                return;

            AcceptItem(pi, quantity, toCell);
        }

        internal void AcceptItem(Item pi, int quantity, int toCell)
        {
            Debug.Assert(pi.Quantity > 0);
            Debug.Assert(quantity > 0);
            Debug.Assert(pi.Quantity >= quantity);

            // Проверяем совместимость
            /*            if (pi.Item.TypeItem != ClassHero.Slots[toCell].TypeItem)
                            return;

                        if (Slots[toCell] != null)
                        {
                            if (ClassHero.Slots[toCell].DefaultItem != null)
                            {
                                // Если это дефолтный предмет, удаляем его
                                if (Slots[toCell].Item == ClassHero.Slots[toCell].DefaultItem)
                                    Slots[toCell] = null;
                                else
                                {
                                    // Иначе помещаем предмет на склад
                                    // Если не можем поместить вещь на склад, выходим
                                    if (Construction.Player.GetItemFromHero(this, toCell) == true)
                                        Slots[toCell] = null;
                                    else
                                        return;
                                }
                            }

                            // Если разный тип предметов, то пытаемся поместить предмет на склад
                            if ((Slots[toCell] != null) && (Slots[toCell].Item != pi.Item))
                            {
                                if (Construction.Player.GetItemFromHero(this, toCell) == true)
                                    Slots[toCell] = null;
                                else
                                    return;
                            }
                        }

                        if (Slots[toCell] == null)
                        {
                            Slots[toCell] = new PlayerItem(pi.Item, Math.Min(ClassHero.MaxQuantityTypeItem(ClassHero.Slots[toCell].TypeItem), quantity), false);
                            pi.Quantity -= Slots[toCell].Quantity;
                        }
                        else
                        {
                            int add = Math.Min(ClassHero.MaxQuantityTypeItem(ClassHero.Slots[toCell].TypeItem) - Slots[toCell].Quantity, quantity);
                            if (add > 0)
                            {
                                Slots[toCell] = new PlayerItem(pi.Item, add, false);
                                pi.Quantity -= add;
                            }
                        }

                        Debug.Assert(Slots[toCell] != null);*/
        }

        internal Item TakeItem(int fromCell, int quantity)
        {
            return null;
            /*Debug.Assert(quantity > 0);
            Debug.Assert(Slots[fromCell] != null);
            Debug.Assert(Slots[fromCell].Quantity > 0);
            Debug.Assert(Slots[fromCell].Quantity >= quantity);

            PlayerItem pi;

            // Если забирают всё, то возвращаем ссылку на этот предмет и убираем его у себя, иначе делим предмет
            if (Slots[fromCell].Quantity == quantity)
            {
                pi = Slots[fromCell];
                Slots[fromCell] = null;

                ValidateCell(fromCell);
            }
            else
            {
                pi = new PlayerItem(Slots[fromCell].Item, quantity, false);
                Slots[fromCell].Quantity -= quantity;
            }

            switch (pi.Item.TypeItem.Category)
            {
                case CategoryItem.Weapon:
                    Weapon = null;
                    break;
                case CategoryItem.Armour:
                    Armour = null;
                    break;
            }

            return pi;*/
        }

        internal void ValidateCell(int number)
        {
            //if ((ClassHero.Slots[number].DefaultItem != null) && (Slots[number] == null))
            //{
            //    Slots[number] = new PlayerItem(ClassHero.Slots[number].DefaultItem, 1, false);
            //}
        }

        internal void MoveItem(int fromSlot, int toSlot)
        {
            /*Debug.Assert(Slots[fromSlot] != null);
            Debug.Assert(fromSlot != toSlot);

            if (Slots[fromSlot].Item.TypeItem == ClassHero.Slots[toSlot].TypeItem)
            {
                PlayerItem tmp = null;
                if (Slots[toSlot] != null)
                    tmp = Slots[toSlot];
                Slots[toSlot] = Slots[fromSlot];
                Slots[fromSlot] = tmp;
            }*/
        }

        internal override string GetName() => GetNameHero();

        internal string GetNameHero()
        {
            Debug.Assert(FullName != null);
            Debug.Assert(FullName.Length > 0);

            return TypeCreature.ImageIndex != FormMain.IMAGE_INDEX_CURRENT_AVATAR ? FullName : Player.GetName();
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            if (IsLive)
            {
                panelHint.AddStep2Entity(this);
                panelHint.AddStep4Level($"Уровень {Level}");
                panelHint.AddStep45State((StateCreature.Name, Color.SkyBlue));
                panelHint.AddStep5Description(TypeCreature.Description);
                panelHint.AddStep7Reward(TypeCreature.TypeReward.Cost.ValueGold());
                panelHint.AddStep8Greatness(TypeCreature.TypeReward.Greatness, 0);
            }
            else
            {
                panelHint.AddStep2Entity(this);
                panelHint.AddStep4Level($"Уровень {Level}");
                panelHint.AddStep5Description($"День смерти: {DayOfDeath}{Environment.NewLine}{ReasonOfDeath.Name}");
            }
        }

        internal override void HideInfo()
        {
            base.HideInfo();

            if (TypeCreature.CategoryCreature == CategoryCreature.Monster)
                Lobby.Layer.panelMonsterInfo.Visible = false;
            else
                Lobby.Layer.panelHeroInfo.Visible = false;
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Debug.Assert(IsLive);

            if (TypeCreature.CategoryCreature == CategoryCreature.Monster)
            {
                Lobby.Layer.panelMonsterInfo.Entity = this;
                Lobby.Layer.panelMonsterInfo.Visible = true;
            }
            else
            {
                Lobby.Layer.panelHeroInfo.Visible = true;
                Lobby.Layer.panelHeroInfo.Entity = this;
            }
        }

        internal void ClearState()
        {
            Debug.Assert(IsLive);
            Debug.Assert((StateCreature.ID == NameStateCreature.DoAttackFlag.ToString())
                || (StateCreature.ID == NameStateCreature.DoDefenseFlag.ToString())
                || (StateCreature.ID == NameStateCreature.DoScoutFlag.ToString())
                || (StateCreature.ID == NameStateCreature.BattleWithPlayer.ToString()));
            //Debug.Assert(TargetByFlag != null);// Убрано из-за локации

            // Убираем себя из флага на логове
            if (TargetByFlag != null)
                TargetByFlag.ComponentObjectOfMap.RemoveAttackingHero(this);
            SetState(NameStateCreature.Nothing);
        }

        internal NameStateCreature StateForFlag(TypeFlag typeFlag)
        {
            Debug.Assert(IsLive);

            switch (typeFlag)
            {
                case TypeFlag.None:
                    return NameStateCreature.Nothing;
                case TypeFlag.Scout:
                    return NameStateCreature.DoScoutFlag;
                case TypeFlag.Attack:
                    return NameStateCreature.DoAttackFlag;
                case TypeFlag.Defense:
                    return NameStateCreature.DoDefenseFlag;
                case TypeFlag.Battle:
                    return NameStateCreature.BattleWithPlayer;
                default:
                    throw new Exception($"Неизвестный тип флага: {typeFlag}");
            }
        }

        internal void AddGold(int income)
        {
            Debug.Assert(IsLive);
            Debug.Assert(income > 0);

            Gold += income;
        }

        internal void SpendGold(int spend)
        {
            Debug.Assert(IsLive);
            Debug.Assert(spend > 0);

            Gold -= spend;
            Debug.Assert(Gold >= 0);
        }

        internal void PrepareQueueShopping(List<UnitOfQueueForBuy> queue)
        {
            Debug.Assert(IsLive);

            foreach (PriorityConstructionForShopping pc in TypeCreature.PriorityConstructionForShoppings)
            {
                queue.Add(new UnitOfQueueForBuy(this, Player.FindConstruction(pc.Descriptor.ID), (int)pc.Priority));
            }
        }

        internal void DoShopping(Construction c)
        {
            bool shopped = false;
            bool abilityBought = false;

            // Получаем список доступных покупок
            List<ConstructionProduct> listProducts = c.GetProducts(TypeCreature);

            // Покупаем все, что можем
            /*foreach (ConstructionProduct cp in listProducts)
            {
                if (cp.DescriptorAbility != null)
                {
                    if (!AbilityExists(cp.DescriptorAbility))
                    {
                        AddAbility(c.PurchaseAbility(this, cp));
                        abilityBought = true;
                    }
                }
            }*/

            if (abilityBought)
                SortAbilities();
        }

        internal int CostOfHiring()
        {
            return TypeCreature.CostOfHiring != 0 ? TypeCreature.CostOfHiring + (int)(TypeCreature.CostOfHiring * Math.Truncate(Level / (decimal)10)) : 0;
        }

        internal int Hire()
        {
            Debug.Assert(PayForHire == 0);
            Debug.Assert(TaxForGuild == 0);
            Debug.Assert(PayForHireWithoutTax == 0);

            PayForHire = CostOfHiring();
            TaxForGuild = Construction.CalcTax(PayForHire);
            if (TaxForGuild > 0)
                Construction.ChangeGold(TaxForGuild);
            PayForHireWithoutTax = PayForHire - TaxForGuild;
            if (PayForHireWithoutTax > 0)
                AddGold(PayForHireWithoutTax);

            return PayForHire;
        }

        internal int Unhire()
        {
            Debug.Assert(PayForHire > 0);

            int g = PayForHire;

            if (TaxForGuild > 0)
                Construction.ChangeGold(-TaxForGuild);
            if (PayForHireWithoutTax > 0)
                SpendGold(PayForHireWithoutTax);

            PayForHire = 0;
            TaxForGuild = 0;
            PayForHireWithoutTax = 0;
            return g;
        }

        internal override bool ProperName() => TypeCreature.CategoryCreature == CategoryCreature.Hero;
    }
}
