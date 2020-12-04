using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс метательных снарядов
    internal abstract class Missile
    {
        public Missile(HeroInBattle hero, BattlefieldTile target)
        {
            Hero = hero;
            SourceTile = hero.CurrentTile;
            DestTile = target;
            StepsPassed = 0;

            Debug.Assert(SourceTile != null);
            Debug.Assert(DestTile != null);

            // Считаем количество шагов до цели
            // Для этого вычисляем расстояние в клетках и делим на скорость
            double distance = Utils.DistanceBetweenPoints(SourceTile.Coord, DestTile.Coord);
            StepsToTarget = (int)(distance / hero.PlayerHero.RangeWeapon.VelocityMissile * FormMain.Config.StepsInSecond);

            Debug.Assert(StepsToTarget > 0);
        }

        internal HeroInBattle Hero { get; }
        internal BattlefieldTile SourceTile { get; }
        internal BattlefieldTile DestTile { get; }
        internal int StepsToTarget { get; }
        internal int StepsPassed { get; private set; }
        internal abstract void Draw(Graphics g, Point p1, Point p2);

        internal void ApplyStep()
        {
            Debug.Assert(StepsPassed < StepsToTarget);

            StepsPassed++;

            if (StepsPassed == StepsToTarget)
            {
                if (DestTile.Unit != null)
                {
                    // Причиняем урон
                    // Урон должен считаться не по текущим параметрам, а параметрам в момент выстрела
                    DestTile.Unit.GetDamage(Hero.CalcDamageMelee(DestTile.Unit), Hero.CalcDamageShoot(DestTile.Unit), Hero.CalcDamageMagic(DestTile.Unit));
                }
            }
        }
    }

    // Стрела
    internal sealed class Arrow : Missile
    {
        private Pen penArrow;

        public Arrow(HeroInBattle hero, BattlefieldTile target) : base(hero, target)
        {
            penArrow = new Pen(Hero.PlayerHero.Player == Hero.Battle.Player1 ? Color.Green : Color.Maroon);
            penArrow.Width = 2;
            penArrow.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4.0F, 8.0F, true);
        }

        internal override void Draw(Graphics g, Point p1, Point p2)
        {
            // Определяем местоположение снаряда исходя из прогресса
            double percent = 1.00 * StepsPassed / StepsToTarget;
            Point p = new Point();
            p.X = (int)(p1.X + ((p2.X - p1.X) * percent));
            p.Y = (int)(p1.Y + ((p2.Y - p1.Y) * percent));

            // Устанавливаем длину стрелы
            double currentLength = Utils.DistanceBetweenPoints(p1, p);
            double cutLength = currentLength - 30;
            if (cutLength > 0)
            {
                double proportion = cutLength / Utils.DistanceBetweenPoints(p1, p2);
                p1.X = (int)(p1.X + ((p2.X - p1.X) * proportion));
                p1.Y = (int)(p1.Y + ((p2.Y - p1.Y) * proportion));
            }

            g.DrawLine(penArrow, p1, p);
        }
    }

    // Магический выстрел
    internal sealed class MagicStrike : Missile
    {
        private Brush brush;

        public MagicStrike(HeroInBattle hero, BattlefieldTile target) : base(hero, target)
        {
            brush = new SolidBrush(Hero.PlayerHero.Player == Hero.Battle.Player1 ? Color.Green : Color.Maroon);
        }

        internal override void Draw(Graphics g, Point p1, Point p2)
        {
            // Определяем местоположение снаряда исходя из прогресса
            double percent = 1.00 * StepsPassed / StepsToTarget;
            Point p = new Point();
            p.X = (int)(p1.X + ((p2.X - p1.X) * percent));
            p.Y = (int)(p1.Y + ((p2.Y - p1.Y) * percent));

            g.FillEllipse(brush, p.X - 5, p.Y - 5, 11, 11);
        }
    }
}
