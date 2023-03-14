using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.Utils;

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



    internal sealed class HintListCustomCells : List<(int ImageIndex, string Text, Color Color)>
    {

    }

    // Панель подсказки
    internal sealed class PanelHint : VisualControl
    {
        private readonly VCText lblHeader;
        private readonly VCCellSimple imgCell;
        private readonly VCText lblType;
        private readonly VCText lblAction;
        private readonly VCText lblState;
        private readonly VCText lblDescription;
        private readonly VCLabelValue lblDurability;
        private readonly VCLabelValue lblIncome;
        private readonly VCLabelValue lblSalary;
        private readonly VCLabelValue lblGreatnessAdd;
        private readonly VCLabelValue lblBuildersPerDay;

        // Изменение параметров населенного пункта
        private readonly List<VCLabelValue> listSettlementParameters = new List<VCLabelValue>();
        private readonly VCLabel lblSettlementParameters;

        //
        private readonly List<VCLabelValue> listProperties = new List<VCLabelValue>();
        private readonly VCLabelValue lblInterest;

        // Раздел "Выполнение"
        private readonly VCSeparator sepExecuting;
        private readonly VCText lblNameExecuting;

        // Подраздел "Стоимость"
        private readonly VCLabel lblChapterCost;
        private readonly VCLabelValue lblCostGold;
        private readonly VCLabelValue lblBuilders;
        private readonly VCLabelValue lblTimeExecuting;
        private readonly List<VCLabelValue> listCostResources = new List<VCLabelValue>();

        // Подраздел "Требуется"
        private readonly VCLabel lblTextForRequirement;
        private readonly List<VCText> listRequirements = new List<VCText>();


        //
        private readonly List<VCLabelValue> listLabelNeeds = new List<VCLabelValue>();
        private readonly VCLabel lblSigner;
        private readonly VCSeparator lblSeparateTooltip;
        private readonly VCText lblTooltip;
        private readonly List<VCCellSimple> listCell = new List<VCCellSimple>();
        private readonly List<(VCCellSimple, VCLabel)> listPerks = new List<(VCCellSimple, VCLabel)>();
        private readonly List<(VCCellSimple, VCLabel)> listCustomCells = new List<(VCCellSimple, VCLabel)>();
        private readonly List<VCCellSimple> listCellBaseResources = new List<VCCellSimple>();
        private readonly VCLabel lblDamageMelee;
        private readonly VCLabel lblDamageArcher;
        private readonly VCLabel lblDamageMagic;
        private readonly VCLabel lblDefenseMelee;
        private readonly VCLabel lblDefenseArcher;
        private readonly VCLabel lblDefenseMagic;
        private int nextTop;
        private readonly Color colorBackground;
        private Bitmap bmpBackground;
        private int widthControl;

        private readonly Timer timerHover;// Таймер для показывания подсказки

        private const int PANEL_WIDTH = 296;

        public PanelHint() : base()
        {
            ShowBorder = true;
            Width = PANEL_WIDTH;
            Visible = false;

            colorBackground = Color.FromArgb(192, 0, 0, 0);

            widthControl = Width - FormMain.Config.GridSize - FormMain.Config.GridSize;

            lblHeader = new VCText(this, FormMain.Config.GridSize, 4, Program.formMain.fontMedCaptionC, Color.Yellow, widthControl);
            lblHeader.StringFormat.Alignment = StringAlignment.Near;
            lblHeader.StringFormat.LineAlignment = StringAlignment.Near;

            imgCell = new VCCellSimple(this, FormMain.Config.GridSize, lblHeader.NextTop());

            lblType = new VCText(this, FormMain.Config.GridSize, lblHeader.NextTop(), Program.formMain.fontParagraphC, Color.DarkKhaki, widthControl);
            lblType.StringFormat.Alignment = StringAlignment.Near;
            lblType.StringFormat.LineAlignment = StringAlignment.Near;

            lblAction = new VCText(this, FormMain.Config.GridSize, lblType.NextTop(), Program.formMain.fontMedCaptionC, FormMain.Config.HintAction, widthControl);
            lblAction.StringFormat.Alignment = StringAlignment.Near;
            lblAction.StringFormat.LineAlignment = StringAlignment.Near;

            lblState = new VCText(this, FormMain.Config.GridSize, lblAction.NextTop(), Program.formMain.fontMedCaptionC, FormMain.Config.HintAction, widthControl);
            lblState.StringFormat.Alignment = StringAlignment.Near;
            lblState.StringFormat.LineAlignment = StringAlignment.Near;

            lblDescription = new VCText(this, FormMain.Config.GridSize, lblState.NextTop(), Program.formMain.fontSmallC, FormMain.Config.HintDescription, widthControl);
            lblDescription.StringFormat.Alignment = StringAlignment.Near;
            lblDescription.StringFormat.LineAlignment = StringAlignment.Near;

            lblDurability = new VCLabelValue(this, FormMain.Config.GridSize, lblDescription.NextTop(), FormMain.Config.HintIncome, false);
            lblDurability.Image.ImageIndex = FormMain.GUI_16_DURABILITY;
            lblDurability.Width = widthControl;

            lblIncome = new VCLabelValue(this, FormMain.Config.GridSize, lblDurability.NextTop(), FormMain.Config.HintIncome, false);
            lblIncome.Image.ImageIndex = FormMain.GUI_16_GOLD;
            lblIncome.Width = widthControl;

            lblSalary = new VCLabelValue(this, FormMain.Config.GridSize, lblIncome.NextTop(), FormMain.Config.HintIncome, false);
            lblSalary.Image.ImageIndex = FormMain.GUI_16_PURSE;
            lblSalary.Width = widthControl;

            lblGreatnessAdd = new VCLabelValue(this, FormMain.Config.GridSize, lblSalary.NextTop(), FormMain.Config.HintIncome, false);
            lblGreatnessAdd.Image.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblGreatnessAdd.Width = widthControl;

            lblBuildersPerDay = new VCLabelValue(this, FormMain.Config.GridSize, lblGreatnessAdd.NextTop(), FormMain.Config.HintIncome, false);
            lblBuildersPerDay.Image.ImageIndex = FormMain.GUI_16_BUILDER;
            lblBuildersPerDay.Width = widthControl;

            //
            lblSettlementParameters = new VCLabel(this, FormMain.Config.GridSize, lblBuildersPerDay.NextTop(), Program.formMain.fontSmallC, Color.White, 16, "Параметры города/деревни:");
            lblSettlementParameters.Width = widthControl;
            lblSettlementParameters.StringFormat.Alignment = StringAlignment.Near;

            //
            listProperties = new List<VCLabelValue>();
            foreach (DescriptorProperty dp in FormMain.Descriptors.PropertiesCreature)
            {
                listProperties.Add(CreateLabelValue(3));
            }

            lblInterest = new VCLabelValue(this, FormMain.Config.GridSize, lblBuildersPerDay.NextTop(), Color.White, false);
            lblInterest.Image.ImageIndex = FormMain.GUI_16_INTEREST_OTHER;
            lblInterest.Width = widthControl;

            //
            sepExecuting = new VCSeparator(this, FormMain.Config.GridSize, lblInterest.NextTop());
            sepExecuting.Width = widthControl;

            lblNameExecuting = new VCText(this, FormMain.Config.GridSize, sepExecuting.NextTop(), Program.formMain.fontMedCaptionC, Color.PaleTurquoise, widthControl);
            lblNameExecuting.StringFormat.Alignment = StringAlignment.Near;

            lblChapterCost = new VCLabel(this, FormMain.Config.GridSize, lblNameExecuting.NextTop(), Program.formMain.fontSmallC, Color.White, 16, "Стоимость:");
            lblChapterCost.Width = widthControl;
            lblChapterCost.StringFormat.Alignment = StringAlignment.Near;

            lblCostGold = CreateLabelValue(4);
            lblCostGold.Image.ImageIndex = FormMain.GUI_16_GOLD;

            lblBuilders = CreateLabelValue(4);
            lblBuilders.Image.ImageIndex = FormMain.GUI_16_BUILDER;

            lblTimeExecuting = CreateLabelValue(4);
            lblTimeExecuting.Image.ImageIndex = FormMain.GUI_16_DAY;

            lblTextForRequirement = new VCLabel(this, FormMain.Config.GridSize, lblBuildersPerDay.NextTop(), Program.formMain.fontSmallC, Color.White, 16, "Требуется:");
            lblTextForRequirement.Width = widthControl;
            lblTextForRequirement.StringFormat.Alignment = StringAlignment.Near;

            //
            lblSigner = new VCLabel(this, FormMain.Config.GridSize, lblCostGold.NextTop(), Program.formMain.fontSmallC, Color.SkyBlue, 16, "");
            lblSigner.StringFormat.Alignment = StringAlignment.Near;
            lblSigner.Width = widthControl;

            lblSeparateTooltip = new VCSeparator(this, FormMain.Config.GridSize, lblSigner.NextTop());
            lblSeparateTooltip.Width = widthControl;

            lblTooltip = new VCText(this, FormMain.Config.GridSize, lblSeparateTooltip.NextTop(), Program.formMain.fontSmallC, Color.White, widthControl);

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

            timerHover = new Timer()
            {
                Interval = FormMain.Config.MouseHoverTime == 0 ? SystemInformation.MouseHoverTime : FormMain.Config.MouseHoverTime,
                Enabled = false
            };
            timerHover.Tick += TimerHover_Tick;
        }

        internal Player Player { get; set; }// 
        internal bool ExistHint { get; set; }
        internal VisualControl ForControl { get; private set; }// Подсказка для этого контрола

        internal void Clear()
        {
            ExistHint = false;
            lblHeader.Text = "";
            lblHeader.ShiftX = FormMain.Config.GridSize;
            imgCell.Visible = false;
            lblType.Visible = false;
            lblAction.Visible = false;
            lblState.Visible = false;
            lblDescription.Visible = false;
            lblDurability.Visible = false;
            lblIncome.Visible = false;
            lblSalary.Visible = false;
            lblGreatnessAdd.Visible = false;
            lblBuildersPerDay.Visible = false;

            //
            lblSettlementParameters.Visible = false;

            foreach (VCLabel l in listSettlementParameters)
                l.Visible = false;

            //

            foreach (VCText l in listRequirements)
                l.Dispose();
            sepExecuting.Visible = false;
            lblNameExecuting.Visible = false;
            lblTextForRequirement.Visible = false;
            listRequirements.Clear();

            foreach (VCLabelValue l in listProperties)
                l.Visible = false;

            lblInterest.Visible = false;

            //
            lblChapterCost.Visible = false;
            lblCostGold.Visible = false;
            lblBuilders.Visible = false;
            lblTimeExecuting.Visible = false;
            foreach (VCLabel l in listCostResources)
                l.Visible = false;

            //
            foreach (VCLabelValue ln in listLabelNeeds)
                ln.Visible = false;

            lblSigner.Visible = false;
            lblSeparateTooltip.Visible = false;
            lblTooltip.Visible = false;

            foreach (VCCellSimple cell in listCell)
                cell.Visible = false;

            foreach ((VCCellSimple, VCLabel) cp in listPerks)
            {
                cp.Item1.Visible = false;
                cp.Item2.Visible = false;
            }

            foreach ((VCCellSimple, VCLabel) cp in listCustomCells)
            {
                cp.Item1.Visible = false;
                cp.Item2.Visible = false;
            }

            foreach (VCCellSimple cell in listCellBaseResources)
                cell.Visible = false;

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

        internal void AddStep2Header(string header)
        {
            Debug.Assert(lblHeader.Text.Length == 0);

            Width = PANEL_WIDTH;
            ExistHint = true;

            if (header.Length > 0)
            {
                lblHeader.Color = Color.Yellow;
                lblHeader.ShiftX = FormMain.Config.GridSize;
                lblHeader.ShiftY = nextTop;
                lblHeader.Width = widthControl - lblHeader.ShiftX;
                lblHeader.Text = header;
                lblHeader.Height = lblHeader.MinHeigth();

                nextTop = lblHeader.NextTop();
            }
        }

        internal void AddStep2Descriptor(DescriptorEntity entity)
        {
            ShowEntity(entity.Name, entity.GetTypeEntity(), entity.ImageIndex, false);
        }

        internal void AddStep2Entity(Entity entity)
        {
            ShowEntity(entity.GetName(), entity.GetTypeEntity(), entity.GetImageIndex(), entity.ProperName());   
        }

        internal void ShowEntity(string name, string typeEntity, int imageIndex, bool properName)
        {
            Debug.Assert(lblHeader.Text.Length == 0);

            Width = PANEL_WIDTH;
            ExistHint = true;

            lblHeader.Color = properName ? Color.SteelBlue : Color.Yellow;
            lblHeader.ShiftX = imgCell.NextLeft();
            lblHeader.ShiftY = nextTop;
            lblHeader.Width = widthControl - lblHeader.ShiftX;
            lblHeader.Text = name;
            lblHeader.Height = lblHeader.MinHeigth();

            nextTop = lblHeader.NextTop();

            imgCell.ShiftY = lblHeader.ShiftY;
            imgCell.ImageIndex = imageIndex;
            imgCell.Visible = true;

            nextTop = Math.Max(lblHeader.NextTop(), imgCell.NextTop());

            AddStep3Type(typeEntity);
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

        internal void AddStep4Level(string level)
        {
            if (level.Length > 0)
            {
                lblAction.ShiftY = nextTop;
                lblAction.Text = level;
                lblAction.Height = lblAction.MinHeigth();
                lblAction.Visible = true;

                nextTop = lblAction.NextTop();
            }
        }

        internal void AddStep45State((string, Color) data)
        {
            if (data.Item1.Length > 0)
            {
                lblState.ShiftY = nextTop;
                lblState.Text = data.Item1;
                lblState.Color = data.Item2;
                lblState.Height = lblState.MinHeigth();
                lblState.Visible = true;

                nextTop = lblState.NextTop();
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

        internal void AddStep55Durability(int durability)
        {
            if (durability > 0)
            {
                lblDurability.ShiftY = nextTop;
                lblDurability.Visible = true;
                lblDurability.Text = durability.ToString();
                nextTop = lblDurability.NextTop();
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

        internal void AddStep75Salary(int salary)
        {
            Debug.Assert(salary >= 0);

            if (salary > 0)
            {
                Debug.Assert(!lblSalary.Visible);

                lblSalary.ShiftY = nextTop;
                lblSalary.Text = $"{salary}";
                lblSalary.Visible = true;

                nextTop = lblSalary.NextTop();
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

        internal void AddStep9CityParameters(ListCityParameters list)
        {
            if (list != null)
            {
                VCLabelValue lbl = null;
                int nextLeft = FormMain.Config.GridSize;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != 0)
                    {
                        if (!lblSettlementParameters.Visible)
                        {
                            lblSettlementParameters.Visible = true;
                            lblSettlementParameters.ShiftY = nextTop;
                            nextTop = lblSettlementParameters.NextTop();
                        }

                        lbl = GetLabel(i);
                        lbl.Visible = true;
                        lbl.Color = FormMain.Descriptors.CityParameters[i].PositiveIsGood ? (list[i] > 0 ? Color.Green : Color.Red) : (list[i] < 0 ? Color.Green : Color.Red);
                        lbl.Text = FormatDecimal100(list[i], true);
                        lbl.Image.ImageIndex = FormMain.Descriptors.CityParameters[i].ImageIndex16;
                        lbl.ShiftX = nextLeft;
                        lbl.ShiftY = nextTop;

                        nextLeft = lbl.NextLeft();
                        if (nextLeft + lbl.Width > Width)
                        {
                            nextLeft = FormMain.Config.GridSize;
                            nextTop = lbl.NextTop();
                        }
                    }
                }

                if ((lbl != null) && (nextLeft > FormMain.Config.GridSize))
                    nextTop = lbl.NextTop();
            }

            VCLabelValue GetLabel(int index)
            {
                if (index < listSettlementParameters.Count)
                    return listSettlementParameters[index];
                else
                {
                    VCLabelValue l = new VCLabelValue(this, 0, 0, FormMain.Config.HintIncome, false);
                    l.Width = 64;
                    listSettlementParameters.Add(l);
                    return l;
                }
            }
        }

        internal void AddStep9Properties(int[] props)
        {
            VCLabelValue priorlv = null;

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i] != 0)
                {
                    AdjustCell(listProperties[i], priorlv, FormMain.Config.GridSize, nextTop);
                    listProperties[i].Image.ImageIndex = FormMain.Descriptors.PropertiesCreature[i].ImageIndex;
                    listProperties[i].Text = FormatDecimal100(props[i], true);
                    listProperties[i].Visible = true;

                    priorlv = listProperties[i];
                }
            }

            if (priorlv != null)
                nextTop = priorlv.NextTop();
        }

        internal void AddStep9Interest(int interest, bool showPlus)
        {
            if (interest != 0)
            {
                lblInterest.ShiftY = nextTop;
                lblInterest.Text = DecIntegerBy10(interest, showPlus);
                lblInterest.Visible = true;

                nextTop = lblInterest.NextTop();
            }
        }

        internal void AddStep9ListNeeds(int[] array)
        {
            if ((array != null) && (array.Length > 0))
            {
                VCLabelValue lv = null;
                VCLabelValue priorlv = null;

                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] != 0)
                    {
                        lv = GetLabel(i);
                        AdjustCell(lv, priorlv, FormMain.Config.GridSize, nextTop);
                        lv.Visible = true;
                        lv.Image.ImageIndex = FormMain.Descriptors.NeedsCreature[i].ImageIndex;
                        lv.Text = DecIntegerBy10(array[i]);
                        lv.ShiftY = nextTop;

                        priorlv = lv;
                    }
                }

                if (lv != null)
                    nextTop = lv.NextTop();
            }

            VCLabelValue GetLabel(int index)
            {
                if (index < listLabelNeeds.Count)
                    return listLabelNeeds[index];
                else
                {
                    VCLabelValue l = CreateLabelValue(4);
                    listLabelNeeds.Add(l);
                    return l;
                }
            }
        }

        internal void AddStep9ListNeeds(List<(DescriptorNeed, int)> list, bool showPlus)
        {
            if (list.Count > 0)
            {
                (DescriptorNeed, int) need;
                VCLabelValue lv = null;
                VCLabelValue priorlv = null;

                for (int i = 0; i < list.Count; i++)
                {
                    need = list[i];
                    Debug.Assert(need.Item2 != 0);

                    lv = GetLabel(i);
                    AdjustCell(lv, priorlv, FormMain.Config.GridSize, nextTop);
                    lv.Visible = true;
                    lv.Image.ImageIndex = need.Item1.ImageIndex;
                    lv.Text = DecIntegerBy10(need.Item2, showPlus);

                    priorlv = lv;
                }

                if (lv != null)
                    nextTop = lv.NextTop();
            }

            VCLabelValue GetLabel(int index)
            {
                if (index < listLabelNeeds.Count)
                    return listLabelNeeds[index];
                else                    
                {
                    VCLabelValue l = CreateLabelValue(4);
                    listLabelNeeds.Add(l);
                    return l;
                }
            }
        }

        internal void AddStep10CostGold(int cost)
        {
            if (cost > 0)
            {
                lblCostGold.ShiftY = nextTop;
                lblCostGold.Text = cost.ToString();
                lblCostGold.Visible = true;
                nextTop = lblCostGold.NextTop();
            }
        }

        internal void AddStep12CostExecuting(string nameExecuting, ListBaseResources costResources)
        {
            AddStep12CostExecuting(nameExecuting, costResources, 0, 0, null);
        }

        internal void AddStep12CostExecuting(string nameExecuting, ListBaseResources costResources, int time, int builders, List<TextRequirement> requirement)
        {
            if (costResources != null)
            {
                Assert(costResources.Gold > 0);

                sepExecuting.ShiftY = nextTop;
                sepExecuting.Visible = true;
                lblNameExecuting.Text = nameExecuting;
                lblNameExecuting.Height = lblNameExecuting.MinHeigth();
                lblNameExecuting.ShiftY = sepExecuting.NextTop();
                lblNameExecuting.Visible = true;
                lblChapterCost.Visible = true;
                lblChapterCost.ShiftY = lblNameExecuting.NextTop();
                nextTop = lblChapterCost.NextTop();

                lblCostGold.ShiftY = nextTop;
                lblCostGold.Text = costResources.Gold.ToString();
                lblCostGold.Color = ColorRequirements(Player.BaseResources.Gold >= costResources.Gold);
                lblCostGold.Visible = true;

                lblBuilders.ShiftY = nextTop;
                if (builders > 0)
                {
                    lblBuilders.Color = ColorRequirements(builders <= Player.MaxBuilders);
                    lblBuilders.Text = builders.ToString();
                    lblBuilders.Visible = true;
                    AdjustCell(lblBuilders, lblCostGold, FormMain.Config.GridSize, nextTop);
                }

                if (time > 0)
                {
                    lblTimeExecuting.ShiftY = nextTop;
                    lblTimeExecuting.Text = time.ToString();
                    lblTimeExecuting.Visible = true;
                    AdjustCell(lblTimeExecuting, lblBuilders, FormMain.Config.GridSize, nextTop);
                }

                nextTop = lblCostGold.NextTop();
                AddCostResources(Player.BaseResources, costResources);

                if (requirement != null)
                {
                    if (requirement.Count > 0)
                    {
                        if (!sepExecuting.Visible)
                        {
                            sepExecuting.Visible = true;
                            sepExecuting.ShiftY = nextTop;
                            nextTop = sepExecuting.NextTop();
                        }

                        lblTextForRequirement.Visible = true;
                        lblTextForRequirement.ShiftY = nextTop;
                        nextTop = lblTextForRequirement.NextTop();

                        VCText lr;
                        foreach (TextRequirement tr in requirement)
                        {
                            if (!(Program.formMain.Settings.HideFulfilledRequirements && tr.Performed))
                            {
                                lr = new VCText(this, FormMain.Config.GridSize, nextTop, Program.formMain.fontSmallC, ColorRequirements(tr.Performed), widthControl - FormMain.Config.GridSize * 3);
                                lr.StringFormat.Alignment = StringAlignment.Near;
                                lr.Text = tr.Text;
                                lr.Height = lr.MinHeigth();
                                //lr.MaximumSize = new Size(Width - FormMain.Config.GridSize * 2, 0);

                                listRequirements.Add(lr);
                                nextTop = lr.NextTop();
                            }
                        }

                        if (listRequirements.Count == 0)
                        {
                            lblTextForRequirement.Visible = false;
                            nextTop = lblTextForRequirement.ShiftY;
                        }
                    }
                }
            }
            else
            {
                Assert(time == 0);
                Assert(builders == 0);
                Assert(costResources is null);
            }
        }

        private void AddCostResources(ListBaseResources ownRes, ListBaseResources costResources)
        {
            if ((costResources != null) && costResources.ExistsResources())
            {
                VCLabelValue lbl = null;
                int nextLeft = FormMain.Config.GridSize;

                for (int i = Config.INDEX_FIRST_RESOURCE; i < ownRes.Count; i++)
                {
                    if (costResources[i] > 0)
                    {
                        lbl = GetLabel(i);
                        lbl.Visible = true;
                        lbl.Color = ColorRequirements(ownRes[i] >= costResources[i]);
                        lbl.Text = costResources[i].ToString();
                        lbl.Image.ImageIndex = FormMain.Descriptors.BaseResources[i].ImageIndex16;
                        lbl.ShiftX = nextLeft;
                        lbl.ShiftY = nextTop;

                        nextLeft = lbl.NextLeft();
                        if (nextLeft + lbl.Width > Width)
                        {
                            nextLeft = FormMain.Config.GridSize;
                            nextTop = lbl.NextTop();
                        }
                    }
                }

                if ((lbl != null) && (nextLeft > FormMain.Config.GridSize))
                    nextTop = lbl.NextTop();

                VCLabelValue GetLabel(int index)
                {
                    if (index < listCostResources.Count)
                        return listCostResources[index];
                    else
                    {
                        VCLabelValue l = new VCLabelValue(this, 0, 0, FormMain.Config.HintIncome, false);
                        l.Width = 64;
                        listCostResources.Add(l);
                        return l;
                    }
                }
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

        internal void AddStep17Signer(Creature signer)
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
                    source = c.Descriptor.Name + (c.Descriptor.MaxLevel > 1 ? $" ({c.Level} ур.)" : "");
                else if (owner is Creature crt)
                    source = crt.TypeCreature.Name;
                Debug.Assert(source != null);

                lblSigner.ShiftY = nextTop;
                lblSigner.Text = $"От: {source}";
                lblSigner.Visible = true;

                nextTop = lblSigner.NextTop();
            }
        }

        internal void AddStep19Perks(List<Perk> list, int indexProperty)
        {
            if (list.Count > 0)
            {
                VCCellSimple cell = null;
                int nextLeft = FormMain.Config.GridSize;

                for (int i = 0; i < list.Count; i++)
                {
                    cell = GetCell(i);
                    cell.Visible = true;
                    cell.ImageIndex = list[i].GetImageIndex();
                    cell.LowText = Utils.FormatDecimal100(list[i].ListProperty[indexProperty]);
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

        internal void AddStep21BaseResources(List<ConstructionBaseResource> list, bool canMining)
        {
            if (list.Count > 0)
            {
                VCCellSimple cell = null;
                VCCellSimple priorCell = null;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Quantity != 0)
                    {
                        cell = GetCell(i);
                        AdjustCell(cell, priorCell, FormMain.Config.GridSize, nextTop);
                        cell.Visible = true;
                        cell.ImageIsEnabled = canMining;
                        cell.LowText = list[i].Quantity.ToString();
                        cell.ImageIndex = list[i].Descriptor.ImageIndex;

                        priorCell = cell;
                    }
                }

                if (cell != null)
                    nextTop = cell.NextTop();

                VCCellSimple GetCell(int index)
                {
                    if (index < listCell.Count)
                        return listCellBaseResources[index];
                    else
                    {
                        VCCellSimple c = new VCCellSimple(this, FormMain.Config.GridSize, 0);
                        listCellBaseResources.Add(c);
                        return c;
                    }
                }
            }
        }

        internal void AddStep21ListCustomCells(HintListCustomCells list)
        {
            if (list.Count > 0)
            {
                (VCCellSimple, VCLabel) cell = (null, null);

                for (int i = 0; i < list.Count; i++)
                {
                    cell = GetCell(i);
                    cell.Item1.Visible = true;
                    cell.Item1.ImageIndex = list[i].ImageIndex;
                    cell.Item1.ShiftY = nextTop;

                    cell.Item2.Visible = true;
                    cell.Item2.Text = list[i].Text;
                    cell.Item2.Color = list[i].Color;                        
                    cell.Item2.ShiftY = cell.Item1.ShiftY;

                    nextTop = cell.Item1.NextTop();
                }

                (VCCellSimple, VCLabel) GetCell(int index)
                {
                    if (index < listCell.Count)
                        return listCustomCells[index];
                    else
                    {
                        (VCCellSimple, VCLabel) c = (new VCCellSimple(this, FormMain.Config.GridSize, 0), new VCLabel(this, 0, 0, Program.formMain.fontParagraphC, Color.White, 48, ""));
                        c.Item2.ShiftX = c.Item1.NextLeft();
                        c.Item2.Width = Width - c.Item2.ShiftX - FormMain.Config.GridSize;
                        c.Item2.StringFormat.Alignment = StringAlignment.Near;
                        c.Item2.StringFormat.LineAlignment = StringAlignment.Center;
                        listCustomCells.Add(c);
                        return c;
                    }
                }
            }
        }



        internal void AddStep21Tooltip(string text)
        {
            Debug.Assert(text.Length > 0);

            if (Program.formMain.Settings.ShowExtraHint)
            {
                lblSeparateTooltip.Visible = true;
                lblSeparateTooltip.ShiftY = nextTop;

                lblTooltip.ShiftY = lblSeparateTooltip.NextTop();
                lblTooltip.Text = text;
                lblTooltip.Height = lblTooltip.MinHeigth();
                lblTooltip.Visible = true;

                nextTop = lblTooltip.NextTop();
            }
        }

        internal void SetControl(VisualControl c)
        {
            Assert(c != null);
            Assert(c != this);

            ForControl = c;
            timerHover.Start();
        }

        internal void HideHint()
        {
            if (Visible)
            {
                Visible = false;
                timerHover.Stop();
                ForControl = null;
                Clear();
                Program.formMain.SetNeedRedrawFrame();
                //Program.formMain.ShowFrame(false);
            }
            else if (timerHover.Enabled)
            {
                timerHover.Stop();
                ForControl = null;
                Clear();
            }
        }

        private void TimerHover_Tick(object sender, EventArgs e)
        {
            timerHover.Stop();

            Assert(ForControl != null);
            Assert(ForControl.Visible);

            ForControl.DoShowHint();
            if (ExistHint)
            {
                DrawHint();
                Visible = true;

                Program.formMain.SetNeedRedrawFrame();
                Program.formMain.ShowFrame(false);

                Assert(ForControl.Visible);
            }
        }

        internal void DrawHint()
        {
            Debug.Assert(ForControl.Visible);
            Debug.Assert(ForControl.Width > 8);
            Debug.Assert(ForControl.Height > 8);
            Debug.Assert(lblHeader.Text.Length > 0);

            Height = nextTop;

            Point l = new Point(ForControl.Left, ForControl.Top + ForControl.Height + 4);
            // Если подсказка уходит за пределы экрана игры, меняем ее положение
            if (l.X + Width > Program.formMain.sizeGamespace.Width - FormMain.Config.GridSize)
                l.X = Program.formMain.sizeGamespace.Width - Width - FormMain.Config.GridSize;
            if (l.Y + Height > Program.formMain.sizeGamespace.Height - FormMain.Config.GridSize)
                l.Y = l.Y - Height - ForControl.Height - 7;

            // Если подсказка не помещается ни снизу, ни сверху, показываем ее справа или слева
            if (l.Y < 0)
            {
                l.Y = Program.formMain.sizeGamespace.Height - FormMain.Config.GridSize - Height;
                // Здесь нужна проверка на то, где рисовать - слева или справа
                l.X = ForControl.Left + ForControl.Width + 4;
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

        private Color ColorRequirements(bool met)
        {
            return met ? FormMain.Config.HintRequirementsMet : FormMain.Config.HintRequirementsNotMet;
        }

        private void AdjustCell(VisualControl cell, VisualControl priorCell, int left, int top)
        {
            if (priorCell is null)
            {
                cell.ShiftX = left;
                cell.ShiftY = top;
            }
            else
            {
                int nextLeft = priorCell.NextLeft();
                if (nextLeft + cell.Width + FormMain.Config.GridSize <= Width)
                {
                    cell.ShiftX = nextLeft;
                    cell.ShiftY = priorCell.ShiftY;
                }
                else
                {
                    cell.ShiftX = left;
                    cell.ShiftY = priorCell.NextTop();
                }
            }
        }

        private VCLabelValue CreateLabelValue(int columns)
        {
            Assert(columns >= 1);

            VCLabelValue l = new VCLabelValue(this, FormMain.Config.GridSize, 0, Color.White, false);
            l.Width = (widthControl - FormMain.Config.GridSize * (columns - 1)) / columns;
            return l;
        }
    }
}
