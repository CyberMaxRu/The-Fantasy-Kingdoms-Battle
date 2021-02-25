using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс логова игрока
    internal sealed class PlayerLair : BattleParticipant
    {
        public PlayerLair(Player p, TypeLair l) : base()
        {
            Player = p;
            Lair = l;

            Level = 1;
            Name = l.Name;
            ImageIndexAvatar = l.ImageIndex;
            TypePlayer = TypePlayer.Lair;

            // Убрать эту проверку после настройки всех логов
            if (Lair.LevelLairs.Count > 0)
                CreateMonsters();            
        }
        internal Player Player { get; }
        internal TypeLair Lair { get; }
        internal int Level { get; private set; }// Текущий уровень логова
        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня

        private void CreateMonsters()
        {
            Debug.Assert(Lair.LevelLairs.Count <= Level);
            Debug.Assert(Lair.LevelLairs[Level - 1].Monsters.Count > 0);

            Monster lm;
            foreach (MonsterLevelLair mll in Lair.LevelLairs[Level - 1].Monsters)
            {
                for (int i = 0; i < mll.StartQuantity; i++)
                {
                    lm = new Monster(mll.Monster, mll.Level, this);
                    Monsters.Add(lm);
                    CombatHeroes.Add(lm);
                }
            }
        }

        internal override void PreparingForBattle()
        {
            base.PreparingForBattle();
        }

        internal int CostAttack()
        {
            Debug.Assert(Level > 0);

            return Lair.LevelLairs[Level - 1].Cost;
        }
    }
}
