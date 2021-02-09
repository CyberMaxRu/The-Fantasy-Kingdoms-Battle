using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    // Класс монстра в логове
    internal sealed class LairMonster : ICell
    {
        private PanelEntity panelEntity;

        public LairMonster(TypeMonster m, int level)
        {
            Debug.Assert(m != null);
            Debug.Assert(level > 0);

            Monster = m;

        }

        internal TypeMonster Monster { get; }

        // Реализация интерфейса
        PanelEntity ICell.Panel
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
        int ICell.ImageIndex() => Monster.ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Value() => 1;
        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Monster.Name, "", Monster.Description);
        }

        void ICell.Click(PanelEntity pe)
        {
            //Program.formMain.SelectHero(this);
            //Program.formMain.SelectPanelEntity(pe);
        }
    }
}
