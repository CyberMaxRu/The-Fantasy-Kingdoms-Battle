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
        private Battle battle;
        private Pen penArrow = new Pen(Color.Fuchsia);
        private Bitmap bmpBackground;

        public FormBattle()
        {
            InitializeComponent();

            Paint += FormBattle_Paint;
            FormClosing += FormBattle_FormClosing;

            BackgroundImage = Program.formMain.background;
            penArrow.Width = 3;
            penArrow.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4.0F, 8.0F, true);
            //bmpBackground = new Bitmap(Width, Height);
            //bmpBackground.
        }

        private void FormBattle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (battle.BattleCalced == false)
                battle.CalcWholeBattle(); 
        }

        private void FormBattle_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmpPanel = new Bitmap(cellHeroes[0, 0].Width, cellHeroes[0, 0].Height);

            // Рисуем героев
            for (int y = 0; y < battle.SizeBattlefield.Height; y++)
                for (int x = 0; x < battle.SizeBattlefield.Width; x++)
                {
                    if (cellHeroes[y, x].Hero != null)
                    {
                        cellHeroes[y, x].DrawToBitmap(bmpPanel, new Rectangle(0, 0, bmpPanel.Width, bmpPanel.Height));
                        e.Graphics.DrawImageUnscaled(bmpPanel, cellHeroes[y, x].Left, cellHeroes[y, x].Top);
                    }
                }

            foreach (HeroInBattle h in battle.ActiveHeroes)
            {
                if (h.Target != null)
                {
                    PanelHeroInBattle p1 = cellHeroes[h.Coord.Y, h.Coord.X];
                    PanelHeroInBattle p2 = cellHeroes[h.Target.Coord.Y, h.Target.Coord.X];

                    penArrow.Color = h.PlayerHero.Player == battle.Player1 ? Color.Green : Color.Maroon;
                    e.Graphics.DrawLine(penArrow, new Point(p1.Location.X + p1.Width / 2 , p1.Location.Y + p1.Height / 2), new Point(p2.Location.X + p2.Width / 2, p2.Location.Y + p2.Height / 2));
                }
            }
        }

        internal void ShowBattle(Battle b)
        {
            battle = b;
            PanelHeroInBattle p;

            cellHeroes = new PanelHeroInBattle[b.SizeBattlefield.Height, b.SizeBattlefield.Width];
            for (int y = 0; y < b.SizeBattlefield.Height; y++)
                for (int x = 0; x < b.SizeBattlefield.Width; x++)
                {
                    p = new PanelHeroInBattle(Program.formMain.ilGuiHeroes)
                    {
                        //Parent = this,
                        Left = Config.GRID_SIZE + (x * (Program.formMain.ilGuiHeroes.ImageSize.Width + Config.GRID_SIZE)),
                        Top = Config.GRID_SIZE * 4 + (y * (Program.formMain.ilGuiHeroes.ImageSize.Height + 16 + Config.GRID_SIZE))
                    };

                    cellHeroes[y, x] = p;
                }

            ApplyStep();

            ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (battle.BattleCalced == false)
            {
                battle.CalcStep();
                ApplyStep();
            }
        }

        private void ApplyStep()
        {
            for (int y = 0; y < battle.SizeBattlefield.Height; y++)
                for (int x = 0; x < battle.SizeBattlefield.Width; x++)
                    cellHeroes[y, x].Hero = null;

            foreach (HeroInBattle h in battle.ActiveHeroes)
            {
                Debug.Assert(cellHeroes[h.Coord.Y, h.Coord.X].Hero == null);

                cellHeroes[h.Coord.Y, h.Coord.X].Hero = h;
            }

            lblStep.Text = "Шаг: " + battle.Step.ToString();
            lblTotalSteps.Text = battle.BattleCalced == false ? "Идет бой" : "Бой закончен";

            Refresh();
        }
    }
}
