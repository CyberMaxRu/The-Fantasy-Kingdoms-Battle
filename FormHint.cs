using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace Fantasy_King_s_Battle
{
    internal sealed class TextRequirement
    {
        public TextRequirement(bool performed, string text)
        {
            Performed = performed;
            Text = text;
        }

        internal bool Performed { get; }
        internal string Text { get; }
    }

    internal sealed class FormHint : Form
    {
        internal readonly Label lblHeader;
        internal readonly Label lblAction;
        internal readonly Label lblDescription;
        internal readonly List<Label> lblRequirement = new List<Label>();
        internal readonly Label lblGold;
        internal readonly Timer timerDelayShow;
        internal readonly Timer timerOpacity;
        internal readonly double maxOpacity = 0.90;
        internal readonly int timeOpacity = 100;
        internal int stepsOpacity = 0;
        internal int stepsInSecond = 50;// 25 кадров в секунду, больше не имеет смысла
        internal DateTime dateTimeStartOpacity;
        internal Font fontRequirement;

        public FormHint(Image backgroundImage, ImageList ilGui16)
        {
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;

            lblHeader = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = ClientSize.Width,
                Height = 18,
                BackColor = Color.Transparent,
                ForeColor = Color.Gold,
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            };

            lblAction = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = ClientSize.Width,
                Height = 18,
                BackColor = Color.Transparent,
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            };

            lblDescription = new Label()
            {
                Parent = this,
                AutoSize = true,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                BackColor = Color.Transparent,
                ForeColor = Color.Silver,
                Font = new Font("Microsoft Sans Serif", 10)
            };
            lblDescription.MaximumSize = new Size(ClientSize.Width - (Config.GRID_SIZE * 2), 0);
            
            lblGold = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = ClientSize.Width - (Config.GRID_SIZE * 2),
                ImageList = ilGui16,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            fontRequirement = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

            FormBorderStyle = FormBorderStyle.None;            
            BackgroundImage = backgroundImage;

            timerDelayShow = new Timer()
            {
                Interval = 500,
                Enabled = false
            };
            timerDelayShow.Tick += TimerDelayShow_Tick;

            stepsOpacity = 0;
            timerOpacity = new Timer()
            {
                Interval = 1000 / stepsInSecond,
                Enabled = false
            };
            timerOpacity.Tick += TimerOpacity_Tick;
        }

        private void TimerOpacity_Tick(object sender, EventArgs e)
        {
            TimeSpan timeSpan = DateTime.Now - dateTimeStartOpacity;
            double percent = timeSpan.TotalMilliseconds * 100 / timeOpacity;
            if (percent >= 100)
            {
                Opacity = maxOpacity;
                timerOpacity.Enabled = false;
            }
            else
            {
                Opacity = maxOpacity * percent / 100;
            }
        }

        private void TimerDelayShow_Tick(object sender, EventArgs e)
        {
            Show();
            timerDelayShow.Enabled = false;
            dateTimeStartOpacity = DateTime.Now;
            timerOpacity.Enabled = true; 
        }

        internal void ShowHint(Point p, string header, string action, string description, List<TextRequirement> requirement, int gold, bool goldEnough)
        {
            Left = p.X;
            Top = p.Y;

            lblHeader.Text = header;
            int nextTop = GuiUtils.NextTop(lblHeader);

            if (action.Length > 0)
            {
                lblAction.Top = nextTop;
                lblAction.Text = action;
                lblAction.Show();

                nextTop = GuiUtils.NextTop(lblAction);
            }
            else
                lblAction.Hide();

            if (description.Length > 0)
            {
                lblDescription.Show();
                lblDescription.Top = nextTop;
                lblDescription.Text = description;

                nextTop = GuiUtils.NextTop(lblDescription);
            }
            else
                lblDescription.Hide();
            
            // Секция требований
            foreach (Label l in lblRequirement)
            {
                l.Dispose();
            }
            lblRequirement.Clear();

            Label lr;
            if (requirement != null)
            {
                foreach (TextRequirement tr in requirement)
                {
                    lr = new Label()
                    {
                        Parent = this,
                        Left = Config.GRID_SIZE,
                        Top = nextTop,
                        Width = Width - (Config.GRID_SIZE * 2),
                        Height = 16,
                        BackColor = Color.Transparent,
                        ForeColor = tr.Performed == true ? Color.Lime : Color.Crimson,
                        Font = fontRequirement,
                        Text = tr.Text
                    };

                    lblRequirement.Add(lr);
                    nextTop = GuiUtils.NextTop(lr);
                }
            }

            if (gold > 0)
            {
                lblGold.ForeColor = goldEnough == true ? Color.Lime : Color.Crimson;
                lblGold.Top = nextTop;
                lblGold.Text = "     " + gold.ToString();
                lblGold.Show();

                nextTop = GuiUtils.NextTop(lblGold);
            }
            else
                lblGold.Hide();

            Height = nextTop;

            Opacity = 0;
            timerDelayShow.Enabled = true;
        }

        internal void HideHint()
        {
            timerDelayShow.Enabled = false;
            timerOpacity.Enabled = false;

            Hide();
        }
    }
}
