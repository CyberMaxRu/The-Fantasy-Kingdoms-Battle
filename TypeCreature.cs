using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Базовый тип существа
    internal abstract class TypeCreature : Object
    {
        public TypeCreature(XmlNode n) : base(n)
        {
            KindCreature = FormMain.Config.FindKindCreature(XmlUtils.GetStringNotNull(n.SelectSingleNode("KindCreature")));
            MaxLevel = XmlUtils.GetInteger(n.SelectSingleNode("MaxLevel"));
            DefaultPositionPriority = XmlUtils.GetInteger(n.SelectSingleNode("DefaultPositionPriority"));
            Reward = XmlUtils.GetInteger(n.SelectSingleNode("Reward"));

            Debug.Assert(MaxLevel >= 1);
            Debug.Assert(MaxLevel <= 100);
            Debug.Assert(DefaultPositionPriority >= 0);
            Debug.Assert(DefaultPositionPriority <= 1000);
            Debug.Assert(Reward > 0);
            Debug.Assert(Reward <= 1000);

            // Загружаем основные параметры
            if (n.SelectSingleNode("BaseParameters") != null)
            {
                ParametersByHire = new HeroParameters(n.SelectSingleNode("BaseParameters"));

                //
                if (n.SelectSingleNode("NextLevel") != null)
                    ConfigNextLevel = new ConfigNextLevelHero(n.SelectSingleNode("NextLevel"));
            }

            // Загружаем дефолтные способности
            XmlNode na = n.SelectSingleNode("Abilities");
            if (na != null)
            {
                Ability a;

                foreach (XmlNode l in na.SelectNodes("Ability"))
                {
                    a = FormMain.Config.FindAbility(l.InnerText);

                    // Проверяем, что такая способность не повторяется
                    foreach (Ability a2 in Abilities)
                    {
                        if (a.ID == a2.ID)
                            throw new Exception("Способность " + a.ID + " повторяется в списке способностей героя.");
                    }

                    Abilities.Add(a);
                }
            }
        }

        internal KindCreature KindCreature { get; }// Вид существа
        internal int Reward { get; }// Награда за убийство существа
        internal int MaxLevel { get; }// Максимальный уровень существа
        internal HeroParameters ParametersByHire { get; }// Параметры при создании существа
        internal ConfigNextLevelHero ConfigNextLevel { get; }
        internal List<Ability> Abilities { get; } = new List<Ability>();// Способности существа
        internal int DefaultPositionPriority { get; private set; }// Приоритет расположения на поле боя по умолчанию
    }
}