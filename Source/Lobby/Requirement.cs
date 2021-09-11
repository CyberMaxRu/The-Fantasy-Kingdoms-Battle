using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс требования
    internal sealed class Requirement : Descriptor
    {
        private string nameConstruction;

        public Requirement(XmlNode n) : base()
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            Level = XmlUtils.GetInteger(n, "Level");
            Destroyed = XmlUtils.GetInteger(n, "Destroyed");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(Level >= 0);
            Debug.Assert(Destroyed >= 0);
        }

        internal DescriptorConstruction Construction { get; private set; }
        internal int Level { get; }
        internal int Destroyed { get; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Construction = Config.FindConstruction(nameConstruction);
            nameConstruction = "";

            if (Construction.IsOurConstruction)
            {
                Debug.Assert(Level <= Construction.MaxLevel, $"Требуется сооружение {Construction.ID} {Level} уровня, но у него максимум {Construction.MaxLevel} уровень.");
                Debug.Assert(Destroyed == 0);
            }
            else
            {
                Debug.Assert(Level == 0);
                Debug.Assert(Destroyed > 0);
            }
        }
    }
}