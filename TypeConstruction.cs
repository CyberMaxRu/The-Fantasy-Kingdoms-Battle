using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Тип сооружения - базовый класс для всех зданий, построек и логов
    internal abstract class TypeConstruction
    {
        public TypeConstruction(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n.SelectSingleNode("ID"));
            Name = XmlUtils.GetStringNotNull(n.SelectSingleNode("Name"));
            Description = XmlUtils.GetDescription(n.SelectSingleNode("Description"));
            ImageIndex = XmlUtils.GetInteger(n.SelectSingleNode("ImageIndex"));

            DefaultLevel = XmlUtils.GetInteger(n.SelectSingleNode("DefaultLevel"));
            MaxLevel = Convert.ToInt32(n.SelectSingleNode("MaxLevel").InnerText);
            Line = XmlUtils.GetInteger(n.SelectSingleNode("Line"));

            Debug.Assert(DefaultLevel >= 0);
            Debug.Assert(DefaultLevel <= 5);
            Debug.Assert(MaxLevel > 0);
            Debug.Assert(MaxLevel <= 5);
            Debug.Assert(DefaultLevel <= MaxLevel);
            Debug.Assert(Line >= 1);
            Debug.Assert(Line <= 3);
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }

        internal int DefaultLevel { get; }
        internal int MaxLevel { get; }
        internal int Line { get; }

        internal virtual void TuneDeferredLinks()
        {

        }
    }
}
