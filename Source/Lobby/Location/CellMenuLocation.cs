using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    abstract internal class CellMenuLocation : CellOfMenu
    {
        public CellMenuLocation(Location l, DescriptorCellMenu d) : base(l, d)
        {
            Location = l;
        }

        internal Location Location  { get; }

        internal override void Click()
        {
            if (CheckRequirements())
            {
                Program.formMain.PlayPushButton();
            }
        }
    }

    sealed internal class CellMenuLocationScout : CellMenuLocation
    {
        private readonly ListBaseResources cost = new ListBaseResources();

        public CellMenuLocationScout(Location l, DescriptorCellMenu d) : base(l, d)
        {
        }

        internal override string GetLevel() => "\u2026";// Троеточие
        internal override int GetQuantity() => Location.ComponentObjectOfMap.ListHeroesForFlag.Count;
        internal override string GetText() => "";

        internal override void Click()
        {
            Location.StateMenu = 1;
            Program.formMain.layerGame.UpdateMenu();
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = cost;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 184;
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

        internal override bool GetImageIsEnabled() => Location.Player.FreeHeroes.Count > 0;
        internal override int GetQuantity() => Location.Player.FreeHeroes.Count;
        internal override string GetText() => "";

        internal override void Click()
        {
            if (GetImageIsEnabled())
            {
                Location.StateMenu = 2;
                Program.formMain.layerGame.UpdateMenu();
            }
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = cost;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 289;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            if (GetImageIsEnabled())
                panelHint.AddSimpleHint("Нанять героев для разведки");
            else
                panelHint.AddSimpleHint("Нет свободных героев для найма");
        }
    }

    sealed internal class CellMenuLocationCancelScout : CellMenuLocation
    {
        public CellMenuLocationCancelScout(Location l, DescriptorCellMenu d) : base(l, d)
        {

        }

        internal override string GetText() => Location.PayForHire > 0 ? "+" + Location.PayForHire.ToString() : "";

        internal override void Click()
        {
            Location.DropFlagScout();
            Program.formMain.layerGame.UpdateMenu();
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 185;
        }

        internal override bool CheckRequirements()
        {
            return base.CheckRequirements() && (Location.ComponentObjectOfMap.ListHeroesForFlag.Count > 0);
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

        internal override void UpdatePurchase()
        {
            PurchaseValue = cost;
        }

        internal override int GetImageIndex()
        {
            return Config.ImageIndexFirstItems + 290;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddSimpleHint("Возврат из меню");
        }
    }

    internal sealed class CellMenuLocationSpell : CellMenuLocation
    {
        public CellMenuLocationSpell(Location forLocation, ConstructionSpell spell) : base(forLocation, new DescriptorCellMenu(spell.DescriptorSpell.Coord))
        {
            Spell = spell;
            Entity = spell.DescriptorSpell;
        }

        internal ConstructionSpell Spell { get; }
        internal DescriptorConstructionSpell Entity { get; }

        internal override string GetLevel() => "";

        internal override int GetImageIndex()
        {
            return Entity.ImageIndex;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Descriptor(Entity);
            panelHint.AddStep4Level($"Осталось: {Spell.Selling.RestQuantity}");
            panelHint.AddStep5Description(Entity.Description);
            panelHint.AddStep12Gold(Location.Player.BaseResources, PurchaseValue);
        }

        internal override void UpdatePurchase()
        {
            PurchaseValue = new ListBaseResources(Entity.Selling.Gold);
        }

        internal override void Click()
        {
            base.Click();
                
            switch (Entity.Action)
            {
                case ActionOfSpell.ScoutInLocation:
                    Assert(Location.ComponentObjectOfMap.Visible);
                    Location.DoScout(100);
                    Location.FindScoutedConstructions();
                    Spell.Selling.Use();

                    Assert(Location.MenuSpells.IndexOf(this) != -1);
                    Location.MenuSpells.Remove(this);
                    break;
                default:
                    DoException($"Неизвестное действие: {Entity.Action}");
                    break;
            }

            Program.formMain.layerGame.UpdateNeighborhoods();
            Location.Player.Lobby.Layer.UpdateMenu();
        }

        internal override bool CheckRequirements() => (Spell.Selling.RestQuantity > 0) && (base.CheckRequirements());

        internal override void PrepareNewDay()
        {
            base.PrepareNewDay();

            Spell.Selling.Reset();
        }
    }
}
