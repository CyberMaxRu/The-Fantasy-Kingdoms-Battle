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
        internal readonly VCTextM2 lblHeader;
        internal readonly VCTextM2 lblAction;
        internal readonly VCTextM2 lblDescription;
        internal readonly VCSeparator lblSeparateRequirement;
        internal readonly VCLabelM2 lblRequirement;
        internal readonly List<VCLabelM2> listRequirements = new List<VCLabelM2>();
        internal readonly VCLabelValue lblIncome;
        internal readonly VCLabelValue lblGold;
        internal readonly VCLabelM2 lblBuilders;
        internal readonly VCLabelM2 lblDamageMelee;
        internal readonly VCLabelM2 lblDamageArcher;
        internal readonly VCLabelM2 lblDamageMagic;
        internal readonly VCLabelM2 lblDefenseMelee;
        internal readonly VCLabelM2 lblDefenseArcher;
        internal readonly VCLabelM2 lblDefenseMagic;
        private int nextTop;
        private readonly Color colorBackground;
        private Bitmap bmpBackground;
        
        public PanelHint() : base()
        {
            ShowBorder = true;
            Width = 256;
            Visible = false;

            colorBackground = Color.FromArgb(192, 0, 0, 0);

            lblHeader = new VCTextM2(this, FormMain.Config.GridSize, 4, Program.formMain.fontMedCaptionC, Color.Yellow, Width - FormMain.Config.GridSize - FormMain.Config.GridSize);
            lblHeader.StringFormat.Alignment = StringAlignment.Near;
            lblHeader.StringFormat.LineAlignment = StringAlignment.Near;

            lblAction = new VCTextM2(this, FormMain.Config.GridSize, lblHeader.NextTop(), Program.formMain.fontMedCaptionC, FormMain.Config.HintAction, Width - FormMain.Config.GridSize - FormMain.Config.GridSize);
            lblAction.StringFormat.Alignment = StringAlignment.Near;
            lblAction.StringFormat.LineAlignment = StringAlignment.Near;

            lblDescription = new VCTextM2(this, FormMain.Config.GridSize, lblAction.NextTop(), Program.formMain.fontSmallC, FormMain.Config.HintDescription, Width - FormMain.Config.GridSize - FormMain.Config.GridSize);
            lblDescription.StringFormat.Alignment = StringAlignment.Near;
            lblDescription.StringFormat.LineAlignment = StringAlignment.Near;

            lblIncome = new VCLabelValue(this, FormMain.Config.GridSize, lblDescription.NextTop(), FormMain.Config.HintIncome);
            lblIncome.ImageIndex = FormMain.GUI_16_INCOME;

            lblSeparateRequirement = new VCSeparator(this, FormMain.Config.GridSize, lblIncome.NextTop());
            lblSeparateRequirement.Width = Width - FormMain.Config.GridSize - FormMain.Config.GridSize;
            lblRequirement = new VCLabelValue(this, FormMain.Config.GridSize, lblSeparateRequirement.NextTop(), FormMain.Config.HintIncome);
            lblRequirement.StringFormat.Alignment = StringAlignment.Center;
            lblRequirement.Width = lblSeparateRequirement.Width;
            lblGold = new VCLabelValue(this, FormMain.Config.GridSize, lblRequirement.NextTop(), FormMain.Config.HintIncome);
            lblGold.ImageIndex = FormMain.GUI_16_GOLD;

            lblBuilders = new VCLabelM2(this, FormMain.Config.GridSize, lblGold.NextTop(), Program.formMain.fontSmallC, FormMain.Config.HintDescription, 16, "");

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
            lblHeader.Text = "";
            lblAction.Visible = false;
            lblDescription.Visible = false;
            lblIncome.Visible = false;

            foreach (VCLabelM2 l in listRequirements)
                l.Dispose();
            lblSeparateRequirement.Visible = false;
            lblRequirement.Visible = false;
            listRequirements.Clear();

            lblGold.Visible = false;
            lblBuilders.Visible = false;

            /*lblDamageMelee.Hide();
            lblDamageArcher.Hide();
            lblDamageMagic.Hide();
            lblDefenseMelee.Hide();
            lblDefenseArcher.Hide();
            lblDefenseMagic.Hide();*/

            nextTop = lblHeader.NextTop();
        }

        internal void AddHeader(string header)
        {
            ExistHint = true;
            lblHeader.Text = header;
            nextTop = lblHeader.ShiftY + lblHeader.MinHeigth() + FormMain.Config.GridSize;
            Width = lblHeader.ShiftX + lblHeader.MinWidth() + FormMain.Config.GridSize;
        }

        internal void AddStep1Header(string header, string action, string description)
        {
            Debug.Assert(header.Length > 0);

            Width = 256;
            ExistHint = true;
            lblHeader.Text = header;
            nextTop = lblHeader.ShiftY + lblHeader.MinHeigth() + FormMain.Config.GridSize;

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
                lblIncome.Text = "+" + income.ToString();
                lblIncome.Visible = true;

                nextTop = lblIncome.NextTop();
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
                lblRequirement.Visible = true;
                lblRequirement.ShiftY = nextTop;
                nextTop = lblRequirement.NextTop();

                VCLabelM2 lr;
                bool requirementPerformed = true;
                foreach (TextRequirement tr in requirement)
                {
                    lr = new VCLabelM2(this, FormMain.Config.GridSize, nextTop, Program.formMain.fontSmallC, ColorRequirements(tr.Performed), 16, tr.Text);
                    lr.StringFormat.Alignment = StringAlignment.Near;
                    //lr.MaximumSize = new Size(Width - FormMain.Config.GridSize * 2, 0);

                    listRequirements.Add(lr);
                    nextTop = lr.NextTop();

                    if (requirementPerformed && !tr.Performed)
                        requirementPerformed = false;
                }
                lblRequirement.Text = requirementPerformed ? "Требования выполнены" : "Требования не выполнены  ";
                lblRequirement.Color = ColorRequirements(requirementPerformed);
            }
        }

        internal void AddStep4Gold(int gold, bool goldEnough)
        {
            if (gold > 0)
            {
                lblGold.Color = ColorRequirements(goldEnough);
                lblGold.Text = gold.ToString();
                lblGold.ShiftY = nextTop;
                lblGold.Visible = true;

                nextTop = lblGold.NextTop();
            }
        }

        internal void AddStep5Builders(bool pointConstructionEnough)
        {
            if (!pointConstructionEnough)
            {
                lblBuilders.Color = ColorRequirements(pointConstructionEnough);
                lblBuilders.Text = "Cтроительство недоступно";
                lblBuilders.ShiftY = nextTop;
                lblBuilders.Visible = true;

                nextTop = lblBuilders.NextTop();
            }
        }

        internal void AddStep6PlayerItem(PlayerItem pi)
        {
            Debug.Assert(pi != null);

        }

        internal void AddStep7Weapon(Weapon w)
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

        internal void AddStep8Armour(Armour a)
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

        internal void DrawHint(VisualControl c, bool delayShow = true)
        {
            if (!ExistHint)
                return;

            Debug.Assert(c.Width > 8);
            Debug.Assert(c.Height > 8);
            Debug.Assert(lblHeader.Text.Length > 0);

            Point l = new Point(c.Left, c.Top + c.Height + 2);
            // Если подсказка уходит за пределы экрана игры, меняем ее положение
            if (l.X + Width > Program.formMain.Location.X + Program.formMain.ClientSize.Width)
                l.X = Program.formMain.Location.X + Program.formMain.ClientSize.Width - Width;
            if (l.Y + nextTop > Program.formMain.Location.Y + Program.formMain.ClientSize.Height)
                l.Y = l.Y - nextTop - c.Height - 4;

            SetPos(l.X + Program.formMain.ShiftControls.X, l.Y + Program.formMain.ShiftControls.Y);

            bool needReshow = (Visible == false) || (Height != nextTop);
            Height = nextTop;

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
