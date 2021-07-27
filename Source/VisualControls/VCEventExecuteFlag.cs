using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCEventExecuteFlag : VCEvent
    {
        private VCImage bmpTypeFlag;

        public VCEventExecuteFlag(TypeFlag typeFlag, TypeLair tl, PlayerLair pl, bool winner, Battle b) : base()
        {
            TypeFlag = typeFlag;
            TypeLair = tl;
            Target = pl;
            Winner = winner;
            Battle = b;

            bmpTypeFlag = new VCImage(this, FormMain.Config.GridSize, FormMain.Config.GridSize, Program.formMain.ilGui, LobbyPlayer.TypeFlagToImageIndex(typeFlag));
                           
            ApplyMaxSize();

            Height += FormMain.Config.GridSize;
        }

        internal TypeFlag TypeFlag { get; }
        internal TypeLair TypeLair { get; }
        internal PlayerLair Target { get; }
        internal bool Winner { get; }
        internal Battle Battle { get; }
    }
}
