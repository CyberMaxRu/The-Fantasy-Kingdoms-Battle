using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
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

    // Панель подсказки
    internal sealed class PanelHint : VisualControl
    {
        internal readonly VCText lblName;
        internal readonly VCText lblHeader;
        internal readonly VCText lblAction;
        internal readonly VCText lblDescription;
        internal readonly VCSeparator lblSeparateRequirement;
        internal readonly List<VCText> listRequirements = new List<VCText>();
        internal readonly VCLabelValue lblIncome;
        internal readonly VCLabelValue lblGreatnessAdd;
        internal readonly VCLabelValue lblBuildersPerDay;
        internal readonly VCLabelValue lblGold;
        internal readonly VCLabelValue lblBuilders;
        internal readonly VCLabel lblDamageMelee;
        internal readonly VCLabel lblDamageArcher;
        internal readonly VCLabel lblDamageMagic;
        internal readonly VCLabel lblDefenseMelee;
        internal readonly VCLabel lblDefenseArcher;
        internal readonly VCLabel lblDefenseMagic;
        private int nextTop;
        private readonly Color colorBackground;
        private Bitmap bmpBackground;
        private int widthControl;

        public PanelHint() : base()
        {
            ShowBorder = true;
            Width = 256;
            Visible = false;

            colorBackground = Color.FromArgb(192, 0, 0, 0);

            widthControl = Width - FormMain.Config.GridSize - FormMain.Config.GridSize;

            lblName = new VCText(this, FormMain.Config.GridSize, 4, Program.formMain.fontMedCaptionC, Color.SteelBlue, widthControl);
            lblName.StringFormat.Alignment = StringAlignment.Near;
            lblName.StringFormat.LineAlignment = StringAlignment.Near;

            lblHeader = new VCText(this, FormMain.Config.GridSize, 4, Program.formMain.fontMedCaptionC, Color.Yellow, widthControl);
            lblHeader.StringFormat.Alignment = StringAlignment.Near;
            lblHeader.StringFormat.LineAlignment = StringAlignment.Near;

            lblAction = new VCText(this, FormMain.Config.GridSize, lblHeader.NextTop(), Program.formMain.fontMedCaptionC, FormMain.Config.HintAction, widthControl);
            lblAction.StringFormat.Alignment = StringAlignment.Near;
            lblAction.StringFormat.LineAlignment = StringAlignment.Near;

            lblDescription = new VCText(this, FormMain.Config.GridSize, lblAction.NextTop(), Program.formMain.fontSmallC, FormMain.Config.HintDescription, widthControl);
            lblDescription.StringFormat.Alignment = StringAlignment.Near;
            lblDescription.StringFormat.LineAlignment = StringAlignment.Near;

            lblIncome = new VCLabelValue(this, FormMain.Config.GridSize, lblDescription.NextTop(), FormMain.Config.HintIncome, false);
            lblIncome.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.Width = widthControl;

            lblGreatnessAdd = new VCLabelValue(this, FormMain.Config.GridSize, lblIncome.NextTop(), FormMain.Config.HintIncome, false);
            lblGreatnessAdd.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblGreatnessAdd.Width = widthControl;

            lblBuildersPerDay = new VCLabelValue(this, FormMain.Config.GridSize, lblGreatnessAdd.NextTop(), FormMain.Config.HintIncome, false);
            lblBuildersPerDay.ImageIndex = FormMain.GUI_16_BUILDER;
            lblBuildersPerDay.Width = widthControl;

            lblSeparateRequirement = new VCSeparator(this, FormMain.Config.GridSize, lblBuildersPerDay.NextTop());
            lblSeparateRequirement.Width = widthControl;
            lblGold = new VCLabelValue(this, FormMain.Config.GridSize, lblSeparateRequirement.NextTop(), FormMain.Config.HintIncome, false);
            lblGold.ImageIndex = FormMain.GUI_16_GOLD;
            lblGold.Width = widthControl;
            lblBuilders = new VCLabelValue(this, FormMain.Config.GridSize, lblGold.NextTop(), FormMain.Config.HintIncome, false);
            lblBuilders.ImageIndex = FormMain.GUI_16_BUILDER;
            lblBuilders.Width = widthControl;

            /*            lblDamageMelee = new Label()
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

                        lblDamageMagic = new Label()
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
                        };*/

            Clear();
        }

        internal bool ExistHint { get; set; }
        internal void Clear()
        {
            ExistHint = false;
            lblName.Visible = false;
            lblHeader.Text = "";
            lblAction.Visible = false;
            lblDescription.Visible = false;
            lblIncome.Visible = false;
            lblGreatnessAdd.Visible = false;
            lblBuildersPerDay.Visible = false;

            foreach (VCText l in listRequirements)
                l.Dispose();
            lblSeparateRequirement.Visible = false;
            listRequirements.Clear();

            lblGold.Visible = false;
            lblBuilders.Visible = false;

            /*lblDamageMelee.Hide();
            lblDamageArcher.Hide();
            lblDamageMagic.Hide();
            lblDefenseMelee.Hide();
            lblDefenseArcher.Hide();
            lblDefenseMagic.Hide();*/

            nextTop = FormMain.Config.GridSize;
        }

        internal void AddSimpleHint(string header)
        {
            Debug.Assert(lblHeader.Text == "");
            Debug.Assert(header != null);
            Debug.Assert(header.Length > 0);

            ExistHint = true;
            lblHeader.Color = Color.White;
            lblHeader.ShiftY = nextTop;
            lblHeader.Width = widthControl;
            lblHeader.Text = header;
            lblHeader.Height = lblHeader.MinHeigth();
            nextTop = lblHeader.NextTop();
            lblHeader.Width = lblHeader.MinWidth();
            Width = lblHeader.ShiftX + lblHeader.MinWidth() + FormMain.Config.GridSize;
        }

        internal void AddStep0Name(string name)
        {
            Debug.Assert(!lblName.Visible);
            Debug.Assert(name != null);
            Debug.Assert(name.Length > 0);

            lblName.ShiftY = nextTop;
            lblName.Text = name;
            lblName.Height = lblName.MinHeigth();
            lblName.Visible = true;

            nextTop = lblName.NextTop();
        }

        internal void AddStep1Header(string header, string action, string description)
        {
            Debug.Assert(lblHeader.Text == "");
            Debug.Assert(header != null);
            Debug.Assert(header.Length > 0);

            Width = 256;
            ExistHint = true;
            lblHeader.Color = Color.Yellow;
            lblHeader.ShiftY = nextTop;
            lblHeader.Width = widthControl;
            lblHeader.Text = header;
            lblHeader.Height = lblHeader.MinHeigth();
            nextTop = lblHeader.NextTop();

            if (action.Length > 0)
            { 
                lblAction.ShiftY = nextTop;
                lblAction.Text = action;
                lblAction.Height = lblAction.MinHeigth();
                lblAction.Visible = true;

                nextTop = lblAction.NextTop();
            }

            if (description.Length > 0)
            {
                lblDescription.ShiftY = nextTop;
                lblDescription.Text = description;
                lblDescription.Height = lblDescription.MinHeigth();
                lblDescription.Visible = true;

                nextTop = lblDescription.NextTop();
            }
        }

        internal void AddStep2Income(int income)
        {
            if (income > 0)
            {
                lblIncome.ShiftY = nextTop;
                lblIncome.Text = $"+{income}/день";
                lblIncome.Visible = true;

                nextTop = lblIncome.NextTop();
            }
        }

        internal void AddStep2Reward(int reward)
        {
            Debug.Assert(reward >= 0);

            if (reward > 0)
            {
                Debug.Assert(!lblIncome.Visible);

                lblIncome.ShiftY = nextTop;
                lblIncome.Text = reward.ToString();
                lblIncome.Visible = true;

                nextTop = lblIncome.NextTop();
            }
        }

        internal void AddStep3Greatness(int addGreatness, int greatnessPerDay)
        {
            Debug.Assert(addGreatness >= 0);
            Debug.Assert(greatnessPerDay >= 0);

            if ((addGreatness > 0) || (greatnessPerDay > 0))
            {
                lblGreatnessAdd.ShiftY = nextTop;
                lblGreatnessAdd.Visible = true;
                lblGreatnessAdd.Text = Utils.FormatGreatness(addGreatness, greatnessPerDay);
                nextTop = lblGreatnessAdd.NextTop();
            }
        }

        internal void AddStep35PlusBuilders(int buildersPerDay)
        {
            Debug.Assert(buildersPerDay >= 0);

            if (buildersPerDay > 0)
            {
                lblBuildersPerDay.ShiftY = nextTop;
                lblBuildersPerDay.Text = $"+{buildersPerDay}";
                lblBuildersPerDay.Visible = true;

                nextTop = lblBuildersPerDay.NextTop();
            }
        }

        internal void AddStep3Requirement(string notEnoughrequirement)
        {
            List<TextRequirement> r = new List<TextRequirement>();
            r.Add(new TextRequirement(false, notEnoughrequirement));
            AddStep3Requirement(r);
        }

        internal void AddStep3Requirement(List<TextRequirement> requirement)
        {
            Debug.Assert(requirement != null);
            if (requirement.Count > 0)
            {
                lblSeparateRequirement.Visible = true;
                lblSeparateRequirement.ShiftY = nextTop;
                nextTop = lblSeparateRequirement.NextTop();

                VCText lr;
                foreach (TextRequirement tr in requirement)
                {
                    lr = new VCText(this, FormMain.Config.GridSize, nextTop, Program.formMain.fontSmallC, ColorRequirements(tr.Performed), widthControl);
                    lr.StringFormat.Alignment = StringAlignment.Near;
                    lr.Text = tr.Text;
                    lr.Height = lr.MinHeigth();
                    //lr.MaximumSize = new Size(Width - FormMain.Config.GridSize * 2, 0);

                    listRequirements.Add(lr);
                    nextTop = lr.NextTop();
                }
            }
        }

        internal void AddStep4Gold(int gold, bool goldEnough)
        {
            if (gold > 0)
            {
                if (!lblSeparateRequirement.Visible)
                {
                    lblSeparateRequirement.Visible = true;
                    lblSeparateRequirement.ShiftY = nextTop;
                    nextTop = lblSeparateRequirement.NextTop();
                }

                lblGold.Color = ColorRequirements(goldEnough);
                lblGold.Text = gold.ToString();
                lblGold.ShiftY = nextTop;
                lblGold.Visible = true;

                nextTop = lblGold.NextTop();
            }
        }

        internal void AddStep5Builders(int builders, bool buildersEnough)
        {
            if (builders > 0)
            {
                if (!lblSeparateRequirement.Visible)
                {
                    lblSeparateRequirement.Visible = true;
                    lblSeparateRequirement.ShiftY = nextTop;
                    nextTop = lblSeparateRequirement.NextTop();
                }

                lblBuilders.Color = ColorRequirements(buildersEnough);
                lblBuilders.Text = builders.ToString();
                lblBuilders.ShiftY = nextTop;
                lblBuilders.Visible = true;

                nextTop = lblBuilders.NextTop();
            }
        }

        internal void AddStep6PlayerItem(PlayerItem pi)
        {
            Debug.Assert(pi != null);

        }

        internal void AddStep7Weapon(Item w)
        {
            Debug.Assert(w != null);

            /*lblDamageMelee.Top = nextTop;
            lblDamageArcher.Top = nextTop;
            lblDamageMagic.Top = nextTop;

            lblDamageMelee.Text = "     " + (w.DamageMelee > 0 ? w.DamageMelee.ToString() : "");
            lblDamageArcher.Text = "     " + (w.DamageRange > 0 ? w.DamageRange.ToString() : "");
            lblDamageMagic.Text = "     " + (w.DamageMagic > 0 ? w.DamageMagic.ToString() : "");

            lblDamageMelee.Show();
            lblDamageArcher.Show();
            lblDamageMagic.Show();

            nextTop = GuiUtils.NextTop(lblDamageMelee);*/
        }

        internal void AddStep8Armour(Item a)
        {
            Debug.Assert(a != null);

            /*lblDefenseMelee.Top = nextTop;
            lblDefenseArcher.Top = nextTop;
            lblDefenseMagic.Top = nextTop;

            lblDefenseMelee.Text = "     " + (a.DefenseMelee > 0 ? a.DefenseMelee.ToString() : "");
            lblDefenseArcher.Text = "     " + (a.DefenseArcher > 0 ? a.DefenseArcher.ToString() : "");
            lblDefenseMagic.Text = "     " + (a.DefenseMagic > 0 ? a.DefenseMagic.ToString() : "");

            lblDefenseMelee.Show();
            lblDefenseArcher.Show();
            lblDefenseMagic.Show();

            nextTop = GuiUtils.NextTop(lblDefenseMelee);*/
        }

        internal void DrawHint(VisualControl c)
        {
            if (!ExistHint)
                return;

            Debug.Assert(c.Visible);
            Debug.Assert(c.Width > 8);
            Debug.Assert(c.Height > 8);
            Debug.Assert(lblHeader.Text.Length > 0);

            Height = nextTop;

            Point l = new Point(c.Left, c.Top + c.Height + 4);
            // Если подсказка уходит за пределы экрана игры, меняем ее положение
            if (l.X + Width > Program.formMain.sizeGamespace.Width - FormMain.Config.GridSize)
                l.X = Program.formMain.sizeGamespace.Width - Width - FormMain.Config.GridSize;
            if (l.Y + Height > Program.formMain.sizeGamespace.Height - FormMain.Config.GridSize)
                l.Y = l.Y - Height - c.Height - 7;

            // Если подсказка не помещается ни снизу, ни сверху, показываем ее справа или слева
            if (l.Y < 0)
            {
                l.Y = Program.formMain.sizeGamespace.Height - FormMain.Config.GridSize - Height;
                // Здесь нужна проверка на то, где рисовать - слева или справа
                l.X = c.Left + c.Width + 4;
            }

            // Сначала меняем высоту, а потом меням координату, чтобы при ArrangeControls не срабатывал Assert
            SetPos(l.X, l.Y);

            bmpBackground?.Dispose();
            bmpBackground = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(bmpBackground);
            g.Clear(colorBackground);
            g.Dispose();

            ArrangeControls();
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(ExistHint);

            g.DrawImageUnscaled(bmpBackground, Left, Top);

            base.Draw(g);
        }

        internal void HideHint()
        {
            ExistHint = false;

            if (Visible)
                Visible = false;
        }

        private Color ColorRequirements(bool met)
        {
            return met ? FormMain.Config.HintRequirementsMet : FormMain.Config.HintRequirementsNotMet;
        }
    }
}
