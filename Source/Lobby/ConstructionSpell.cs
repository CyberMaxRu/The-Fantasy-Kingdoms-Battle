using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionSpell : EntityForConstruction
    {
        public ConstructionSpell(BigEntity entity, DescriptorProduct cp, DescriptorConstructionSpell ds) : base(entity, ds)
        {
            DescriptorSpell = ds;
            Product = cp;
        }

        internal DescriptorConstructionSpell DescriptorSpell { get; }
        internal DescriptorProduct Product { get; }
        internal bool Enabled { get; set; } = true;// Товар доступен для продажи// !!! Делать через 0 В количестве?!!!

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal int GetCostGold()
        {
            return Product.Selling.Gold;
        }

        internal override bool GetNormalImage() => Enabled;
        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Header(Descriptor.Name, GetImageIndex());
            panelHint.AddStep3Type(Descriptor.GetTypeEntity());
            panelHint.AddStep5Description(Descriptor.Description);
            panelHint.AddStep4Level($"Осталось: {Selling.RestQuantity}");
            panelHint.AddStep10CostGold(GetCostGold());
        }
    }
}
