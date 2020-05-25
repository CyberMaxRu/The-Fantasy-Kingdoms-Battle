using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    public partial class FormBattle : Form
    {
        private PanelHeroInBattle[,] cellHeroes;
        private int currentStep;
        private Battle battle;

        public FormBattle()
        {
            InitializeComponent();
        }

        internal void ShowBattle(Battle b)
        {
            battle = b;
            currentStep = 0;
            PanelHeroInBattle p;

            cellHeroes = new PanelHeroInBattle[b.SizeBattlefield.Height, b.SizeBattlefield.Width];
            for (int y = 0; y < b.SizeBattlefield.Height; y++)
                for (int x = 0; x < b.SizeBattlefield.Width; x++)
                {
                    p = new PanelHeroInBattle(Program.formMain.ilGuiHeroes)
                    {
                        Parent = this,
                        Left = Config.GRID_SIZE + (x * (Program.formMain.ilGuiHeroes.ImageSize.Width + Config.GRID_SIZE)),
                        Top = Config.GRID_SIZE + (y * (Program.formMain.ilGuiHeroes.ImageSize.Height + 16 + Config.GRID_SIZE))
                    };

                    cellHeroes[y, x] = p;
                }

            ApplyStep();

            ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentStep++;
            ApplyStep();
        }

        private void ApplyStep()
        {
            for (int y = 0; y < battle.SizeBattlefield.Height; y++)
                for (int x = 0; x < battle.SizeBattlefield.Width; x++)
                    cellHeroes[y, x].Hero = null;

            foreach (HeroInBattle h in battle.Steps[currentStep].Heroes)
            {
                Debug.Assert(cellHeroes[h.Parameters.Coord.Y, h.Parameters.Coord.X].Hero == null);

                cellHeroes[h.Parameters.Coord.Y, h.Parameters.Coord.X].Hero = h;
            }

            Refresh(); 
        }
    }
}
