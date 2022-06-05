using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class RequirementGoods : DescriptorRequirement
    {
        private string nameConstruction;
        private string nameGoods;

        private DescriptorConstruction construction;
        private DescriptorProduct goods;

        public RequirementGoods(DescriptorWithID forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            nameGoods = XmlUtils.GetStringNotNull(n, "Goods");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(nameGoods.Length > 0);
        }

        internal override bool CheckRequirement(Player p) => p.CheatingIgnoreRequirements ? true : p.FindConstruction(construction.ID).GoodsAvailabled(goods);
        internal override TextRequirement GetTextRequirement(Player p) => new TextRequirement(CheckRequirement(p), $"{goods.Name} ({construction.Name})");

        internal override void TuneLinks()
        {
            base.TuneLinks();

            construction = Descriptors.FindConstruction(nameConstruction);
            goods = Descriptors.FindProduct(nameGoods);
            nameConstruction = "";
            nameGoods = "";

            bool founded = false;

            foreach (DescriptorCellMenu cm in construction.CellsMenu)
            {
                if (cm.IDCreatedEntity == goods.ID)
                {
                    founded = true;
                    break;
                }
            }

            //if (ForCellMenu is DescriptorCellMenuForConstruction cmc)
            //    Debug.Assert(goods.ID != cmc.NameEntity, $"Товар {goods.ID} требует сам себя.");
            //Debug.Assert(founded, $"Товар {goods.ID} не найден в {construction.ID}.");

            //goods.UseForResearch.Add(ForCellMenu);
        }
    }
}
