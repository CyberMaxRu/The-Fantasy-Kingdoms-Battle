using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Класс требования
    internal abstract class Requirement : Descriptor
    {
        public Requirement(DescriptorEntity forEntity, XmlNode n) : base()
        {
            ForEntity = forEntity;
        }

        public Requirement(DescriptorEntity forEntity) : base()
        {
            ForEntity = forEntity;
        }

        internal DescriptorEntity ForEntity { get; }
        internal abstract bool CheckRequirement(Player p);
        internal abstract TextRequirement GetTextRequirement(Player p);
    }

    internal sealed class RequirementConstruction : Requirement
    {
        private DescriptorConstruction construction;
        private string nameConstruction;
        private int level;

        public RequirementConstruction(DescriptorEntity forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            level = XmlUtils.GetInteger(n, "Level");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(level >= 0);
        }

        public RequirementConstruction(DescriptorEntity forEntity, string name, int requiredLevel) : base(forEntity)
        {
            nameConstruction = name;
            level = requiredLevel;
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            construction = Config.FindConstruction(nameConstruction);
            nameConstruction = "";

            Debug.Assert(construction.IsOurConstruction);
            Debug.Assert(level <= construction.MaxLevel, $"Требуется сооружение {construction.ID} {level} уровня, но у него максимум {construction.MaxLevel} уровень.");
        }

        internal override bool CheckRequirement(Player p) => p.GetPlayerConstruction(construction).Level >= level;
        internal override TextRequirement GetTextRequirement(Player p)
        {
            return new TextRequirement(CheckRequirement(p), p.GetPlayerConstruction(construction).TypeConstruction.Name + (level > 1 ? " " + level + " уровня" : ""));
        }
    }

    internal sealed class RequirementDestroyedLairs : Requirement
    {
        private DescriptorConstruction construction;
        private string nameConstruction;
        private int destroyed;

        public RequirementDestroyedLairs(DescriptorEntity forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            destroyed = XmlUtils.GetInteger(n, "Destroyed");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(destroyed > 0);
        }

        internal override bool CheckRequirement(Player p) => p.LairsDestroyed(construction) >= destroyed;

        internal override TextRequirement GetTextRequirement(Player p)
        {
            return new TextRequirement(CheckRequirement(p), $"Разрушить {construction.Name}: {p.LairsDestroyed(construction)}/{destroyed}");
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            construction = Config.FindConstruction(nameConstruction);
            nameConstruction = "";

            Debug.Assert(construction.Category == CategoryConstruction.Lair);
        }
    }

    internal sealed class RequirementTypeConstruction : Requirement
    {
        private DescriptorTypeConstruction typeConstruction;
        private string nameTypeConstruction;
        private int quantity;

        public RequirementTypeConstruction(DescriptorEntity forEntity, XmlNode n) : base(forEntity, n)
        {
            nameTypeConstruction = XmlUtils.GetStringNotNull(n, "TypeConstruction");
            quantity = XmlUtils.GetInteger(n, "Quantity");

            Debug.Assert(nameTypeConstruction.Length > 0);
            Debug.Assert(quantity > 0);
        }

        internal override bool CheckRequirement(Player p)
        {
            return p.TypeConstructionBuilded(typeConstruction) >= quantity;
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            typeConstruction = Config.FindTypeConstruction(nameTypeConstruction);
            nameTypeConstruction = "";
        }

        internal override TextRequirement GetTextRequirement(Player p) => new TextRequirement(CheckRequirement(p), $"Сооружение с типом \"{typeConstruction.Name}\": {quantity} шт.");
    }

    internal sealed class RequirementGoods : Requirement
    {
        private string nameConstruction;
        private string nameGoods;

        private DescriptorConstruction construction;
        private DescriptorItem goods;

        public RequirementGoods(DescriptorEntity forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            nameGoods = XmlUtils.GetStringNotNull(n, "Goods");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(nameGoods.Length > 0);
        }

        internal override bool CheckRequirement(Player p) => p.FindConstruction(construction.ID).GoodsAvailabled(goods);        
        internal override TextRequirement GetTextRequirement(Player p) => new TextRequirement(CheckRequirement(p), $"{goods.Name} ({construction.Name})");

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            construction = Config.FindConstruction(nameConstruction);
            goods = Config.FindItem(nameGoods);
            nameConstruction = "";
            nameGoods = "";

            bool founded = false;
            foreach (DescriptorCellMenuForConstruction cm in construction.ListResearches)
            {
                if (cm.NameEntity == goods.ID)
                {
                    founded = true;
                    break;
                }
            }

            Debug.Assert(founded, $"Товар {goods.ID} не найден в {construction.ID}.");
            //goods.UseForResearch.Add();
        }
    }

    internal sealed class RequirementExtension : Requirement
    {
        private DescriptorConstruction Construction;
        private string nameConstruction;
        private DescriptorConstructionExtension Extension;
        private string nameExtension;

        public RequirementExtension(DescriptorEntity forEntity, XmlNode n) : base(forEntity, n)
        {
            nameConstruction = XmlUtils.GetStringNotNull(n, "Construction");
            nameExtension = XmlUtils.GetStringNotNull(n, "Extension");

            Debug.Assert(nameConstruction.Length > 0);
            Debug.Assert(nameExtension.Length > 0);
        }

        internal override bool CheckRequirement(Player p)
        {
            return p.FindConstruction(Construction.ID).ExtensionAvailabled(Extension);
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            Construction = Config.FindConstruction(nameConstruction);
            Extension = Config.FindConstructionExtension(nameExtension);
            nameConstruction = "";
            nameExtension = "";

            bool founded = false;
            foreach (DescriptorCellMenuForConstruction cm in Construction.ListResearches)
                if (cm.NameEntity == Extension.ID)
                {
                    //cm.UseForResearches.Add(Goods);
                    founded = true;
                    break;
                }

            Debug.Assert(founded, $"Расширение {Extension.ID} не найдено в {Construction.ID}.");
        }

        internal override TextRequirement GetTextRequirement(Player p)
        {
            return new TextRequirement(CheckRequirement(p), $"{Extension.Name} ({Construction.Name})");
        }
    }
}