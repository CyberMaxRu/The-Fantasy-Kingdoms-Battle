using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum LevelSecondSkill { Base, Advanced, Master, Expert };

    // Класс параметра вторичного навыка
    internal sealed class ParamSecondSkill
    {
        public ParamSecondSkill(XmlNode n)
        {
            Level = XmlUtils.GetInteger(n, "Number");
            Health = XmlUtils.GetInteger(n, "Health");
            Income = XmlUtils.GetInteger(n, "Income");
        }

        internal int Level { get; }
        internal int Health { get; }
        internal int SpeedMove { get; }

        internal int Income { get; }
    }

    // Класс вторичного навыка
    internal sealed class TypeSecondarySkill : TypeEntity
    {
        public TypeSecondarySkill(XmlNode n) : base(n)
        {

            // Загружаем параметры
            XmlNode ne = n.SelectSingleNode("Levels");
            ParamSecondSkill p;

            foreach (XmlNode l in ne.SelectNodes("Level"))
            {
                p = new ParamSecondSkill(l);

                Debug.Assert(Levels[p.Level] is null);
                Levels[p.Level] = p;
            }

            for (int i = 0; i < Levels.Length; i++)
                Debug.Assert(!(Levels[i] is null));

        }

        internal ParamSecondSkill[] Levels { get; } = new ParamSecondSkill[(int)LevelSecondSkill.Expert + 1];

        internal override void TuneDeferredLinks()
        {

        }

        /*protected override void DoPrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Name, "Начальный уровень", Description);
        }*/

        /*protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost() => "баз.";
        */
    }
}
