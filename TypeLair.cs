using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс монстров уровня логова
    internal sealed class MonsterLevelLair
    {
        private string idMonster;

        public MonsterLevelLair(XmlNode n)
        {
            idMonster = n.SelectSingleNode("ID").InnerText;
            StartQuantity = XmlUtils.GetInteger(n.SelectSingleNode("StartQuantity"));
            MaxQuantity = XmlUtils.GetInteger(n.SelectSingleNode("MaxQuantity"));
            Level = XmlUtils.GetInteger(n.SelectSingleNode("Level"));
            DaysRespawn = XmlUtils.GetInteger(n.SelectSingleNode("DaysRespawn"));
            QuantityRespawn = XmlUtils.GetInteger(n.SelectSingleNode("QuantityRespawn"));

            Debug.Assert(idMonster.Length > 0);
            Debug.Assert(StartQuantity >= 0);
            Debug.Assert(MaxQuantity > 0);
            Debug.Assert(StartQuantity <= MaxQuantity);
            Debug.Assert(Level > 0);
            Debug.Assert(DaysRespawn >= 0);
            Debug.Assert(DaysRespawn <= 25);
            Debug.Assert(QuantityRespawn >= 0);
            //Debug.Assert(QuantityRespawn <= 49);
        }

        internal TypeMonster Monster { get; private set; }
        internal int StartQuantity { get; }
        internal int MaxQuantity { get; }
        internal int Level { get; }
        internal int DaysRespawn { get; }
        internal int QuantityRespawn { get; }
        internal List<Monster> Monsters { get; } = new List<Monster>();

        internal void TuneDeferredLinks()
        {
            Monster = FormMain.Config.FindTypeMonster(idMonster);
            idMonster = null;
            Debug.Assert(Level <= Monster.MaxLevel);
        }
    }

    // Класс награды за зачистку уровень логова
    internal sealed class RewardLevelLair
    {
        public RewardLevelLair(XmlNode n)
        {
            MinGold = XmlUtils.GetInteger(n.SelectSingleNode("MinGold"));
            MaxGold = XmlUtils.GetInteger(n.SelectSingleNode("MaxGold"));

            Debug.Assert(MinGold >= 0);
            Debug.Assert(MinGold <= 50_000);
            Debug.Assert(MaxGold >= 0);
            Debug.Assert(MaxGold <= 50_000);
            Debug.Assert(MinGold <= MaxGold);
        }

        internal int MinGold { get; }
        internal int MaxGold { get; }
    }

    // Класс уровня логова
    internal sealed class LevelLair
    {
        public LevelLair(XmlNode n)
        {
            // Информация о монстрах
            MonsterLevelLair mll;

            foreach (XmlNode l in n.SelectNodes("Monster"))
            {
                mll = new MonsterLevelLair(l);
                Monsters.Add(mll);
            }

            Cost = XmlUtils.GetInteger(n.SelectSingleNode("Cost"));
            // Информация о награде
            if (n.SelectSingleNode("Reward") != null)
                Reward = new RewardLevelLair(n.SelectSingleNode("Reward"));

            Debug.Assert(Cost >= 0);
            Debug.Assert(Cost < 100_000);
        }

        internal List<MonsterLevelLair> Monsters { get; } = new List<MonsterLevelLair>();
        internal int Cost { get; }
        internal RewardLevelLair Reward { get; }

        internal void TuneDeferredLinks()
        {
            foreach (MonsterLevelLair mll in Monsters)
            {
                mll.TuneDeferredLinks();

                // Проверяем, что тип монстра не повторяется
                foreach (MonsterLevelLair mlev in Monsters)
                    if ((mlev != mll) && (mlev.Monster != null))
                        if (mlev.Monster == mll.Monster)
                            throw new Exception("Тип монстра " + mll.Monster.ID + " повторяется.");
            }
        }
    }

    // Класс логова монстров
    internal sealed class TypeLair : TypeMapObject
    {
        public TypeLair(XmlNode n) : base(n)
        {
            // Проверяем, что таких же ID и наименования нет
            foreach (TypeLair tl in FormMain.Config.TypeLairs)
            {
                Debug.Assert(tl.ID != ID);
                Debug.Assert(tl.Name != Name);
                Debug.Assert(tl.ImageIndex != ImageIndex);
            }

            // Загружаем информацию об уровнях
            XmlNode nl = n.SelectSingleNode("Levels");
            if (nl != null)
            {
                LevelLair level;

                foreach (XmlNode l in nl.SelectNodes("Level"))
                {
                    level = new LevelLair(l);
                    LevelLairs.Add(level);
                }
            }
            //else
            //    throw new Exception("В конфигурации логова у " + ID + " нет информации об уровнях. ");
        }

        internal List<LevelLair> LevelLairs { get; } = new List<LevelLair>();

        internal override void TuneDeferredLinks()
        {
            foreach (LevelLair ll in LevelLairs)
            {
                ll.TuneDeferredLinks();
            }
        }
    }
}