using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс требования
    internal sealed class Requirement : Descriptor
    {
        private string nameConstruction;

        public Requirement(string nameRequiredConstruction, int level) : base()
        {
            Debug.Assert(nameRequiredConstruction.Length > 0);
            Debug.Assert(level >= 0);

            nameConstruction = nameRequiredConstruction;
            Level = level;
        }

        internal DescriptorConstruction Construction { get; private set; }
        internal int Level { get; }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Construction = Config.FindConstruction(nameConstruction);
            nameConstruction = "";

            Debug.Assert(Level <= Construction.MaxLevel, $"Требуется сооружение {Construction.ID} {Level} уровня, но у него максимум {Construction.MaxLevel} уровень.");
        }
    }
}