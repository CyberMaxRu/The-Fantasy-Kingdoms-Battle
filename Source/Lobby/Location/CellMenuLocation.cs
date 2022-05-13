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
    }

    sealed internal class CellMenuLocationAddScoutHero : CellMenuLocation
    {
        private readonly ListBaseResources cost = new ListBaseResources();

        public CellMenuLocationAddScoutHero(Location l, DescriptorCellMenu d) : base(l, d)
        {

        }

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
    }

    sealed internal class CellMenuLocationCancelScout : CellMenuLocation
    {
        private readonly ListBaseResources cost = new ListBaseResources();

        public CellMenuLocationCancelScout(Location l, DescriptorCellMenu d) : base(l, d)
        {

        }

        internal override void Click()
        {
            Location.StateMenu = 0;
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
            return Config.ImageIndexFirstItems + 185;
        }

        internal override bool InstantExecute()
        {
            throw new NotImplementedException();
        }
    }
}
