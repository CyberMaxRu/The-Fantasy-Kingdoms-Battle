using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Класс участника миссии
    internal sealed class MissionMember : BigEntity
    {
        public MissionMember(Lobby l, DescriptorMissionMember d) : base(d, l, null)
        {
            Descriptor = d;
        }

        internal new DescriptorMissionMember Descriptor { get; }

        internal override int GetImageIndex() => Descriptor.ImageIndex;

        internal override string GetName() => Descriptor.Name;

        internal override string GetTypeEntity() => Descriptor.GetTypeEntity();

        internal override string GetIDEntity(DescriptorEntity descriptor) => descriptor.ID;

        internal override void PrepareHint(PanelHint panelHint)
        {
        }

        internal override void ShowInfo(int selectPage = -1)
        {
        }
    }
}
