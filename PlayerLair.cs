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
            TypeLair = l;

            Level = 1;
            Name = l.Name;
            ImageIndexAvatar = l.ImageIndex;
            TypePlayer = TypePlayer.Lair;
            Hidden = true;

            // Убрать эту проверку после настройки всех логов
            if (TypeLair.LevelLairs.Count > 0)
                CreateMonsters();            
        }
        internal Player Player { get; }
        internal TypeLair TypeLair { get; }
        internal int Level { get; private set; }// Текущий уровень логова
        internal bool Hidden { get; set; }// Логово не разведано
        internal List<Monster> Monsters { get; } = new List<Monster>();// Монстры текущего уровня

        private void CreateMonsters()
        {
            Debug.Assert(TypeLair.LevelLairs.Count <= Level);
            Debug.Assert(TypeLair.LevelLairs[Level - 1].Monsters.Count > 0);

            Monster lm;
            foreach (MonsterLevelLair mll in TypeLair.LevelLairs[Level - 1].Monsters)
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

            return TypeLair.LevelLairs[Level - 1].Cost;
        }
        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeLair.Name, "", TypeLair.Description);
        }
    }
}
