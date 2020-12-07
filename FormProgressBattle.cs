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
    // Класс отображение прогресса битвы между двумя другими игроками
    public partial class FormProgressBattle : Form
    {
        private Battle battle;
        private ProgressBar progressBar;
        private Label speedCalc;
        private List<DateTime> steps = new List<DateTime>();

        public FormProgressBattle()
        {
            InitializeComponent();

            Width = 320;
            Height = 96;

            progressBar = new ProgressBar()
            {
                Parent = this,
                Width = ClientSize.Width - (FormMain.Config.GridSize * 2),
                Top = FormMain.Config.GridSize,
                Left = FormMain.Config.GridSize
            };

            speedCalc = new Label()
            {
                Parent = this,
                Width = progressBar.Width,
                Top = GuiUtils.NextTop(progressBar),
                Left = FormMain.Config.GridSize
            };
        }

        internal void SetBattle(Battle b, int totalPairs, int pair)
        {
            Debug.Assert(pair <= totalPairs);
            Text = "Расчет битвы пары №" + pair.ToString() + " из " + totalPairs.ToString();
            battle = b;
            progressBar.Maximum = FormMain.Config.MaxStepsInBattle;
            steps.Clear();

            ShowDialog();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Полный расчет боя
            for (; ; )
            {
                if (!battle.CalcStep())
                    break;

                ShowStep();
                Application.DoEvents();
            }

            Close();
        }

        internal void ShowStep()
        {
            steps.Add(DateTime.Now); 
            Utils.TrimActions(steps);

            progressBar.Value = battle.Step;
            speedCalc.Text = steps.Count().ToString() + " (" + battle.Step.ToString() + ")";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!battle.BattleCalced)
                e.Cancel = true;
        }
    }
}
