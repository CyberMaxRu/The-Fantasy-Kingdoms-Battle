using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс монстра в логове
    internal sealed class Monster : Creature
    {
        public Monster(DescriptorCreature tm, int level, BattleParticipant bp) : base(tm, bp)
        {
            Debug.Assert(tm != null);
            Debug.Assert(level > 0);
        }

        internal override void PrepareHint(PanelHint panelHint)
        {
            //Debug.Assert(IsLive);

            panelHint.AddStep2Header(TypeCreature.Name);
            panelHint.AddStep5Description(TypeCreature.Description);
            panelHint.AddStep7Reward(TypeCreature.TypeReward.Cost.ValueGold());
            panelHint.AddStep8Greatness(TypeCreature.TypeReward.Greatness, 0);
        }

        internal override void HideInfo()
        {
            base.HideInfo();

            Lobby.Layer.panelMonsterInfo.Visible = false;
        }

        internal override void ShowInfo(int selectPage = -1)
        {
            Debug.Assert(IsLive);

            Lobby.Layer.panelMonsterInfo.Entity = this;
            Lobby.Layer.panelMonsterInfo.Visible = true;
        }
    }
}
