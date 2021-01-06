using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Базовый класс существа
    internal abstract class Creature : ICell
    {
        private PanelEntity panelEntity;

        public Creature(TypeUnit tc)
        {
            TypeCreature = tc;
        }

        internal TypeUnit TypeCreature { get; }
        internal int Level { get; private set; }// Уровень

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
        ImageList ICell.ImageList() => Program.formMain.ilGuiHeroes;
        int ICell.ImageIndex() => -1;// ClassHero.ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Value() => Level;
        void ICell.PrepareHint()
        {
            //Program.formMain.formHint.AddStep1Header(ClassHero.Name, "", ClassHero.Description);
        }

        void ICell.Click(PanelEntity pe)
        {
            //Program.formMain.SelectHero(this);
            Program.formMain.SelectPanelEntity(pe);
        }
    }
}
