namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowConfirmExit : WindowConfirm
    {
        public WindowConfirmExit() : base("Выход из программы",
            Program.formMain.layerGame.CurrentLobby is null ? "Выйти из программы?": "Выход приведет к потере текущей игры.\r\nПродолжить?")
        {
        }

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            if (da == DialogAction.OK)
            {
                Program.formMain.layerGame.EndLobby();
                Program.formMain.SetProgrameState(ProgramState.NeedQuit);
                Program.formMain.Close();
            }
        }
    }
}