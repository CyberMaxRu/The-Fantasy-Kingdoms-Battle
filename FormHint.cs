using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;

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
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        internal readonly Label lblHeader;
        internal readonly Label lblAction;
        internal readonly Label lblDescription;
        internal readonly List<Label> lblRequirement = new List<Label>();
        internal readonly Label lblIncome;
        internal readonly Label lblGold;
        internal readonly Label lblBuilders;
        internal readonly Label lblDamageMelee;
        internal readonly Label lblDamageMissile;
        internal readonly Label lblDamageMagic;
        internal readonly Label lblDefenseMelee;
        internal readonly Label lblDefenseMissile;
        internal readonly Label lblDefenseMagic;

        internal readonly Timer timerDelayShow;
        internal readonly Timer timerOpacity;
        internal readonly double maxOpacity = 0.90;
        internal readonly int timeOpacity = 150;
        internal int stepsOpacity = 0;
        internal int stepsInSecond = 50;// 25 кадров в секунду, больше не имеет смысла
        internal DateTime dateTimeStartOpacity;
        internal Font fontRequirement;

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

            lblIncome = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = ClientSize.Width - (Config.GRID_SIZE * 2),
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
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = ClientSize.Width - (Config.GRID_SIZE * 2),
                ImageList = ilGui16,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblBuilders = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = ClientSize.Width - (Config.GRID_SIZE * 2),
                ImageList = ilGui16,
                ImageIndex = FormMain.GUI_16_PEASANT,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDamageMelee = new Label()
            {
                Parent = this,
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = (ClientSize.Width - (Config.GRID_SIZE * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_ATTACK_MELEE,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDamageMissile = new Label()
            {
                Parent = this,
                Left = lblDamageMelee.Left + lblDamageMelee.Width,
                Top = Config.GRID_SIZE,
                Width = (ClientSize.Width - (Config.GRID_SIZE * 2)) / 3,
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
                Left = lblDamageMissile.Left + lblDamageMissile.Width,
                Top = Config.GRID_SIZE,
                Width = (ClientSize.Width - (Config.GRID_SIZE * 2)) / 3,
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
                Left = Config.GRID_SIZE,
                Top = Config.GRID_SIZE,
                Width = (ClientSize.Width - (Config.GRID_SIZE * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_DEFENSE_MELEE,
                ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };

            lblDefenseMissile = new Label()
            {
                Parent = this,
                Left = lblDefenseMelee.Left + lblDefenseMelee.Width,
                Top = Config.GRID_SIZE,
                Width = (ClientSize.Width - (Config.GRID_SIZE * 2)) / 3,
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
                Left = lblDefenseMissile.Left + lblDefenseMissile.Width,
                Top = Config.GRID_SIZE,
                Width = (ClientSize.Width - (Config.GRID_SIZE * 2)) / 3,
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
            SetForegroundWindow(Handle);
            Show();
            timerDelayShow.Enabled = false;
            dateTimeStartOpacity = DateTime.Now;
            timerOpacity.Enabled = true; 
        }

        internal void ShowHint(Point p, string header, string action, string description, List<TextRequirement> requirement, int gold, bool goldEnough,
            int income, int builders, bool buildersEnough, PlayerItem pi)
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

            if (income > 0)
            {
                lblIncome.Show();
                lblIncome.Top = nextTop;
                lblIncome.Text = "     +" + income.ToString();

                nextTop = GuiUtils.NextTop(lblIncome);
            }
            else
                lblIncome.Hide();

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
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        ForeColor = tr.Performed == true ? Color.Lime : Color.Crimson,
                        Font = fontRequirement,
                        Text = tr.Text
                    };
                    lr.MaximumSize = new Size(Width - Config.GRID_SIZE * 2, 0);

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

            if (builders > 0)
            {
                lblBuilders.ForeColor = buildersEnough == true ? Color.Lime : Color.Crimson;
                lblBuilders.Top = nextTop;
                lblBuilders.Text = "     " + builders.ToString();
                lblBuilders.Show();

                nextTop = GuiUtils.NextTop(lblBuilders);
            }
            else
                lblBuilders.Hide();

            lblDamageMelee.Hide();
            lblDamageMissile.Hide();
            lblDamageMagic.Hide();
            lblDefenseMelee.Hide();
            lblDefenseMissile.Hide();
            lblDefenseMagic.Hide();

            if (pi != null)
            {
                switch (pi.Item.TypeItem.Category)
                {
                    case CategoryItem.Weapon:
                        lblDamageMelee.Top = nextTop;
                        lblDamageMissile.Top = nextTop;
                        lblDamageMagic.Top = nextTop;

                        lblDamageMelee.Text = "     " + (pi.Item.DamageMelee > 0 ? pi.Item.DamageMelee.ToString() : "");
                        lblDamageMissile.Text = "     " + (pi.Item.DamageMissile > 0 ? pi.Item.DamageMissile.ToString() : "");
                        lblDamageMagic.Text = "     " + (pi.Item.DamageMagic > 0 ? pi.Item.DamageMagic.ToString() : "");

                        lblDamageMelee.Show();
                        lblDamageMissile.Show();
                        lblDamageMagic.Show();

                        nextTop = GuiUtils.NextTop(lblDamageMelee);
                        break;
                    case CategoryItem.Armour:
                        lblDefenseMelee.Top = nextTop;
                        lblDefenseMissile.Top = nextTop;
                        lblDefenseMagic.Top = nextTop;

                        lblDefenseMelee.Text = "     " + (pi.Item.DefenseMelee > 0 ? pi.Item.DefenseMelee.ToString() : "");
                        lblDefenseMissile.Text = "     " + (pi.Item.DefenseMissile > 0 ? pi.Item.DefenseMissile.ToString() : "");
                        lblDefenseMagic.Text = "     " + (pi.Item.DefenseMagic > 0 ? pi.Item.DefenseMagic.ToString() : "");

                        lblDefenseMelee.Show();
                        lblDefenseMissile.Show();
                        lblDefenseMagic.Show();

                        nextTop = GuiUtils.NextTop(lblDefenseMelee);
                        break;
                }
            }

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
