using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

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
        internal readonly Label lblIncome;
        internal readonly Label lblGold;
        internal readonly Label lblBuilders;
        internal readonly Label lblDamageMelee;
        internal readonly Label lblDamageArcher;
        internal readonly Label lblDamageMagic;
        internal readonly Label lblDefenseMelee;
        internal readonly Label lblDefenseArcher;
        internal readonly Label lblDefenseMagic;

        internal readonly Timer timerDelayShow;
        internal readonly Timer timerOpacity;
        internal readonly double maxOpacity = 0.90;
        internal readonly int timeOpacity = 150;
        internal int stepsOpacity = 0;
        internal int stepsInSecond = 50;// 25 кадров в секунду, больше не имеет смысла
        internal DateTime dateTimeStartOpacity;
        internal Font fontRequirement;

        private int nextTop;

        public FormHint(Image backgroundImage, ImageList ilGui16, ImageList ilParameters)
        {
            //TopMost = true;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = backgroundImage;

            lblHeader = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width,
                Height = 18,
                BackColor = Color.Transparent,
                ForeColor = Color.Gold,
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            };

            lblAction = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
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
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                BackColor = Color.Transparent,
                ForeColor = Color.Silver,
                Font = new Font("Microsoft Sans Serif", 10)
            };
            lblDescription.MaximumSize = new Size(ClientSize.Width - (FormMain.Config.GridSize * 2), 0);

            lblIncome = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width - (FormMain.Config.GridSize * 2),
                ImageList = ilGui16,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.Lime,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblGold = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width - (FormMain.Config.GridSize * 2),
                ImageList = ilGui16,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblBuilders = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width - (FormMain.Config.GridSize * 2),
                ImageList = ilGui16,
                ImageIndex = FormMain.GUI_16_PEASANT,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDamageMelee = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_ATTACK_MELEE,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDamageArcher = new Label()
            {
                Parent = this,
                Left = lblDamageMelee.Left + lblDamageMelee.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_ATTACK_RANGE,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDamageMagic= new Label()
            {
                Parent = this,
                Left = lblDamageArcher.Left + lblDamageArcher.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_ATTACK_MAGIC,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDefenseMelee = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_DEFENSE_MELEE,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDefenseArcher = new Label()
            {
                Parent = this,
                Left = lblDefenseMelee.Left + lblDefenseMelee.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_DEFENSE_RANGE,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDefenseMagic = new Label()
            {
                Parent = this,
                Left = lblDefenseArcher.Left + lblDefenseArcher.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_DEFENSE_MAGIC,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            fontRequirement = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);

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

            Clear();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                baseParams.ExStyle |= (WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);

                return baseParams;
            }
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

        internal void Clear()
        {
            lblHeader.Text = "";
            lblAction.Hide();
            lblDescription.Hide();
            lblIncome.Hide();

            foreach (Label l in lblRequirement)
                l.Dispose();
            lblRequirement.Clear();

            lblGold.Hide();
            lblBuilders.Hide();

            lblDamageMelee.Hide();
            lblDamageArcher.Hide();
            lblDamageMagic.Hide();
            lblDefenseMelee.Hide();
            lblDefenseArcher.Hide();
            lblDefenseMagic.Hide();

            nextTop = GuiUtils.NextTop(lblHeader);
        }

        internal void AddStep1Header(string header, string action, string description)
        {
            Debug.Assert(header.Length > 0);

            lblHeader.Text = header;

            if (action.Length > 0)
            {
                lblAction.Top = nextTop;
                lblAction.Text = action;
                lblAction.Show();

                nextTop = GuiUtils.NextTop(lblAction);
            }

            if (description.Length > 0)
            {
                lblDescription.Show();
                lblDescription.Top = nextTop;
                lblDescription.Text = description;

                nextTop = GuiUtils.NextTop(lblDescription);
            }
        }

        internal void AddStep2Income(int income)
        {
            if (income > 0)
            {
                lblIncome.Show();
                lblIncome.Top = nextTop;
                lblIncome.Text = "     +" + income.ToString();

                nextTop = GuiUtils.NextTop(lblIncome);
            }
        }

        internal void AddStep3Requirement(List<TextRequirement> requirement)
        {
            Debug.Assert(requirement != null);

            Label lr;
            foreach (TextRequirement tr in requirement)
            {
                lr = new Label()
                {
                    Parent = this,
                    Left = FormMain.Config.GridSize,
                    Top = nextTop,
                    Width = Width - (FormMain.Config.GridSize * 2),
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    ForeColor = tr.Performed == true ? Color.Lime : Color.Crimson,
                    Font = fontRequirement,
                    Text = tr.Text
                };
                lr.MaximumSize = new Size(Width - FormMain.Config.GridSize * 2, 0);

                lblRequirement.Add(lr);
                nextTop = GuiUtils.NextTop(lr);
            }
        }

        internal void AddStep4Gold(int gold, bool goldEnough)
        {
            if (gold > 0)
            {
                lblGold.ForeColor = goldEnough == true ? Color.Lime : Color.Crimson;
                lblGold.Top = nextTop;
                lblGold.Text = "     " + gold.ToString();
                lblGold.Show();

                nextTop = GuiUtils.NextTop(lblGold);
            }
        }

        internal void AddStep5Builders(int builders, bool buildersEnough)
        {
            if (builders > 0)
            {
                lblBuilders.ForeColor = buildersEnough == true ? Color.Lime : Color.Crimson;
                lblBuilders.Top = nextTop;
                lblBuilders.Text = "     " + builders.ToString();
                lblBuilders.Show();

                nextTop = GuiUtils.NextTop(lblBuilders);
            }
        }

        internal void AddStep6PlayerItem(PlayerItem pi)
        {
            Debug.Assert(pi != null);

        }

        internal void AddStep7Weapon(Weapon w)
        {
            Debug.Assert(w != null);

            lblDamageMelee.Top = nextTop;
            lblDamageArcher.Top = nextTop;
            lblDamageMagic.Top = nextTop;

            lblDamageMelee.Text = "     " + (w.DamageMelee > 0 ? w.DamageMelee.ToString() : "");
            lblDamageArcher.Text = "     " + (w.DamageArcher > 0 ? w.DamageArcher.ToString() : "");
            lblDamageMagic.Text = "     " + (w.DamageMagic > 0 ? w.DamageMagic.ToString() : "");

            lblDamageMelee.Show();
            lblDamageArcher.Show();
            lblDamageMagic.Show();

            nextTop = GuiUtils.NextTop(lblDamageMelee);
        }

        internal void AddStep8Armour(Armour a)
        {
            Debug.Assert(a != null);
                
            lblDefenseMelee.Top = nextTop;
            lblDefenseArcher.Top = nextTop;
            lblDefenseMagic.Top = nextTop;

            lblDefenseMelee.Text = "     " + (a.DefenseMelee > 0 ? a.DefenseMelee.ToString() : "");
            lblDefenseArcher.Text = "     " + (a.DefenseArcher > 0 ? a.DefenseArcher.ToString() : "");
            lblDefenseMagic.Text = "     " + (a.DefenseMagic > 0 ? a.DefenseMagic.ToString() : "");

            lblDefenseMelee.Show();
            lblDefenseArcher.Show();
            lblDefenseMagic.Show();

            nextTop = GuiUtils.NextTop(lblDefenseMelee);
        }

        internal void ShowHint(Control c)
        {
            Debug.Assert(lblHeader.Text.Length > 0);

            Location = c.PointToScreen(new Point(0, c.Height + 2));

            bool needReshow = (Visible == false) || (Height != nextTop);
            Height = nextTop;

            if (needReshow == true)
            {
                Opacity = 0;
                timerDelayShow.Enabled = true;
            }
        }

        internal void ShowHint(Point location)
        {
            Debug.Assert(lblHeader.Text.Length > 0);
            Debug.Assert(location.X >= 0);
            Debug.Assert(location.Y >= 0);

            Location = location;

            bool needReshow = (Visible == false) || (Height != nextTop);
            Height = nextTop;

            if (needReshow == true)
            {
                Opacity = 0;
                timerDelayShow.Enabled = true;
            }
        }

        internal void HideHint()
        {
            timerDelayShow.Enabled = false;
            timerOpacity.Enabled = false;

            Hide();
        }
    }
}
