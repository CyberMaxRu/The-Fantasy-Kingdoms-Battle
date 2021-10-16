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
        internal readonly VCCellSimple imgCell;
        internal readonly VCImage128 img128;
        internal readonly VCText lblType;
        internal readonly VCText lblAction;
        internal readonly VCText lblDescription;
        internal readonly VCSeparator lblSeparateRequirement;
        internal readonly VCLabel lblTextForRequirement;
        internal readonly List<VCText> listRequirements = new List<VCText>();
        internal readonly VCLabelValue lblIncome;
        internal readonly VCLabelValue lblGreatnessAdd;
        internal readonly VCLabelValue lblBuildersPerDay;
        internal readonly VCLabelValue lblGold;
        internal readonly VCLabelValue lblBuilders;
        internal readonly VCLabelValue lblHonor;
        internal readonly VCLabelValue lblEnthusiasm;
        internal readonly VCLabelValue lblMorale;
        internal readonly VCLabelValue lblLuck;
        private readonly List<VCLabelValue> listLabelNeeds = new List<VCLabelValue>();
        internal readonly VCLabel lblSigner;
        private readonly List<VCCellSimple> listCell = new List<VCCellSimple>();
        private readonly List<(VCCellSimple, VCLabel)> listPerks = new List<(VCCellSimple, VCLabel)>();
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

        private const int PANEL_WIDTH = 296;

        public PanelHint() : base()
        {
            ShowBorder = true;
            Width = PANEL_WIDTH;
            Visible = false;

            colorBackground = Color.FromArgb(192, 0, 0, 0);

            widthControl = Width - FormMain.Config.GridSize - FormMain.Config.GridSize;

            lblName = new VCText(this, FormMain.Config.GridSize, 4, Program.formMain.fontMedCaptionC, Color.SteelBlue, widthControl);
            lblName.StringFormat.Alignment = StringAlignment.Near;
            lblName.StringFormat.LineAlignment = StringAlignment.Near;

            lblHeader = new VCText(this, FormMain.Config.GridSize, 4, Program.formMain.fontMedCaptionC, Color.Yellow, widthControl);
            lblHeader.StringFormat.Alignment = StringAlignment.Near;
            lblHeader.StringFormat.LineAlignment = StringAlignment.Near;

            imgCell = new VCCellSimple(this, FormMain.Config.GridSize, lblHeader.NextTop());
            img128 = new VCImage128(this, FormMain.Config.GridSize, lblHeader.NextTop());

            lblType = new VCText(this, FormMain.Config.GridSize, lblHeader.NextTop(), Program.formMain.fontParagraphC, Color.DarkKhaki, widthControl);
            lblType.StringFormat.Alignment = StringAlignment.Near;
            lblType.StringFormat.LineAlignment = StringAlignment.Near;

            lblAction = new VCText(this, FormMain.Config.GridSize, lblType.NextTop(), Program.formMain.fontMedCaptionC, FormMain.Config.HintAction, widthControl);
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

            lblTextForRequirement = new VCLabel(this, FormMain.Config.GridSize, lblSeparateRequirement.NextTop(), Program.formMain.fontSmallC, Color.White, 16, "Требования:");
            lblTextForRequirement.Width = widthControl;
            lblTextForRequirement.StringFormat.Alignment = StringAlignment.Near;

            lblGold = new VCLabelValue(this, FormMain.Config.GridSize, lblTextForRequirement.NextTop(), FormMain.Config.HintIncome, false);
            lblGold.ImageIndex = FormMain.GUI_16_GOLD;
            lblGold.Width = widthControl;

            lblBuilders = new VCLabelValue(this, FormMain.Config.GridSize, lblGold.NextTop(), FormMain.Config.HintIncome, false);
            lblBuilders.ImageIndex = FormMain.GUI_16_BUILDER;
            lblBuilders.Width = widthControl;

            lblHonor = new VCLabelValue(this, FormMain.Config.GridSize, lblBuilders.NextTop(), FormMain.Config.HintIncome, false);
            lblHonor.ImageIndex = FormMain.GUI_16_HONOR;
            lblHonor.Width = widthControl;

            lblEnthusiasm = new VCLabelValue(this, FormMain.Config.GridSize, lblHonor.NextTop(), FormMain.Config.HintIncome, false);
            lblEnthusiasm.ImageIndex = FormMain.GUI_16_ENTHUSIASM;
            lblEnthusiasm.Width = widthControl;

            lblMorale = new VCLabelValue(this, FormMain.Config.GridSize, lblEnthusiasm.NextTop(), FormMain.Config.HintIncome, false);
            lblMorale.ImageIndex = FormMain.GUI_16_MORALE;
            lblMorale.Width = widthControl;

            lblLuck = new VCLabelValue(this, FormMain.Config.GridSize, lblMorale.NextTop(), FormMain.Config.HintIncome, false);
            lblLuck.ImageIndex = FormMain.GUI_16_LUCK;
            lblLuck.Width = widthControl;

            lblSigner = new VCLabel(this, FormMain.Config.GridSize, lblLuck.NextTop(), Program.formMain.fontSmallC, Color.SkyBlue, 16, "");
            lblSigner.StringFormat.Alignment = StringAlignment.Near;
            lblSigner.Width = widthControl;
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
            lblName.Text = "";
            lblHeader.Text = "";
            lblHeader.ShiftX = FormMain.Config.GridSize;
            imgCell.Visible = false;
            img128.Visible = false;
            lblType.Visible = false;
            lblAction.Visible = false;
            lblDescription.Visible = false;
            lblIncome.Visible = false;
            lblGreatnessAdd.Visible = false;
            lblBuildersPerDay.Visible = false;

            foreach (VCText l in listRequirements)
                l.Dispose();
            lblSeparateRequirement.Visible = false;
            lblTextForRequirement.Visible = false;
            listRequirements.Clear();

            lblGold.Visible = false;
            lblBuilders.Visible = false;
            lblHonor.Visible = false;
            lblEnthusiasm.Visible = false;
            lblMorale.Visible = false;
            lblLuck.Visible = false;

            foreach (VCLabelValue ln in listLabelNeeds)
                ln.Visible = false;

            lblSigner.Visible = false;

            foreach (VCCellSimple cell in listCell)
                cell.Visible = false;

            foreach ((VCCellSimple, VCLabel) cp in listPerks)
            {
                cp.Item1.Visible = false;
                cp.Item2.Visible = false;
            }

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
            Debug.Assert(lblHeader.Text.Length == 0);
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

        internal void AddStep1Name(string name)
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

        internal void AddStep2Header(string header, int imageIndexSmall = -1)
        {
            Debug.Assert(lblHeader.Text.Length == 0);
            if (header.Length == 0)
            {
                Debug.Assert(lblName.Visible);
            }

            Width = PANEL_WIDTH;
            ExistHint = true;

            if (header.Length > 0)
            {
                lblHeader.Color = Color.Yellow;
                lblHeader.ShiftY = nextTop;
                lblHeader.Width = widthControl;
                lblHeader.Text = header;
                lblHeader.Height = lblHeader.MinHeigth();
                nextTop = lblHeader.NextTop();
            }

            if (imageIndexSmall != -1)
            {
                imgCell.ShiftY = lblHeader.ShiftY;
                imgCell.ImageIndex = imageIndexSmall;
                imgCell.Visible = true;

                lblHeader.ShiftX = imgCell.NextLeft();
                nextTop = Math.Max(lblHeader.NextTop(), imgCell.NextTop());
            }
        }

        internal void AddStep3Type(string type)
        {
            if (type.Length > 0)
            {
                lblType.ShiftX = lblHeader.ShiftX;
                lblType.ShiftY = lblHeader.NextTop() - FormMain.Config.GridSize;
                lblType.Text = type;
                lblType.Height = lblType.MinHeigth();
                lblType.Visible = true;

                nextTop = imgCell.Visible ? Math.Max(lblType.NextTop(), imgCell.NextTop()) : lblType.NextTop();
            }
        }

        internal void AddStep4Level(string level, int imageIndexBig = -1)
        {
            if (level.Length > 0)
            {
                lblAction.ShiftY = nextTop;
                lblAction.Text = level;
                lblAction.Height = lblAction.MinHeigth();
                lblAction.Visible = true;

                nextTop = lblAction.NextTop();
            }

            if (imageIndexBig != -1)
            {
                img128.ShiftY = nextTop;
                img128.ImageIndex = imageIndexBig;
                img128.Visible = true;
                nextTop = img128.NextTop();
            }
        }

        internal void AddStep5Description(string description)
        {
            if (description.Length > 0)
            {
                lblDescription.ShiftY = nextTop;
                lblDescription.Text = description;
                lblDescription.Height = lblDescription.MinHeigth();
                lblDescription.Visible = true;

                nextTop = lblDescription.NextTop();
            }
        }

        internal void AddStep6Income(int income)
        {
            if (income > 0)
            {
                lblIncome.ShiftY = nextTop;
                lblIncome.Text = $"+{income}/день";
                lblIncome.Visible = true;

                nextTop = lblIncome.NextTop();
            }
        }

        internal void AddStep7Reward(int reward)
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

        internal void AddStep8Greatness(int addGreatness, int greatnessPerDay)
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

        internal void AddStep9PlusBuilders(int buildersPerDay)
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

        internal void AddStep9Honor(int honor)
        {
            if (honor != 0)
            {
                lblHonor.ShiftY = nextTop;
                lblHonor.Text = Utils.DecIntegerBy10(honor, true);
                lblHonor.Visible = true;

                nextTop = lblHonor.NextTop();
            }
        }

        internal void AddStep9Enthusiasm(int enthusiasm)
        {
            if (enthusiasm != 0)
            {
                lblEnthusiasm.ShiftY = nextTop;
                lblEnthusiasm.Text = Utils.DecIntegerBy10(enthusiasm, true);
                lblEnthusiasm.Visible = true;

                nextTop = lblEnthusiasm.NextTop();
            }
        }

        internal void AddStep9Morale(int morale)
        {
            if (morale != 0)
            {
                lblMorale.ShiftY = nextTop;
                lblMorale.Text = Utils.DecIntegerBy10(morale, true);
                lblMorale.Visible = true;

                nextTop = lblMorale.NextTop();
            }
        }

        internal void AddStep9Luck(int luck)
        {
            if (luck != 0)
            {
                lblLuck.ShiftY = nextTop;
                lblLuck.Text = Utils.FormatInteger(luck);
                lblLuck.Visible = true;

                nextTop = lblLuck.NextTop();
            }
        }
        internal void AddStep9ListNeeds(int[] array)
        {
            if ((array != null) && (array.Length > 0))
            {
                VCLabelValue lv;

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != 0)
                    {
                        lv = GetLabel(i);
                        lv.Visible = true;
                        lv.ImageIndex = FormMain.Config.NeedsCreature[i].ImageIndex;
                        lv.Text = Utils.DecIntegerBy10(array[i]);
                        lv.ShiftY = nextTop;

                        nextTop = lv.NextTop();
                    }
                }
            }

            VCLabelValue GetLabel(int index)
            {
                if (index < listLabelNeeds.Count)
                    return listLabelNeeds[index];
                else
                {
                    VCLabelValue l = new VCLabelValue(this, FormMain.Config.GridSize, 0, Color.White, false);
                    l.Width = widthControl;
                    listLabelNeeds.Add(l);
                    return l;
                }
            }
        }

        internal void AddStep9ListNeeds(List<(DescriptorNeed, int)> list)
        {
            if (list.Count > 0)
            {
                (DescriptorNeed, int) need;
                VCLabelValue lv;

                for (int i = 0; i < list.Count; i++)
                {
                    need = list[i];
                    Debug.Assert(need.Item2 != 0);

                    lv = GetLabel(i);
                    lv.Visible = true;
                    lv.ImageIndex = need.Item1.ImageIndex;
                    lv.Text = Utils.DecIntegerBy10(need.Item2);
                    lv.ShiftY = nextTop;

                    nextTop = lv.NextTop();
                }
            }

            VCLabelValue GetLabel(int index)
            {
                if (index < listLabelNeeds.Count)
                    return listLabelNeeds[index];
                else
                {
                    VCLabelValue l = new VCLabelValue(this, FormMain.Config.GridSize, 0, Color.White, false);
                    l.Width = widthControl;
                    listLabelNeeds.Add(l);
                    return l;
                }
            }
        }

        internal void AddStep10Requirement(string notEnoughrequirement)
        {
            List<TextRequirement> r = new List<TextRequirement>();
            r.Add(new TextRequirement(false, notEnoughrequirement));
            AddStep11Requirement(r);
        }

        internal void AddStep11Requirement(List<TextRequirement> requirement)
        {
            Debug.Assert(requirement != null);
            if (requirement.Count > 0)
            {
                lblSeparateRequirement.Visible = true;
                lblSeparateRequirement.ShiftY = nextTop;
                nextTop = lblSeparateRequirement.NextTop();
                lblTextForRequirement.Visible = true;
                lblTextForRequirement.ShiftY = nextTop;
                nextTop = lblTextForRequirement.NextTop();

                VCText lr;
                foreach (TextRequirement tr in requirement)
                {
                    lr = new VCText(this, FormMain.Config.GridSize * 4, nextTop, Program.formMain.fontSmallC, ColorRequirements(tr.Performed), widthControl - FormMain.Config.GridSize * 3);
                    lr.StringFormat.Alignment = StringAlignment.Near;
                    lr.Text = tr.Text;
                    lr.Height = lr.MinHeigth();
                    //lr.MaximumSize = new Size(Width - FormMain.Config.GridSize * 2, 0);

                    listRequirements.Add(lr);
                    nextTop = lr.NextTop();
                }
            }
        }

        internal void AddStep12Gold(int gold, bool goldEnough)
        {
            if (gold > 0)
            {
                if (!lblSeparateRequirement.Visible)
                {
                    lblSeparateRequirement.Visible = true;
                    lblSeparateRequirement.ShiftY = nextTop;
                    nextTop = lblSeparateRequirement.NextTop();
                    lblTextForRequirement.Visible = true;
                    lblTextForRequirement.ShiftY = nextTop;
                    nextTop = lblTextForRequirement.NextTop();
                }

                lblGold.Color = ColorRequirements(goldEnough);
                lblGold.Text = gold.ToString();
                lblGold.ShiftY = nextTop;
                lblGold.Visible = true;

                nextTop = lblGold.NextTop();
            }
        }

        internal void AddStep13Builders(int builders, bool buildersEnough)
        {
            if (builders > 0)
            {
                if (!lblSeparateRequirement.Visible)
                {
                    lblSeparateRequirement.Visible = true;
                    lblSeparateRequirement.ShiftY = nextTop;
                    nextTop = lblSeparateRequirement.NextTop();
                    lblTextForRequirement.Visible = true;
                    lblTextForRequirement.ShiftY = nextTop;
                    nextTop = lblTextForRequirement.NextTop();
                }

                lblBuilders.Color = ColorRequirements(buildersEnough);
                lblBuilders.Text = builders.ToString();
                lblBuilders.ShiftY = nextTop;
                lblBuilders.Visible = true;

                nextTop = lblBuilders.NextTop();
            }
        }

        internal void AddStep14PlayerItem(Item pi)
        {
            Debug.Assert(pi != null);

        }

        internal void AddStep15Weapon(DescriptorItem w)
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

        internal void AddStep16Armour(DescriptorItem a)
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

        internal void AddStep17Signer(Hero signer)
        {
            if (signer != null)
            {
                lblSigner.ShiftY = nextTop;
                lblSigner.Text = $"Подписал: {signer.GetNameHero()}";
                lblSigner.Visible = true;

                nextTop = lblSigner.NextTop();
            }
        }

        internal void AddStep18Owner(Entity owner = null)
        {
            if (owner != null)
            {
                string source = null;
                if (owner is Item i)
                    source = i.Descriptor.Name;
                else if (owner is Construction c)
                    source = c.TypeConstruction.Name + (c.TypeConstruction.MaxLevel > 1 ? $" ({c.Level} ур.)" : "");
                else if (owner is Creature crt)
                    source = crt.TypeCreature.Name;
                Debug.Assert(source != null);

                lblSigner.ShiftY = nextTop;
                lblSigner.Text = $"От: {source}";
                lblSigner.Visible = true;

                nextTop = lblSigner.NextTop();
            }
        }

        internal void AddStep19Descriptors(List<(DescriptorEntity, string)> list)
        {
            if (list.Count > 0)
            {
                VCCellSimple cell = null;
                int nextLeft = FormMain.Config.GridSize;

                for (int i = 0; i < list.Count; i++)
                {
                    cell = GetCell(i);
                    cell.Visible = true;
                    cell.Descriptor = list[i].Item1;
                    cell.Text = list[i].Item2;
                    cell.ShiftY = nextTop;
                    cell.ShiftX = nextLeft;
                    nextLeft = cell.NextLeft();
                    if (nextLeft > Width)
                    {
                        nextLeft = 0;
                        nextTop = cell.NextTop();
                    }
                }

                nextTop = cell.NextTop() + 4;

                VCCellSimple GetCell(int index)
                {
                    if (index < listCell.Count)
                        return listCell[index];
                    else
                    {
                        VCCellSimple c = new VCCellSimple(this, 0, 0);
                        listCell.Add(c);
                        return c;
                    }
                }
            }
        }

        internal void AddStep19Descriptors(List<DescriptorItem> list)
        {
            if (list.Count > 0)
            {
                VCCellSimple cell = null;
                int nextLeft = FormMain.Config.GridSize;

                for (int i = 0; i < list.Count; i++)
                {
                    cell = GetCell(i);
                    cell.Visible = true;
                    cell.Descriptor = list[i];
                    cell.ShiftY = nextTop;
                    cell.ShiftX = nextLeft;
                    nextLeft = cell.NextLeft();
                    if (nextLeft > Width)
                    {
                        nextLeft = 0;
                        nextTop = cell.NextTop();
                    }
                }

                nextTop = cell.NextLeft() + 4;

                VCCellSimple GetCell(int index)
                {
                    if (index < listCell.Count)
                        return listCell[index];
                    else
                    {
                        VCCellSimple c = new VCCellSimple(this, 0, 0);
                        listCell.Add(c);
                        return c;
                    }
                }
            }
        }

        internal void AddStep20Perks(List<DescriptorPerk> list)
        {
            if (list.Count > 0)
            {
                (VCCellSimple, VCLabel) cell = (null, null);

                for (int i = 0; i < list.Count; i++)
                {
                    cell = GetCell(i);
                    cell.Item1.Visible = true;
                    cell.Item1.Descriptor = list[i];
                    cell.Item1.ShiftY = nextTop;

                    cell.Item2.Visible = true;
                    cell.Item2.Text = list[i].Name;
                    cell.Item2.ShiftY = cell.Item1.ShiftY;

                    nextTop = cell.Item1.NextTop();
                }

                (VCCellSimple, VCLabel) GetCell(int index)
                {
                    if (index < listCell.Count)
                        return listPerks[index];
                    else
                    {
                        (VCCellSimple, VCLabel) c = (new VCCellSimple(this, FormMain.Config.GridSize, 0), new VCLabel(this, 0, 0, Program.formMain.fontSmallC, Color.White, 48, ""));
                        c.Item2.ShiftX = c.Item1.NextLeft();
                        c.Item2.Width = Width - c.Item2.ShiftX - FormMain.Config.GridSize;
                        c.Item2.StringFormat.Alignment = StringAlignment.Near;
                        c.Item2.StringFormat.LineAlignment = StringAlignment.Center;
                        listPerks.Add(c);
                        return c;
                    }
                }
            }
        }

        internal void DrawHint(VisualControl c)
        {
            if (!ExistHint)
                return;

            Debug.Assert(c.Visible);
            Debug.Assert(c.Width > 8);
            Debug.Assert(c.Height > 8);
            Debug.Assert((lblName.Text.Length > 0) || (lblHeader.Text.Length > 0));

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
