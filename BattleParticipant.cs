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

        public BattleParticipant(Lobby lobby)
        {
            Lobby = lobby;
        }

        internal Lobby Lobby { get; }
        internal bool BattleCalced { get; set; } = false;
        internal bool IsLive { get; set; } = true;/*private set*/
        internal List<Battle> HistoryBattles { get; } = new List<Battle>();
        internal List<Creature> CombatHeroes { get; } = new List<Creature>();

        internal virtual void PreparingForBattle()
        {
        }

        internal void AddCombatHero(Creature c)
        {
            Debug.Assert(c != null);
            Debug.Assert(CombatHeroes.IndexOf(c) == -1);
            Debug.Assert(c.IsLive);

            CombatHeroes.Add(c);
            CombatHeroes.Sort(ComparePlaceCreature);
        }

        internal void ArrangeHeroes(List<HeroInBattle> list)
        {
            HeroInBattle[,] cells = new HeroInBattle[FormMain.Config.HeroInRow, FormMain.Config.HeroRows];

            // Проставляем координаты для героев
            foreach (HeroInBattle ph in list.OrderBy(ph => ph.PlayerHero.Priority()))
            {
                Debug.Assert(ph.Player == this);

                SetPosForHero(ph, cells);
            }
        }

        private void SetPosForHero(HeroInBattle ph, HeroInBattle[,] cells)
        {
            Debug.Assert(ph.IsLive);

            // Ищем место в ячейках героев
            int coordY = -1;
            int coordX = 0;
            List<int> positions = new List<int>();

            // Сначала ищем ячейку согласно категории героя
            // Для этого ищем линию со свободными ячейками для категории героя, начиная с первой
            // Пытаемся разместить его в середине линии, а затем в стороны от середины
            for (int x = cells.GetLength(1) - 1; x >= 0; x--)
            {
                coordX = x;
                positions.Clear();

                for (int y = 0; y < cells.GetLength(0); y++)
                    if (cells[y, x] == null)
                    {
                        positions.Add(y);
                    }

                if (positions.Count > 0)
                {
                    int centre = (int)Math.Truncate(cells.GetLength(0) / 2.0 + 0.5) - 1;
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
            Debug.Assert(cells[coordY, coordX] == null);

            cells[coordY, coordX] = ph;
            ph.StartCoord = new Point(coordX, coordY);
        }

        internal abstract string GetName();
        internal abstract TypePlayer GetTypePlayer();
        internal abstract LobbyPlayer GetPlayer();
        internal abstract int GetImageIndexAvatar();
    }
}
