using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс логова игрока
    internal sealed class PlayerLair
    {
        public PlayerLair(Player p, TypeLair l)
        {
            Player = p;
            Lair = l;

            Level = 1;

            // Убрать эту проверку после настройки всех логов
            if (Lair.LevelLairs.Count > 0)
                CreateMonsters();            
        }
        internal Player Player { get; }
        internal TypeLair Lair { get; }
        internal int Level { get; private set; }// Текущий уровень логова
        internal List<LairMonster> Monsters { get; } = new List<LairMonster>();// Монстры текущего уровня

        private void CreateMonsters()
        {
            Debug.Assert(Lair.LevelLairs.Count <= Level);
            Debug.Assert(Lair.LevelLairs[Level - 1].Monsters.Count > 0);

            LairMonster lm;
            foreach (MonsterLevelLair mll in Lair.LevelLairs[Level - 1].Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new LairMonster(mll.Monster, mll.Level);
                    Monsters.Add(lm);
                }
            }
        }
    }
}
