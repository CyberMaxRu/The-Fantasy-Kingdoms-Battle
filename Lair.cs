using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс монстров уровня логова
    internal sealed class MonsterLevelLair
    {
        private string idMonster;

        public MonsterLevelLair(XmlNode n)
        {
            idMonster = n.SelectSingleNode("ID").InnerText;
            StartQuantity = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("StartQuantity"));
            MaxQuantity = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("MaxQuantity"));
            Level = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("Level"));
            DaysRespawn = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("DaysRespawn"));
            QuantityRespawn = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("QuantityRespawn"));

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

        internal Monster Monster { get; private set; }
        internal int StartQuantity { get; }
        internal int MaxQuantity { get; }
        internal int Level { get; }
        internal int DaysRespawn { get; }
        internal int QuantityRespawn { get; }
        internal List<LairMonster> Monsters { get; } = new List<LairMonster>();

        internal void SetDeferredLink()
        {
            Monster = FormMain.Config.FindMonster(idMonster);
            idMonster = null;
            Debug.Assert(Level <= Monster.MaxLevel);

            // Создаем монстров
            /*LairMonster m;
            for (int i = 0; i < StartQuantity; i++)
            {
                m = new LairMonster(Monster, Level);
                Monsters.Add(m);
            }*/
        }
    }

    // Класс награды за зачистку уровень логова
    internal sealed class RewardLevelLair
    {
        public RewardLevelLair(XmlNode n)
        {
            Gold = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("Gold"));

            Debug.Assert(Gold >= 0);
            Debug.Assert(Gold <= 50_000);
        }

        internal int Gold { get; }
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
                MonsterLevelLairs.Add(mll);
            }
            // Информация о награде
        }

        internal List<MonsterLevelLair> MonsterLevelLairs { get; } = new List<MonsterLevelLair>();
        internal List<RewardLevelLair> RewardLevelLairs { get; } = new List<RewardLevelLair>();
    }

    // Класс логова монстров
    internal sealed class Lair
    {
        public Lair(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = n.SelectSingleNode("Description").InnerText.Replace("/", Environment.NewLine);
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Line = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("Line"));

            Debug.Assert(Line >= 1);
            Debug.Assert(Line <= 3);

            // Проверяем, что таких же ID и наименования нет
            foreach (Lair l in FormMain.Config.Lairs)
            {
                if (l.ID == ID)
                    throw new Exception("В конфигурации логов повторяется ID = " + ID);

                if (l.Name == Name)
                    throw new Exception("В конфигурации логов повторяется Name = " + Name);

                if (l.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации логов повторяется ImageIndex = " + ImageIndex.ToString());
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

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
        internal int Line { get; }
        internal List<LevelLair> LevelLairs { get; } = new List<LevelLair>();
        internal PanelLair Panel { get; set; }

    }
}