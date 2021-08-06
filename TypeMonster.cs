using System;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип монстра
    internal sealed class TypeMonster : TypeCreature
    {
        public TypeMonster(XmlNode n) : base(n)
        {
            // Проверяем, что таких же ID и наименования нет
            foreach (TypeMonster tm in FormMain.Config.TypeMonsters)
            {
                Debug.Assert(tm.ID != ID);
                Debug.Assert(tm.Name != Name);
                Debug.Assert(tm.ImageIndex != ImageIndex);
            }

            foreach (TypeHero th in FormMain.Config.TypeHeroes)
            {
                Debug.Assert(th.ID != ID);
            }

            // Загружаем награду
            TypeReward = new TypeReward(n.SelectSingleNode("Reward"));
        }

        internal TypeReward TypeReward { get; }// Награда за убийство существа
    }
}
