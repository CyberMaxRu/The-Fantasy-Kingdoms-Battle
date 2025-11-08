using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{

    sealed internal class CellMenuCreature : ActionForEntity
    {
        public CellMenuCreature(BigEntity forEntity, DescriptorActionForEntity d) : base(forEntity, d)
        {
        }

        internal EventHandler OnClick { get; set; }

        internal Creature Creature { get; set; }
        internal override string GetLevel() => Creature != null ? Creature.GetLevel() : "";

        internal override string GetText()
        {
            if ((Creature != null) && (Creature is Creature h))
                return h.CostOfHiring().ToString();
            else
                return "";
        }

        internal override void Click()
        {
            Utils.Assert(Creature != null); 

            OnClick?.Invoke(this, EventArgs.Empty);
        }

        internal override int GetImageIndex()
        {
            return Creature != null ? Creature.GetImageIndex() : -1;// Creature пропадает при клике на герое
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
        }
    }

    sealed internal class CellMenuCreaturePage : ActionForEntity
    {
        private int quantityPerPage = FormMain.Config.PlateWidth * (FormMain.Config.PlateHeight - 1);

        public CellMenuCreaturePage(BigEntity forEntity, DescriptorActionForEntity d) : base(forEntity, d)
        {
            //resources = new ListBaseResources();
        }

        internal int Pages { get; private set; }
        internal int CurrentPage { get; private set; }
        internal bool ChangePage { get; set; }

        internal override void Click()
        {
            if (Pages > 0)
            {
                CurrentPage++;
                if (CurrentPage >= Pages)
                    CurrentPage = 0;

                ChangePage = true;
                Program.formMain.layerGame.UpdateMenu();
            }
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 291;
        }

        internal void SetQuantity(int quantity)
        {
            ChangePage = false;

            if (quantity > 0)
            {
                Pages = quantity / quantityPerPage;
                if (quantity % quantityPerPage > 0)
                    Pages++;
                CurrentPage = 0;
            }
            else
                Pages = 1;
        }

        internal override string GetText() => $"{CurrentPage + 1}/{Pages}";
    }
}
