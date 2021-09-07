using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс требования
    internal sealed class Requirement
    {
        private string nameConstruction;

        public Requirement(string nameRequiredConstruction, int level)
        {
            Debug.Assert(nameRequiredConstruction.Length > 0);
            Debug.Assert(level >= 0);

            nameConstruction = nameRequiredConstruction;
            Level = level;
        }

        internal DescriptorConstruction Construction { get; private set; }
        internal int Level { get; }

        internal void FindConstruction()
        {
            Construction = FormMain.Config.FindTypeConstruction(nameConstruction);
            nameConstruction = "";

            Debug.Assert(Level <= Construction.MaxLevel);
        }
    }
}