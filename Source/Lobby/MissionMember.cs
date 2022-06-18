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
        public MissionMember(Player p, DescriptorMissionMember d) : base(d, p.Lobby, p)
        {
            Descriptor = d;
        }

        internal new DescriptorMissionMember Descriptor { get; }

        internal override int GetImageIndex() => Descriptor.ImageIndex;

        internal override string GetName() => Descriptor.Name;

        internal override string GetTypeEntity() => Descriptor.GetTypeEntity();

        internal override void PrepareHint(PanelHint panelHint)
        {
        }

        internal override void ShowInfo(int selectPage = -1)
        {
        }
    }
}
