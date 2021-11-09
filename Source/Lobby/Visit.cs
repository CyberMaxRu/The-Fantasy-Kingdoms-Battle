using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle.Source.Lobby
{
    internal class Visit : EntityForCreature
    {
        public Visit(Creature creature) : base(creature)
        {
        }

        internal override int GetImageIndex()
        {
            throw new NotImplementedException();
        }

        internal override void PrepareHint()
        {
            throw new NotImplementedException();
        }
    }
}
