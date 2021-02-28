using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypePlayer { Human, Computer, Lair };

    // Класс участника битвы
    internal abstract class BattleParticipant : PlayerObject
    {
        private static int ComparePlaceCreature(Creature c1, Creature c2)
        {
            return c1.ID - c2.ID;
        }

        public BattleParticipant()
        {
            // Настройка ячеек героев
            CellHeroes = new Creature[FormMain.Config.HeroRows, FormMain.Config.HeroInRow];
        }

        internal string Name { get; set; }
        internal int ImageIndexAvatar { get; set; }
        internal TypePlayer TypePlayer { get; set; }
        internal bool BattleCalced { get; set; } = false;
        internal bool IsLive { get; set; } = true;/*private set*/
        internal List<Battle> HistoryBattles { get; } = new List<Battle>();
        internal List<Creature> CombatHeroes { get; } = new List<Creature>();

        // Основные параметры
        internal Creature[,] CellHeroes { get; private set; }

        internal virtual void PreparingForBattle()
        {
            RearrangeHeroes();
        }

        internal void AddCombatHero(Creature c)
        {
            CombatHeroes.Add(c);
            CombatHeroes.Sort(ComparePlaceCreature);
        }

        protected void RearrangeHeroes()
        {
            // Очищаем все координаты героев
            foreach (PlayerHero ph in CellHeroes)
            {
                if (ph != null)
                {
                    CellHeroes[ph.CoordInPlayer.Y, ph.CoordInPlayer.X] = null;
                    ph.CoordInPlayer = new Point(-1, -1);
                }
            }

            // Проставляем координаты для героев
            foreach (Creature ph in CombatHeroes.OrderBy(ph => ph.Priority()))
                SetPosForHero(ph);
        }

        private void SetPosForHero(Creature ph)
        {
            // Ищем место в ячейках героев
            int coordY = -1;
            int coordX = 0;
            List<int> positions = new List<int>();

            // Сначала ищем ячейку согласно категории героя
            // Для этого ищем линию со свободными ячейками для категории героя, начиная с первой
            // Пытаемся разместить его в середине линии, а затем в стороны от середины
            for (int x = CellHeroes.GetLength(1) - 1; x >= 0; x--)
            {
                coordX = x;
                positions.Clear();

                for (int y = 0; y < CellHeroes.GetLength(0); y++)
                    if (CellHeroes[y, x] == null)
                    {
                        positions.Add(y);
                    }

                if (positions.Count > 0)
                {
                    int centre = (int)Math.Truncate(CellHeroes.GetLength(0) / 2.0 + 0.5) - 1;
                    if (positions.IndexOf(centre) != -1)
                    {
                        coordY = centre;
                    }
                    else
                    {
                        int shift = 1;
                        for (; ; shift++)
                        {
                            if (positions.IndexOf(centre - shift) != -1)
                            {
                                coordY = centre - shift;
                                break;
                            }

                            if (positions.IndexOf(centre + shift) != -1)
                            {
                                coordY = centre + shift;
                                break;
                            }

                            if (shift == centre)
                                break;
                        }
                    }
                }

                if (coordY != -1)
                    break;

            }

            Debug.Assert(coordY != -1);
            Debug.Assert(CellHeroes[coordY, coordX] == null);

            CellHeroes[coordY, coordX] = ph;
            ph.CoordInPlayer = new Point(coordX, coordY);
        }
    }
}
