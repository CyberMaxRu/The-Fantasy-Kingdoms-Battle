using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class SecondarySkill : Entity
    {
        public SecondarySkill(Creature c, DescriptorSecondarySkill type) : base()
        {
            Creature = c;
            TypeSecondarySkill = type;
        }

        internal Creature Creature { get; }
        internal DescriptorSecondarySkill TypeSecondarySkill { get; }

        internal override int GetImageIndex()
        {
            return TypeSecondarySkill.ImageIndex;
        }

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            
        }

        internal override string GetName() => TypeSecondarySkill.Name;
        internal override string GetTypeEntity() => "Вторичный навык";
    }
}
