using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    class WindowMenuInGame : CustomWindow
    {
        private VCBitmap bmpMenu;
        private VCButton btnBackToGame;
        private VCButton btnSettings;
        private VCButton btnExitToMainMenu;
        private VCButton btnExitToWindows;

        public WindowMenuInGame()
        {
            bmpMenu = new VCBitmap(this, 0, 0, Program.formMain.bmpMenuInGame);
            Width = bmpMenu.Width;
            Height = bmpMenu.Height;

            btnBackToGame = new VCButton(bmpMenu, 80, 72, "Вернуться к игре");
            btnBackToGame.Width = Width - 80 - 80;
            btnExitToWindows = new VCButton(bmpMenu, 80, bmpMenu.Height - 96, "Выход в Windows");
            btnExitToWindows.Width = Width - 80 - 80;
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

        }
    }
}
