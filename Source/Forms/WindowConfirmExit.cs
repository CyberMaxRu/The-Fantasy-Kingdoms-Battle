namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowConfirmExit : WindowConfirm
    {
        public WindowConfirmExit() : base("Выход из программы",
            Program.formMain.layerGame.CurrentLobby is null ? "Выйти из программы?": "Выход приведет к потере текущей игры.\r\nПродолжить?")
        {
            Program.formMain.SetProgrameState(ProgramState.ConfirmQuit);
        }

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            Program.formMain.SetProgrameState(da == DialogAction.OK ? ProgramState.NeedQuit : ProgramState.Started);
        }
    }
}