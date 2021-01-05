using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Базовый класс для всех юнитов
    internal class Unit : Object
    {
        public Unit(XmlNode n) : base(n)
        {
            TypeUnit = FormMain.Config.FindTypeUnit(n.SelectSingleNode("TypeUnit").InnerText);
            MaxLevel = Convert.ToInt32(n.SelectSingleNode("MaxLevel").InnerText);
            DefaultPositionPriority = XmlUtils.GetParamFromXmlInteger(n.SelectSingleNode("DefaultPositionPriority"));

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

        internal TypeUnit TypeUnit { get; }
        internal int MaxLevel { get; }
        internal HeroParameters ParametersByHire { get; }// Параметры при создании юнита
        internal ConfigNextLevelHero ConfigNextLevel { get; }
        internal List<Ability> Abilities { get; } = new List<Ability>();// Способности юнита
        internal int DefaultPositionPriority { get; private set; }// Приоритет расположения на поле боя по умолчанию
    }
}