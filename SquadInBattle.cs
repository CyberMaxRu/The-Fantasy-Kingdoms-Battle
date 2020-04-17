using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс юнита в бою
    internal sealed class UnitInBattle
    {
        public UnitInBattle(SquadInBattle s)
        {
            Squad = s;
            DamageMin = s.Squad.DamageMin;
            DamageMax = s.Squad.DamageMax;
            Health = s.Squad.Health;
            RestHealth = Health;
        }

        internal SquadInBattle Squad { get; }
        internal int DamageMin { get; }
        internal int DamageMax { get; }
        internal int Health { get; }
        internal int RestHealth { get; private set; }
        internal int Damaged { get; private set; }
        internal int DamageReceived { get; private set; }

        internal void DoDamage(UnitInBattle enemy, Random r)
        {
            // Юнит мог быть убит другим юнитом на этом же ходу
            if (enemy.RestHealth > 0)
            {
                int damage = r.Next(DamageMin, DamageMax);// Вычисляем нанесенный урон
                damage = enemy.GetDamage(damage);// Получаем, сколько фактически нанесли урона
                Damaged += damage;
                Squad.Damaged += damage;
                if (enemy.RestHealth == 0)
                    Squad.Killed++;
            }
        }

        internal int GetDamage(int damage)
        {
            Debug.Assert(RestHealth > 0);
            Debug.Assert(damage > 0);

            int dmg = Math.Min(RestHealth, damage);
            DamageReceived += dmg;
            RestHealth -= dmg;

            Squad.DamageReceived += dmg;
            if (RestHealth == 0)
            {
                Squad.Losses++;
                Squad.LostUnits.Add(this);
                Squad.UnitsAlive--;
            }

            return dmg;
        }
    }

    // Класс отряда в бою для расчета сражения
    internal sealed class SquadInBattle
    {
        public SquadInBattle(Squad s)
        {
            Squad = s;
            UnitsTotal = Config.ROWS_IN_SQUAD * Config.UNIT_IN_ROW;
            UnitsAlive = UnitsTotal;

            // Создаем юнитов
            Units = new UnitInBattle[Config.ROWS_IN_SQUAD, Config.UNIT_IN_ROW];

            for (int i = 0; i < Config.ROWS_IN_SQUAD; i++)
                for (int k = 0; k < Config.UNIT_IN_ROW; k++)
                {
                    Units[i, k] = new UnitInBattle(this);
                }
        }
        internal Squad Squad { get; }
        internal UnitInBattle[,] Units { get; }
        internal List<UnitInBattle> LostUnits = new List<UnitInBattle>();
        internal int UnitsTotal { get;}
        internal int UnitsAlive { get; set; }

        // Статистика отряда
        internal int Damaged { get; set; }// Нанесено повреждений
        internal int Killed { get; set; }// Убито врагов
        internal int DamageReceived { get; set; }// Получено повреждений
        internal int Losses { get; set; }// Потери

        internal void DoDamage(SquadInBattle enemy, Random r)
        {
            Debug.Assert(enemy != null);
            Debug.Assert(r != null);
            Debug.Assert(this != enemy);

            UnitInBattle ourUnit;
            UnitInBattle enemyUnit;

            for (int i = 0; i < Config.UNIT_IN_ROW; i++)
            {
                ourUnit = Units[0, i];
                // Если юнит есть, берем его
                if (ourUnit != null)
                {
                    enemyUnit = enemy.Units[0, (Config.UNIT_IN_ROW - 1) - i];
                    if (enemyUnit != null)
                    {
                        // Враг перед собой есть, наносим ему повреждение
                        ourUnit.DoDamage(enemyUnit, r);
                    }
                }
            }
        }

        // Убираем убитых юнитов
        internal void RemoveDied()
        {
            UnitInBattle unit;

            for (int i = 0; i < Config.UNIT_IN_ROW; i++)
            {
                unit = Units[0, i];
                if (unit != null)
                {
                    if (unit.RestHealth == 0)
                    {
                        Units[0, i] = null;

                        // Сдвигаем юниты вперед
                        for (int k = 1; k < Config.ROWS_IN_SQUAD; k++)
                        {
                            if (Units[k, i] == null)
                                break;

                            Units[k - 1, i] = Units[k, i];
                        }

                        Units[Config.ROWS_IN_SQUAD - 1, i] = null;
                    }
                }
            }
        }

        // Реструктуризирует живых юнитов отряда, размещая их заново по рядам
        internal void RearrangeSquad()
        {
            if (Losses > 0)
            {

            }
        }

        internal string GetName()
        {
            return Squad.Player.Name + "." + Squad.TypeUnit.Name;
        }
    }
}
