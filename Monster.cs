using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс монстра в логове
    internal sealed class Monster : ICell
    {
        private VCCell panelEntity;

        public Monster(TypeMonster m, int level)
        {
            Debug.Assert(m != null);
            Debug.Assert(level > 0);

            TypeMonster = m;

        }

        internal TypeMonster TypeMonster { get; }

        // Реализация интерфейса
        VCCell ICell.Panel
        {
            get => panelEntity;
            set
            {
                //if (value == null)
                //    Debug.Assert(panelEntity != null);
                //else
                //    Debug.Assert(panelEntity == null);

                panelEntity = value;
            }
        }
        BitmapList ICell.BitmapList() => Program.formMain.imListObjectsCell;
        int ICell.ImageIndex() => TypeMonster.ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Value() => 1;
        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(TypeMonster.Name, "", TypeMonster.Description);
        }

        void ICell.Click(VCCell pe)
        {
            //Program.formMain.SelectHero(this);
            //Program.formMain.SelectPanelEntity(pe);
        }
    }
}
