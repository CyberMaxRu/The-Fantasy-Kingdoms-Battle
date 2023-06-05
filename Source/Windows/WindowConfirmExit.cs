// Ревизия 05.06.2023
//
using static Fantasy_Kingdoms_Battle.Program;
using static Fantasy_Kingdoms_Battle.Utils;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Fantasy_Kingdoms_Battle
{
    // Окно подтверждения выхода из игры
    internal sealed class WindowConfirmExit : WindowConfirm
    {
        public WindowConfirmExit() : base("Выход из игры", formMain.InGame() ? "Выход приведет к потере текущей игры.\r\nПродолжить?" : "Выйти из игры?")
        {
            Assert(formMain.ProgramState == ProgramState.Started);
        }

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            formMain.SetProgrameState(da == DialogAction.OK ? ProgramState.NeedQuit : ProgramState.Started);
        }

        internal static void ConfirmExit()
        {
            WindowConfirmExit w = new WindowConfirmExit();
            formMain.SetProgrameState(ProgramState.ConfirmQuit);
            w.Show();
        }
    }
}