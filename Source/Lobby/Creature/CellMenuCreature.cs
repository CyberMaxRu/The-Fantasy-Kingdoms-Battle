using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal enum ModeTextForCreature { Hire, Scout, };

    sealed internal class CellMenuCreature : CellOfMenu
    {
        public CellMenuCreature(BigEntity forEntity, DescriptorCellMenu d) : base(forEntity, d)
        {
        }

        internal ModeTextForCreature ModeText { get; set; }
        internal EventHandler OnClick { get; set; }

        internal Creature Creature { get; set; }
        internal override string GetLevel() => Creature != null ? Creature.GetLevel() : "";
        internal override string GetDaysExecuting()
        {
            switch (ModeText)
            {
                case ModeTextForCreature.Hire:
                    if (BigEntity is Location l)
                        return Creature != null ? Utils.FormatPercent((Creature as Creature).CalcPercentScoutArea(l)).ToString() : "";
                    else
                        return "";
                case ModeTextForCreature.Scout:
                    return Creature != null ? Utils.FormatPercent(Creature.PercentLocationForScout) : "";
                default:
                    return "";
            }
        }

        internal override string GetText()
        {
            switch (ModeText)
            {
                case ModeTextForCreature.Hire:
                    if ((Creature != null) && (Creature is Creature h))
                        return h.CostOfHiring().ToString();
                    else
                        return "";
                case ModeTextForCreature.Scout:
                    if ((Creature != null) && (Creature is Creature h2))
                        return h2.PayForHire.ToString();
                    else
                        return "";
                default:
                    return "";
            }
        }

        internal override void Click()
        {
            Utils.Assert(Creature != null); 

            OnClick?.Invoke(this, EventArgs.Empty);
        }

        internal override void Execute()
        {
            throw new NotImplementedException();
        }

        internal override int GetImageIndex()
        {
            return Creature != null ? Creature.GetImageIndex() : -1;// Creature пропадает при клике на герое
        }

        internal override bool InstantExecute()
        {
            throw new NotImplementedException();
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            if (BigEntity is Location l)
                panelHint.AddSimpleHint($"Разведка: {Utils.FormatPercent(Creature.CalcPercentScoutArea(l))}");
        }
    }

    sealed internal class CellMenuCreaturePage : CellOfMenu
    {
        private ListBaseResources resources;
        private int quantityPerPage = FormMain.Config.PlateWidth * (FormMain.Config.PlateHeight - 1);

        public CellMenuCreaturePage(BigEntity forEntity, DescriptorCellMenu d) : base(forEntity, d)
        {
            resources = new ListBaseResources();
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

        internal override void Execute()
        {
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = resources;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 291;
        }

        internal override bool InstantExecute()
        {
            throw new NotImplementedException();
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
