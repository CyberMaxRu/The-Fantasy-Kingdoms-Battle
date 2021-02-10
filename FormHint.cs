using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
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
        private Bitmap bmpBackground;

        internal readonly Timer timerDelayShow;
        internal Font fontRequirement;

        private int nextTop;

        public FormHint(ImageList ilGui16, ImageList ilParameters)
        {
            //TopMost = true;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;

            lblHeader = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width,
                Height = 20,
                ForeColor = FormMain.Config.HintHeader,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintMainText
            };

            lblAction = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width,
                Height = 18,
                ForeColor = FormMain.Config.HintAction,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintMainText
            };

            lblDescription = new Label()
            {
                Parent = this,
                AutoSize = true,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                ForeColor = FormMain.Config.HintDescription,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
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
                ImageAlign = ContentAlignment.MiddleLeft,
                ForeColor = FormMain.Config.HintIncome,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblGold = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width - (FormMain.Config.GridSize * 2),
                ImageList = ilGui16,
                ImageIndex = FormMain.GUI_16_GOLD,
                ImageAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblBuilders = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = ClientSize.Width - (FormMain.Config.GridSize * 2),
                ImageAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblDamageMelee = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_ATTACK_MELEE,
                ImageAlign = ContentAlignment.MiddleLeft,
                ForeColor = FormMain.Config.HintParameter,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblDamageArcher = new Label()
            {
                Parent = this,
                Left = lblDamageMelee.Left + lblDamageMelee.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_ATTACK_RANGE,
                ImageAlign = ContentAlignment.MiddleLeft,
                ForeColor = FormMain.Config.HintParameter,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblDamageMagic= new Label()
            {
                Parent = this,
                Left = lblDamageArcher.Left + lblDamageArcher.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_ATTACK_MAGIC,
                ImageAlign = ContentAlignment.MiddleLeft,
                ForeColor = FormMain.Config.HintParameter,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblDefenseMelee = new Label()
            {
                Parent = this,
                Left = FormMain.Config.GridSize,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_DEFENSE_MELEE,
                ImageAlign = ContentAlignment.MiddleLeft,
                ForeColor = FormMain.Config.HintParameter,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblDefenseArcher = new Label()
            {
                Parent = this,
                Left = lblDefenseMelee.Left + lblDefenseMelee.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_DEFENSE_RANGE,
                ImageAlign = ContentAlignment.MiddleLeft,
                ForeColor = FormMain.Config.HintParameter,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            lblDefenseMagic = new Label()
            {
                Parent = this,
                Left = lblDefenseArcher.Left + lblDefenseArcher.Width,
                Top = FormMain.Config.GridSize,
                Width = (ClientSize.Width - (FormMain.Config.GridSize * 2)) / 3,
                ImageList = ilParameters,
                ImageIndex = FormMain.GUI_PARAMETER_DEFENSE_MAGIC,
                ImageAlign = ContentAlignment.MiddleLeft,
                ForeColor = FormMain.Config.HintParameter,
                BackColor = Color.Transparent,
                Font = FormMain.Config.FontHintAdditionalText
            };

            fontRequirement = FormMain.Config.FontHintAdditionalText;

            // Пауза перед показом формы нужна, чтобы успела выполниться отрисовка изображения,
            // Иначе видно изменение текста невооруженным взглядом
            timerDelayShow = new Timer()
            {
                Interval = 300,
                Enabled = false
            };
            timerDelayShow.Tick += TimerDelayShow_Tick;

            Opacity = 0;
            Clear();
        }

        internal bool ExistHint { get; set; }

        protected override void Dispose(bool disposing)
        {
            timerDelayShow.Dispose();

            base.Dispose(disposing);
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

        private void TimerDelayShow_Tick(object sender, EventArgs e)
        {
            timerDelayShow.Enabled = false;
            Opacity = 0.8;
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

            ExistHint = true;
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
                    ForeColor = ColorRequirements(tr.Performed),
                    BackColor = Color.Transparent,
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
                lblGold.ForeColor = ColorRequirements(goldEnough);
                lblGold.Top = nextTop;
                lblGold.Text = "     " + gold.ToString();
                lblGold.Show();

                nextTop = GuiUtils.NextTop(lblGold);
            }
        }

        internal void AddStep5Builders(bool pointConstructionEnough)
        {
            if (!pointConstructionEnough)
            {
                lblBuilders.ForeColor = ColorRequirements(pointConstructionEnough);
                lblBuilders.Top = nextTop;
                lblBuilders.Text = "Cтроительство недоступно";
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
            lblDamageArcher.Text = "     " + (w.DamageRange > 0 ? w.DamageRange.ToString() : "");
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
            if (!ExistHint)
                return;

            Debug.Assert(c.Width > 8);
            Debug.Assert(c.Height > 8);
            Debug.Assert(lblHeader.Text.Length > 0);

            Point l = c.PointToScreen(new Point(0, c.Height + 2));
            // Если подсказка уходит за пределы экрана игры, меняем ее положение
            if (l.X + Width > Program.formMain.Location.X + Program.formMain.ClientSize.Width)
                l.X = Program.formMain.Location.X + Program.formMain.ClientSize.Width - Width;
            if (l.Y + nextTop > Program.formMain.Location.Y + Program.formMain.ClientSize.Height)
                l.Y = l.Y - nextTop - c.Height - 4;

            Location = l;

            bool needReshow = (Visible == false) || (Height != nextTop);
            Height = nextTop;
            bmpBackground?.Dispose();
            bmpBackground = GuiUtils.MakeBackgroundWithBorder(ClientSize, FormMain.Config.CommonBorder);

            //if (needReshow == true)
            {
                timerDelayShow.Enabled = true;
                Opacity = 0;
                Show();
            }
        }

        internal void ShowHint(VisualControl c)
        {
            if (!ExistHint)
                return;

            Debug.Assert(c.Width > 8);
            Debug.Assert(c.Height > 8);
            Debug.Assert(lblHeader.Text.Length > 0);

            Point l = Program.formMain.PointToScreen(new Point(c.Left, c.Top + c.Height + 2));
            // Если подсказка уходит за пределы экрана игры, меняем ее положение
            if (l.X + Width > Program.formMain.Location.X + Program.formMain.ClientSize.Width)
                l.X = Program.formMain.Location.X + Program.formMain.ClientSize.Width - Width;
            if (l.Y + nextTop > Program.formMain.Location.Y + Program.formMain.ClientSize.Height)
                l.Y = l.Y - nextTop - c.Height - 4;

            Location = l;

            bool needReshow = (Visible == false) || (Height != nextTop);
            Height = nextTop;
            bmpBackground?.Dispose();
            bmpBackground = GuiUtils.MakeBackgroundWithBorder(ClientSize, FormMain.Config.CommonBorder);

            //if (needReshow == true)
            {
                timerDelayShow.Enabled = true;
                Opacity = 0;
                Show();
            }
        }

        internal void HideHint()
        {
            timerDelayShow.Enabled = false;
            ExistHint = false;

            if (Visible)
                Hide();
        }

        private Color ColorRequirements(bool met)
        {
            return met ? FormMain.Config.HintRequirementsMet : FormMain.Config.HintRequirementsNotMet;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            HideHint();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            e.Graphics.DrawImage(bmpBackground, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }
    }

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
}
