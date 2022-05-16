using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    abstract internal class CellMenuLocation : CellOfMenu
    {
        public CellMenuLocation(Location l, DescriptorCellMenu d) : base(l, d)
        {
            Location = l;
        }

        internal Location Location  { get; }
    }

    sealed internal class CellMenuLocationScout : CellMenuLocation
    {
        private readonly ListBaseResources cost = new ListBaseResources();

        public CellMenuLocationScout(Location l, DescriptorCellMenu d) : base(l, d)
        {
        }

        internal override string GetText() => Location.HeroesForScout.Count.ToString();

        internal override void Click()
        {
            Location.StateMenu = 1;
            Program.formMain.layerGame.UpdateMenu();
        }

        internal override void Execute()
        {
        }

        internal override ListBaseResources GetCost()
        {
            return cost;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 184;
        }

        internal override bool InstantExecute()
        {
            throw new NotImplementedException();
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddSimpleHint("Информация о задании разведки");
        }
    }

    sealed internal class CellMenuLocationAddScoutHero : CellMenuLocation
    {
        private readonly ListBaseResources cost = new ListBaseResources();

        public CellMenuLocationAddScoutHero(Location l, DescriptorCellMenu d) : base(l, d)
        {

        }

        internal override string GetText() => "";

        internal override void Click()
        {
            Location.StateMenu = 2;
            Program.formMain.layerGame.UpdateMenu();
        }

        internal override void Execute()
        {
        }

        internal override ListBaseResources GetCost()
        {
            return cost;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 289;
        }

        internal override bool InstantExecute()
        {
            throw new NotImplementedException();
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddSimpleHint("Нанять героев для разведки");
        }
    }

    sealed internal class CellMenuLocationCancelScout : CellMenuLocation
    {
        public CellMenuLocationCancelScout(Location l, DescriptorCellMenu d) : base(l, d)
        {

        }

        internal override string GetText() => "+" + Location.PayForHire.ToString();

        internal override void Click()
        {
            Location.DropFlagScout();
            Program.formMain.layerGame.UpdateMenu();
        }

        internal override void Execute()
        {
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 185;
        }

        internal override bool CheckRequirements()
        {
            return base.CheckRequirements() && (Location.HeroesForScout.Count > 0);
        }

        internal override bool InstantExecute()
        {
            return true;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddSimpleHint("Распустить героев, нанятых на разведку");
        }
    }

    sealed internal class CellMenuLocationReturn : CellMenuLocation
    {
        private readonly ListBaseResources cost = new ListBaseResources();

        public CellMenuLocationReturn(Location l, DescriptorCellMenu d) : base(l, d)
        {

        }

        internal override string GetText() => "";

        internal override void Click()
        {
            Location.StateMenu--;
            Program.formMain.layerGame.UpdateMenu();
        }

        internal override void Execute()
        {
        }

        internal override ListBaseResources GetCost()
        {
            return cost;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 290;
        }

        internal override bool InstantExecute()
        {
            return true;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddSimpleHint("Возврат из меню");
        }
    }
}
