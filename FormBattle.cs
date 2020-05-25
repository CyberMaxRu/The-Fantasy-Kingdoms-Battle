using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    public partial class FormBattle : Form
    {
        internal PanelHeroInBattle[,] cellHeroes;

        public FormBattle()
        {
            InitializeComponent();
        }

        internal void ShowBattle(Battle b)
        {
            PanelHeroInBattle p;

            cellHeroes = new PanelHeroInBattle[b.SizeBattlefield.Height, b.SizeBattlefield.Width];
            for (int y = 0; y < b.SizeBattlefield.Height; y++)
                for (int x = 0; x < b.SizeBattlefield.Width; x++)
                {
                    p = new PanelHeroInBattle(null, Program.formMain.ilGuiHeroes)
                    {
                        Parent = this,
                        Left = Config.GRID_SIZE + (x * (Program.formMain.ilGuiHeroes.ImageSize.Width + Config.GRID_SIZE)),
                        Top = Config.GRID_SIZE + (y * (Program.formMain.ilGuiHeroes.ImageSize.Height + 16 + Config.GRID_SIZE))
                    };
                }

            ShowDialog();
        }
    }
}
