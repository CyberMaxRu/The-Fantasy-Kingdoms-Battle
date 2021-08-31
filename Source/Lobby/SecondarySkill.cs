using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class SecondarySkill : Entity
    {
        public SecondarySkill(Creature c, TypeSecondarySkill type) : base()
        {
            Creature = c;
            TypeSecondarySkill = type;
        }

        internal Creature Creature { get; }
        internal TypeSecondarySkill TypeSecondarySkill { get; }

        internal override string GetCost()
        {
            return "";
        }

        internal override int GetImageIndex()
        {
            return TypeSecondarySkill.ImageIndex;
        }

        internal override int GetLevel()
        {
            return 0;
        }

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override int GetQuantity()
        {
            return 0;
        }

        internal override void PrepareHint()
        {
            
        }
    }
}
