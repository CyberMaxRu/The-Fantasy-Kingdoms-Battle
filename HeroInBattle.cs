using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_King_s_Battle
{
    internal enum StateHeroInBattle { MeleeAttack, RangeAttack, Cast, Drink, Healing, Rest, Resurrection, Tumbstone, Dead, 
        PrepareMove, Move, None }// Состояние героя в бою

    internal sealed class HeroInBattle
    {
        private int countAction;// Счетчик действия
        private int timeAction;// Какое количество времени выполнения действие
        private bool inRollbackAfterAction;// Герой во время отката после выполнения действия        
        private HeroInBattle lastAttackedHero;
        private BattlefieldTile currentTile;
        private SolidBrush brushBandHealth;
        private SolidBrush brushBandHealthNone;
        private Bitmap bmpIcon;
        private bool needRedraw;
        private StateHeroInBattle priorState;

        public HeroInBattle(Battle b, PlayerHero ph, Point coord, bool needImage)
        {
            Battle = b;
            CurrentTile = b.Battlefield.Tiles[coord.Y, coord.X];
            PlayerHero = ph;
            IsLive = true;

            Parameters = new HeroParameters(ph.ParametersWithAmmunition);

            Debug.Assert(Parameters.Health > 0);
            Debug.Assert(Parameters.Mana > 0);
            Debug.Assert(Parameters.Stamina > 0);

            CurrentHealth = Parameters.Health;
            CurrentMana = Parameters.Mana;
            CurrentStamina = Parameters.Stamina;

            State = StateHeroInBattle.None;

            LastTarget = default;

            if (needImage)
            {
                bmpIcon = new Bitmap(Program.formMain.bmpBorderForIcon.Size.Width, Program.formMain.bmpBorderForIcon.Size.Height);
                brushBandHealth = new SolidBrush(FormMain.Config.UnitHealth);
                brushBandHealthNone = new SolidBrush(FormMain.Config.UnitHealthNone);
                needRedraw = true;
            }
        }

        internal PlayerHero PlayerHero { get; }
        internal Player Player => PlayerHero.Player;
        internal HeroParameters Parameters { get; }
        internal Battle Battle { get; }
        internal bool IsLive { get; set; }
        internal StateHeroInBattle State { get; private set; }
        internal HeroInBattle Target { get; private set; }
        internal Point LastTarget { get; private set; }
        internal int CurrentHealth { get; set; }
        internal int CurrentMana { get; set; }
        internal int CurrentStamina { get; set; }
        internal int ReceivedDamage { get; private set; }

        internal Bitmap BmpIcon
        {
            get
            {
                Debug.Assert(bmpIcon != null);

                if (needRedraw)
                    DrawIcon();
                return bmpIcon;
            }
        }

        //
        internal Point Coord { get { return new Point(currentTile.X, currentTile.Y); } }
        internal BattlefieldTile CurrentTile
        {
            get { return currentTile; }
            set
            {
                Debug.Assert(value != null);

                if (currentTile != null)
                    currentTile.Unit = null;

                currentTile = value;
                currentTile.Unit = this;
            }
        }
        internal BattlefieldTile DestinationForMove { get; set; }
        internal BattlefieldTile TileForMove { get; set; }
        internal List<BattlefieldTile> PathToDestination;
        //internal int MoveStepPaa

        // Делает шаг битвы
        internal void DoStepBattle(Battle b)
        {
            Debug.Assert(IsLive == true);
            Debug.Assert(CurrentTile != null);

            priorState = State;

            if (inRollbackAfterAction == false)
            {
                switch (State)
                {
                    case StateHeroInBattle.None:
                        Debug.Assert(Target == null);
                        Debug.Assert(countAction == 0);
                        Debug.Assert(CurrentHealth > 0);
                        Debug.Assert(IsLive == true);

                        // Если сейчас ничего не выполняем, ищем, что можно сделать
                        // Сначала пробуем атаковать стрелковым оружием

                        if ((PlayerHero.RangeWeapon != null) || (PlayerHero.ClassHero.ID == "Cleric") || (PlayerHero.ClassHero.ID == "Mage"))
                        {
                            bool underMeleeAttack = false;
                            // Если юнит не атакован врукопашную, можно атаковать стрелковой атакой
                            foreach (HeroInBattle h in b.ActiveHeroes)
                            {
                                if ((h != this) && (h.Target == this) && (h.State == StateHeroInBattle.MeleeAttack))
                                {
                                    underMeleeAttack = true;
                                    break;
                                }
                            }
                            
                            if (!underMeleeAttack)
                                SearchTargetForShoot();
                        }

                        if (Target == null)
                        {
                            if (!SearchTargetForMelee())
                            {
                                // Если целей нет, идем к ней
                                if (Target == null)
                                {
                                    if (SearchTargetForMove() == true)
                                    {
                                        State = StateHeroInBattle.Move;
                                        countAction = (int)(TimeMove() * 1.00 * ((TileForMove.X != 0) && (TileForMove.Y != 0) ? 1.4 : 1));
                                        timeAction = countAction;
                                        inRollbackAfterAction = false;
                                        //State = StateHeroInBattle.PrepareMove;
                                    }
                                }
                            }

                        }

                        break;
                    case StateHeroInBattle.MeleeAttack:
                        countAction--;

                        if (Target.State != StateHeroInBattle.Tumbstone)
                        {
                            if (countAction == 0)
                            {
                                // Делаем удар по противнику
                                Target.GetDamage(CalcDamageMelee(Target), CalcDamageShoot(Target), CalcDamageMagic(Target));
                                LastTarget = Target.Coord;
                                Target = null;

                                // После удара делаем паузу длиной во время атаки
                                countAction = TimeAttack();
                                inRollbackAfterAction = true;
                            }
                        }
                        else
                        {
                            // Противника уже убили, пропускаем ход
                            LastTarget = Target.Coord;
                            Target = null;
                            State = StateHeroInBattle.None;
                            countAction = timeAction - countAction;
                            timeAction = countAction;
                            inRollbackAfterAction = true;
                        }

                        break;
                    case StateHeroInBattle.RangeAttack:
                    case StateHeroInBattle.Cast:
                        countAction--;

                        if (Target.State != StateHeroInBattle.Tumbstone)
                        {
                            if (countAction == 0)
                            {
                                // Делаем удар по противнику
                                LastTarget = Target.Coord;
                                Target = null;

                                // После удара делаем паузу длиной во время атаки
                                countAction = TimeAttack();
                                inRollbackAfterAction = true;
                            }
                        }
                        else
                        {
                            // Противника уже убили, пропускаем ход
                            LastTarget = Target.Coord;
                            Target = null;
                            State = StateHeroInBattle.None;
                            countAction = timeAction - countAction;
                            timeAction = countAction;
                            inRollbackAfterAction = true;
                        }

                        break;
                    case StateHeroInBattle.Tumbstone:
                        Debug.Assert(Target == null);

                        countAction--;
                        if (countAction == 0)
                        {
                            IsLive = false;
                            State = StateHeroInBattle.Dead;
                            currentTile.Unit = null;
                            currentTile = null;
                        }

                        break;
                    case StateHeroInBattle.Move:
                        Debug.Assert(TileForMove != null);
                        Debug.Assert(TileForMove.Unit == null);

                        countAction--;
                        if (countAction == 0)
                        {
                            Debug.Assert(TileForMove.ReservedForMove == this);

                            // Пришли на тайл
                            Target = null;
                            CurrentTile = TileForMove;
                            TileForMove.ReservedForMove = null;
                            TileForMove = null;

                            if (PathToDestination.Count() > 1)
                            {
                                // Заново осматриваемся
                                State = StateHeroInBattle.None;

                                /*TileForMove = PathToDestination.First();
                                PathToDestination.RemoveAt(0);

                                countAction = TimeMove();
                                timeAction = countAction;*/
                            }
                            else
                            {
                                // Пришли на конечный тайл
                                State = StateHeroInBattle.None;
                            }
                            PathToDestination = null;
                            DestinationForMove = null;
                        }

                        break;
                    default:
                        break;
                }
            }
            else
            {
                countAction--;
                if (countAction == 0)
                {
                    LastTarget = default;
                    State = StateHeroInBattle.None;
                    inRollbackAfterAction = false;
                }
            }

            if (priorState != State)
                needRedraw = true;            

            bool SearchTargetForMelee()
            {
                // Ищем, кого атаковать
                List<HeroInBattle> targets = new List<HeroInBattle>();

                foreach (HeroInBattle h in b.ActiveHeroes)
                {
                    // Собираем список вражеских героев вокруг себя
                    if (h.IsLive == true)
                        if (h.Player != Player)
                            if (h.CurrentHealth > 0)
                                if (IsNeighbour(h) == true)
                                   targets.Add(h);
                }

                if (targets.Count > 0)
                {
                    Debug.Assert(this != targets[0]);

                    State = StateHeroInBattle.MeleeAttack;
                    Target = targets[0];
                    countAction = TimeAttack();
                    timeAction = countAction;

                    return true;
                }
                else
                    return false;
            }

            bool SearchTargetForShoot()
            {
                //Debug.Assert(PlayerHero.RangeWeapon != null);

                // Если герой, по которому стреляли, жив, атакуем его снова
                if ((lastAttackedHero != null) && (lastAttackedHero.CurrentHealth > 0))
                {
                    Target = lastAttackedHero;
                }
                else
                {
                    // Ищем, кого атаковать
                    List<HeroInBattle> targets = new List<HeroInBattle>();

                    foreach (HeroInBattle h in b.ActiveHeroes)
                    {
                        // Собираем список вражеских героев
                        if (h.Player != Player)
                            if (h.CurrentHealth > 0)
                                targets.Add(h);
                    }

                    if (targets.Count > 0)
                    {
                        Debug.Assert(this != targets[0]);
                        Target = targets[0];// targets[Battle.Rnd.Next(0, targets.Count - 1)];
                    }
                }

                if (Target != null)
                { 
                    State = StateHeroInBattle.RangeAttack;
                    countAction = TimeAttack();
                    timeAction = countAction;
                    lastAttackedHero = Target;

                    // Создаем выстрел
                    if (PlayerHero.RangeWeapon != null)
                        Battle.Missiles.Add(new Arrow(this, Target.CurrentTile));
                    else
                        Battle.Missiles.Add(new MagicStrike(this, Target.CurrentTile));

                    return true;
                }
                else
                    return false;
            }

            bool IsNeighbour(HeroInBattle hb)
            {
                Debug.Assert(this != hb);

                return Utils.PointsIsNeighbor(Coord, hb.Coord);
            }
        }

        // Применяем шаг битвы
        internal void ApplyStepBattle()
        {
            CurrentHealth -= ReceivedDamage;

            if (State != StateHeroInBattle.Tumbstone)
            {
                if (CurrentHealth <= 0)
                {
                    LastTarget = default;
                    Target = null;
                    DestinationForMove = null;

                    if (TileForMove != null)
                    {
                        TileForMove.ReservedForMove = null;
                        TileForMove = null;
                    }
                    State = StateHeroInBattle.Tumbstone;
                    countAction = FormMain.Config.StepsHeroInTumbstone;
                    timeAction = countAction;
                    CurrentHealth = 0;
                    inRollbackAfterAction = false;
                }
            }

            ReceivedDamage = 0;

            Debug.Assert(CurrentHealth <= Parameters.Health);
            Debug.Assert(CurrentMana <= Parameters.Mana);
            Debug.Assert(CurrentStamina <= Parameters.Stamina);
        }

        private int TimeAttack()
        {
            Debug.Assert((State == StateHeroInBattle.MeleeAttack) || (State == StateHeroInBattle.RangeAttack));

            int timeAttack;
            switch (State)
            {
                case StateHeroInBattle.MeleeAttack:
                    timeAttack = (int)(PlayerHero.MeleeWeapon.TimeHit / 100.00 * FormMain.Config.StepsInSecond);
                    break;
                case StateHeroInBattle.RangeAttack:
                    // Костыль для магов
                    if (PlayerHero.RangeWeapon != null)
                        timeAttack = (int)(PlayerHero.RangeWeapon.TimeHit / 100.00 * FormMain.Config.StepsInSecond);
                    else
                        timeAttack = (int)(PlayerHero.MeleeWeapon.TimeHit / 100.00 * FormMain.Config.StepsInSecond);
                    break;
                default:
                    throw new Exception("Неизвестное состояние.");
            }

            Debug.Assert(timeAttack > 0);

            return timeAttack;
        }

        internal int CalcDamageMelee(HeroInBattle target)
        {
            int delta = Parameters.MaxMeleeDamage - Parameters.MinMeleeDamage;
            int value = FormMain.Rnd.Next(delta);

            int d = Parameters.MaxMeleeDamage;// Parameters.MinMeleeDamage + value;

            return d;
        }

        internal int CalcDamageShoot(HeroInBattle target)
        {
            int delta = Parameters.MaxArcherDamage - Parameters.MinArcherDamage;
            int value = FormMain.Rnd.Next(delta);

            int d = Parameters.MaxArcherDamage;// Parameters.MinArcherDamage + value;

            return d;
        }
        internal int CalcDamageMagic(HeroInBattle target)
        {
            int d = Parameters.MagicDamage;

            return d;
        }

        internal void GetDamage(int damageMelee, int damageArcher, int damageMagic)
        {
            Debug.Assert((damageMelee > 0) || (damageArcher > 0) || (damageMagic > 0));
            if (State == StateHeroInBattle.Tumbstone)
                return;
            //if ((State == StateHeroInBattle.Tumbstone) && (damageArcher > 0))
            //    return;
            // Временно отключено, до реализации урона магами через заклинание
            //Debug.Assert(State != StateHeroInBattle.Tumbstone);
            Debug.Assert(State != StateHeroInBattle.Dead);
            Debug.Assert(State != StateHeroInBattle.Resurrection);

            ReceivedDamage += damageMelee + damageArcher + damageMagic;
            needRedraw = true;
        }

        internal void CalcParameters()
        {
            // Расчет надо делать через модифицированные основные параметры, после которых уже применяются баффы/дебаффы на урон
            // Сейчас для простоты берем уже посчитанные параметры с амуницией
            Parameters.MinMeleeDamage = PlayerHero.ParametersWithAmmunition.MinMeleeDamage;
            Parameters.MaxMeleeDamage = PlayerHero.ParametersWithAmmunition.MaxMeleeDamage;
            Parameters.MinArcherDamage = PlayerHero.ParametersWithAmmunition.MinArcherDamage;
            Parameters.MaxArcherDamage = PlayerHero.ParametersWithAmmunition.MaxArcherDamage;
            Parameters.MagicDamage = PlayerHero.ParametersWithAmmunition.MagicDamage;

            Debug.Assert(Parameters.MinMeleeDamage <= Parameters.MaxMeleeDamage);
            Debug.Assert(Parameters.MinArcherDamage <= Parameters.MaxArcherDamage);
        }

        internal double PercentExecuteAction()
        {
            Debug.Assert(timeAction > 0);
            Debug.Assert(countAction > 0);
            double percent = 1.00 * (timeAction - countAction) / timeAction;

            Debug.Assert(percent >= 0);
            Debug.Assert(percent <= 1);
            return percent;
        }

        internal bool InRollbackAction()
        {
            return inRollbackAfterAction;
        }

        private bool SearchTargetForMove()
        {
            // Ищем ближайшего противника
            // Здесь переделать на перебор всех противников, до которых можно добраться, и учесть их вес
            foreach (HeroInBattle h in Battle.ActiveHeroes)
                if ((h.Player != Player) && (h.currentTile != null) && (h.State != StateHeroInBattle.Tumbstone))
                {
                    bool pathFinded = Battle.Battlefield.Pathfind(CurrentTile, h.currentTile, null);

                    // Если некуда идти, то надо идти в сторону противника. Возможно, после шага к нему можно будет пройти
                    if (!pathFinded)
                        pathFinded = Battle.Battlefield.Pathfind(CurrentTile, h.currentTile, Player);

                    if (pathFinded && (Battle.Battlefield._path[0].Unit == null))
                    {
                        PathToDestination = Battle.Battlefield._path;
                        DestinationForMove = PathToDestination.Last();
                        Debug.Assert(DestinationForMove == h.currentTile);
                        TileForMove = PathToDestination.First();
                        Debug.Assert(TileForMove.Unit == null);
                        Debug.Assert(TileForMove.ReservedForMove == null);
                        Debug.Assert(Utils.PointsIsNeighbor(Coord, new Point(TileForMove.X, TileForMove.Y)) == true);

                        Debug.Assert(TileForMove.ReservedForMove == null);
                        TileForMove.ReservedForMove = this;

                        return true;
                    }
                }
            //foreach (BattlefieldTile t in curTile.TilesAround)
            //{
            //    if (SearchAround(t) == true)
            //        return true;
            //}

            return false;

            bool SearchAround(BattlefieldTile tile)
            {
                foreach (BattlefieldTile t in tile.TilesAround)
                {
                    if (t.Unit != null)
                        if (t.Unit.Player != Player)
                            if (t.Unit.IsLive == true)
                            // if ((t.Unit.IsLive == true) && (t.Unit.State != StateHeroInBattle.Move))
                            {
                                // Смотрим, можно ли к нему построить путь
                                if (Battle.Battlefield.Pathfind(CurrentTile, t.Unit.currentTile, null) == true)
                                {
                                    PathToDestination = Battle.Battlefield._path;
                                    DestinationForMove = PathToDestination.Last();
                                    return true;
                                }
                            }
                }

                return false;
            }
        }

        private void Move()
        {

        }

        private int TimeMove()
        {
            int timeMove = (int)(PlayerHero.ParametersWithAmmunition.SecondsToMove / 100.00 * FormMain.Config.StepsInSecond);
            if (timeMove == 0)
                timeMove = 1 * FormMain.Config.StepsInSecond;
            //timeMove = 3 * FormMain.Config.StepsInSecond;
            Debug.Assert(timeMove > 0);
            return timeMove;
        }

        private void DrawIcon()
        {
            Debug.Assert(bmpIcon != null);
            Debug.Assert(IsLive);

            Graphics g = Graphics.FromImage(bmpIcon);

            // Рисуем иконку героя
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.DrawImageUnscaled(Program.formMain.ilGuiHeroes.Images[GuiUtils.GetImageIndexWithGray(Program.formMain.ilGuiHeroes, PlayerHero.ClassHero.ImageIndex, State != StateHeroInBattle.Tumbstone)], FormMain.Config.ShiftForBorder);

            // Если это противник, то обращаем его в противоположную сторону
            if (PlayerHero.Player != Battle.Player1)
                bmpIcon.RotateFlip(RotateFlipType.RotateNoneFlipX);

            // Рисуем бордюр
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImageUnscaled(Program.formMain.bmpBorderForIcon, 0, 0);
            
            // Рисуем состояние
            if ((State != StateHeroInBattle.None) || (priorState != StateHeroInBattle.None))
            {
                StateHeroInBattle s = State != StateHeroInBattle.None ? State : priorState;

                g.DrawImageUnscaled(Program.formMain.ilStateHero.Images[(int)s], FormMain.Config.ShiftForBorder.X + 1, FormMain.Config.ShiftForBorder.Y + 1);
            }
            
            // Рисуем полоску жизни
            GuiUtils.DrawBand(g, new Rectangle(FormMain.Config.ShiftForBorder.X + 2, FormMain.Config.ShiftForBorder.Y + Program.formMain.ilGuiHeroes.ImageSize.Height - 6, Program.formMain.ilGuiHeroes.ImageSize.Height - 4, 4), brushBandHealth, brushBandHealthNone, CurrentHealth, Parameters.Health);

            needRedraw = false;
            g.Dispose();
        }
    }
}