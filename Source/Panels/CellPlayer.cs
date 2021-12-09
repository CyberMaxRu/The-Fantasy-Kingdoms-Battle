using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс ячейки игрока лобби
    internal sealed class CellPlayer : VCCell
    {
        private Player _player;

        public CellPlayer(VisualControl parent, int shiftX) : base(parent, shiftX, 0)
        {

        }

        protected override void SetEntity(Entity po)
        {
            Assert(po is Player);

            base.SetEntity(po);

            _player = (Player)po;
        }

        protected override bool Selected() => _player != null ? _player == _player.Lobby.CurrentPlayer : false;
    }
}
