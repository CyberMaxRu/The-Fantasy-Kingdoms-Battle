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
    internal sealed class RewardForLair
    {
        public RewardForLair(XmlNode n)
        {
            Gold = XmlUtils.GetInteger(n.SelectSingleNode("Gold"));
            Greatness = XmlUtils.GetInteger(n.SelectSingleNode("Greatness"));
            PointTradePost = XmlUtils.GetInteger(n.SelectSingleNode("PointTradePost"));
            PointTemple = XmlUtils.GetInteger(n.SelectSingleNode("PointTemple"));
            
            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 50_000);
            Debug.Assert(Greatness >= 0);
            Debug.Assert(Greatness <= 10_000);
            Debug.Assert(PointTradePost >= 0);
            Debug.Assert(PointTradePost <= 1);
            Debug.Assert(PointTemple >= 0);
            Debug.Assert(PointTemple <= 1);
        }

        internal int Gold { get; }
        internal int Greatness { get; }
        internal int PointTemple { get; }
        internal int PointTradePost { get; }
    }

    // Класс типа логова монстров
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

            // Информация о монстрах
            XmlNode ne = n.SelectSingleNode("Monsters");
            if (ne != null)
            {
                MonsterLevelLair mll;
                foreach (XmlNode l in ne.SelectNodes("Monster"))
                {
                    mll = new MonsterLevelLair(l);
                    Monsters.Add(mll);
                }
            }

            MaxHeroes = XmlUtils.GetInteger(n.SelectSingleNode("MaxHeroes"));
            IsLair = XmlUtils.GetBoolNotNull(n.SelectSingleNode("IsLair"));
            // Информация о награде
            if (n.SelectSingleNode("Reward") != null)
                Reward = new RewardForLair(n.SelectSingleNode("Reward"));

            Debug.Assert(MaxHeroes < 50);

            if (IsLair)
            {
                Debug.Assert(MaxHeroes > 0);
            }
            else
            {
                Debug.Assert(MaxHeroes == 0);
            }

            //else
            //    throw new Exception("В конфигурации логова у " + ID + " нет информации об уровнях. ");
        }

        internal List<MonsterLevelLair> Monsters { get; } = new List<MonsterLevelLair>();
        internal int MaxHeroes { get; }// Максимальное количество героев, которое может атаковать логово
        internal bool IsLair { get; }// Признак - это логово
        internal RewardForLair Reward { get; }// Награда за зачистку логова

        internal override void TuneDeferredLinks()
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
}