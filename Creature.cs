using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс существа
    internal abstract class Creature : ICell
    {
        private VCCell panelEntity;

        public Creature(KindCreature kc)
        {
            KindCreature = kc;
        }

        internal KindCreature KindCreature { get; }
        internal int Level { get; private set; }// Уровень

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
        int ICell.ImageIndex() => -1;// ClassHero.ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Value() => Level;
        void ICell.PrepareHint()
        {
            //Program.formMain.formHint.AddStep1Header(ClassHero.Name, "", ClassHero.Description);
        }

        void ICell.Click(VCCell pe)
        {
            //Program.formMain.SelectHero(this);
            Program.formMain.SelectPanelEntity(pe);
        }
    }
}
