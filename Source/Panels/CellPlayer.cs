namespace Fantasy_Kingdoms_Battle
{
    // Класс ячейки игрока лобби
    internal sealed class CellPlayer : VCCell
    {
        public CellPlayer(VisualControl parent, int shiftX) : base(parent, shiftX, 0)
        {
            
        }

        protected override bool Selected() => Program.formMain.CurrentLobby?.CurrentPlayer == Entity;
    }
}
