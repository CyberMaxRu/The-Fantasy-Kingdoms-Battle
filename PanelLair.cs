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
    // Класс панели логова
    internal sealed class PanelLair : PanelMapObject
    {
        private readonly VCIconButton btnAction;
        private readonly VCIconButton btnCancel;
        private readonly VCIconButton btnInhabitants;
        private readonly VCIconButton btnHeroes;
        private readonly VCLabelValue lblRewardGold;
        private readonly VCLabelValue lblRewardGreatness;

        public PanelLair(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            btnAction = new VCIconButton(this, imgMapObject.NextLeft(), imgMapObject.NextTop(), Program.formMain.ilGui, FormMain.GUI_BATTLE);
            btnAction.Click += BtnAction_Click;
            btnAction.ShowHint += BtnAction_ShowHint;

            btnCancel = new VCIconButton(this, btnAction.ShiftX - btnAction.Width - FormMain.Config.GridSize, btnAction.ShiftY, Program.formMain.ilGui, FormMain.GUI_FLAG_CANCEL);
            btnCancel.Click += BtnCancel_Click;
            btnCancel.ShowHint += BtnCancel_ShowHint;

            btnInhabitants = new VCIconButton(this, imgMapObject.NextLeft(), imgMapObject.ShiftY, Program.formMain.ilGui, FormMain.GUI_HOME);
            btnInhabitants.Click += BtnInhabitants_Click;
            btnInhabitants.ShowHint += BtnInhabitants_ShowHint;

            btnHeroes = new VCIconButton(this, btnInhabitants.ShiftX, btnInhabitants.NextTop() + FormMain.Config.GridSize + FormMain.Config.GridSizeHalf, Program.formMain.ilGui, FormMain.GUI_TARGET);
            btnHeroes.Click += BtnHeroes_Click;
            btnHeroes.ShowHint += BtnHeroes_ShowHint;

            lblRewardGold = new VCLabelValue(this, FormMain.Config.GridSize, imgMapObject.NextTop(), Color.Green, true);
            lblRewardGold.Width = btnCancel.ShiftX - FormMain.Config.GridSize - lblRewardGold.ShiftX;
            lblRewardGold.ImageIndex = FormMain.GUI_16_GOLD;
            lblRewardGold.StringFormat.Alignment = StringAlignment.Near;

            lblRewardGreatness = new VCLabelValue(this, lblRewardGold.ShiftX, lblRewardGold.NextTop() - FormMain.Config.GridSizeHalf, Color.Green, true);
            lblRewardGreatness.Width = lblRewardGold.Width;
            lblRewardGreatness.ImageIndex = FormMain.GUI_16_GREATNESS;
            lblRewardGreatness.StringFormat.Alignment = StringAlignment.Near;
            lblRewardGreatness.Color = FormMain.Config.HintIncome;

            Height = btnAction.NextTop();
            Width = btnAction.NextLeft();
        }

        private void BtnInhabitants_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep1Header("Существа", "", Lair.ListMonstersForHint());
        }

        private void BtnHeroes_ShowHint(object sender, EventArgs e)
        {
            Program.formMain.formHint.AddStep1Header("Герои, выполняющие флаг", "", Lair.ListHeroesForHint());
        }

        internal PlayerConstruction Lair { get => PlayerObject as PlayerConstruction; }
        internal TypeConstruction TypeLair { get => Lair.TypeConstruction; }

        private void BtnHeroes_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(PlayerObject);
            Program.formMain.panelLairInfo.SelectPageHeroes();
        }

        private void BtnCancel_ShowHint(object sender, EventArgs e)
        {
            if (Lair.Hidden)
            {
                if (Lair.Cashback() == 0)
                {
                    Program.formMain.formHint.AddHeader("Отмена флага разведки");
                }
                else
                {
                    Program.formMain.formHint.AddStep1Header("Отмена флага разведки", "", "Возврат денег");
                    Program.formMain.formHint.AddStep2Income(Lair.Cashback());
                }
            }
            else
            {
                if (Lair.Cashback() == 0)
                {
                    Program.formMain.formHint.AddHeader("Отмена флага атаки");
                }
                else
                {
                    Program.formMain.formHint.AddStep1Header("Отмена флага атаки", "", "Возврат денег");
                    Program.formMain.formHint.AddStep2Income(Lair.Cashback());
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SelectThisConstruction();
            btnCancel.Visible = false;
            Lair.CancelFlag();
            Lair.Player.SetTaskForHeroes();
        }

        private void BtnAction_ShowHint(object sender, EventArgs e)
        {
            switch (Lair.TypeAction())
            {
                case TypeFlag.Scout:
                    if (Lair.PriorityFlag == PriorityExecution.None)
                        Program.formMain.formHint.AddStep1Header("Разведка", "", "Установить флаг разведки для отправки героев к месту");
                    else if (Lair.PriorityFlag < PriorityExecution.Exclusive)
                        Program.formMain.formHint.AddStep1Header("Разведка", Lair.PriorityFlatToText() + " приоритет", "Повысить приоритет разведки места");
                    else
                        Program.formMain.formHint.AddStep1Header("Разведка", Lair.PriorityFlatToText() + " приоритет", "Установлен максимальный приоритет флага");
                    break;
                case TypeFlag.Attack:
                    if (Lair.PriorityFlag == PriorityExecution.None)
                        Program.formMain.formHint.AddStep1Header("Атака", "", "Установить флаг атаки для отправки героев к месту");
                    else if (Lair.PriorityFlag < PriorityExecution.Exclusive)
                        Program.formMain.formHint.AddStep1Header("Атака", Lair.PriorityFlatToText() + " приоритет", "Повысить приоритет атаки логова");
                    else
                        Program.formMain.formHint.AddStep1Header("Атака", Lair.PriorityFlatToText() + " приоритет", "Установлен максимальный приоритет флага");
                    break;
                case TypeFlag.Defense:
                    if (Lair.PriorityFlag == PriorityExecution.None)
                        Program.formMain.formHint.AddStep1Header("Защита", "", "Установить флаг защиты для отправки героев к месту");
                    else if (Lair.PriorityFlag < PriorityExecution.Exclusive)
                        Program.formMain.formHint.AddStep1Header("Защита", Lair.PriorityFlatToText() + " приоритет", "Повысить приоритет защиты места");
                    else
                        Program.formMain.formHint.AddStep1Header("Защита", Lair.PriorityFlatToText() + " приоритет", "Установлен максимальный приоритет флага");
                    break;
                default:
                    throw new Exception($"Неизвестный тип действия: {Lair.TypeAction()}");
            }

            Program.formMain.formHint.AddStep3Requirement(Lair.GetRequirements());
            Program.formMain.formHint.AddStep4Gold(Lair.RequiredGold(), Lair.Player.Gold >= Lair.RequiredGold());
        }

        private void BtnInhabitants_Click(object sender, EventArgs e)
        {
            Program.formMain.SelectPlayerObject(PlayerObject);
            Program.formMain.panelLairInfo.SelectPageInhabitants();
        }

        internal void LinkToPlayer(PlayerConstruction pl)
        {
            Debug.Assert(pl != null);
            Debug.Assert(pl.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);
            Debug.Assert(pl.TypeConstruction == TypeLair);

            PlayerObject = pl;
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            if (btnAction.ImageIsEnabled)
            {
                SelectThisConstruction();

                if (Lair.PriorityFlag < PriorityExecution.Exclusive)
                {
                    Lair.IncPriority();
                    Lair.Player.SetTaskForHeroes();
                }
            }
        }

        internal override void Draw(Graphics g)
        {
            Debug.Assert(Lair.Player.Lobby.ID == Program.formMain.CurrentLobby.ID);

            btnAction.Visible = Lair.Hidden || (Lair.TypeConstruction.Category == CategoryConstruction.Lair);
            if (btnAction.Visible)
            {
                btnAction.ImageIsEnabled = Lair.CheckFlagRequirements();
                btnAction.Level = (int)Lair.PriorityFlag + 1;
                btnAction.Cost = Lair.PriorityFlag < PriorityExecution.High ? Lair.RequiredGold().ToString() : null;
            }

            Debug.Assert(btnAction.Visible || (!btnAction.Visible && (Lair.PriorityFlag == PriorityExecution.None)));
            btnCancel.Visible = Lair.PriorityFlag != PriorityExecution.None;

            if (btnAction.Visible)
            {
                switch (Lair.TypeAction())
                {
                    case TypeFlag.Scout:
                        btnAction.ImageIndex = FormMain.GUI_FLAG_SCOUT;
                        btnInhabitants.Visible = false;
                        break;
                    case TypeFlag.Attack:
                        btnAction.ImageIndex = FormMain.GUI_FLAG_ATTACK;
                        btnInhabitants.Visible = Lair.Monsters.Count > 0;
                        if (btnInhabitants.Visible)
                            btnInhabitants.Cost = Lair.Monsters.Count.ToString();
                        break;
                    case TypeFlag.Defense:
                        btnAction.ImageIndex = FormMain.GUI_FLAG_DEFENSE;
                        btnInhabitants.Visible = Lair.Monsters.Count > 0;
                        if (btnInhabitants.Visible)
                            btnInhabitants.Cost = Lair.Monsters.Count.ToString();
                        break;
                    default:
                        throw new Exception($"Неизвестный тип действия: {Lair.TypeAction()}");
                }
            }
            else
            {
                Debug.Assert(Lair.Monsters.Count == 0);
                btnInhabitants.Visible = false;
            }

            btnHeroes.Visible = Lair.listAttackedHero.Count > 0;
            if (btnHeroes.Visible)
                btnHeroes.Cost = $"{Lair.listAttackedHero.Count}/{Lair.MaxHeroesForFlag()}";

            imgMapObject.ImageIndex = Lair.ImageIndexLair();
            lblNameMapObject.Text = Lair.NameLair();
            lblNameMapObject.Color = GetColorCaption();

            lblRewardGold.Visible = !Lair.Hidden && (Lair.TypeConstruction.TypeReward != null) && (Lair.TypeConstruction.TypeReward.Gold > 0);
            if (lblRewardGold.Visible)
            {
                lblRewardGold.Text = Lair.TypeConstruction.TypeReward.Gold.ToString();
            }

            lblRewardGreatness.Visible = !Lair.Hidden && (Lair.TypeConstruction.TypeReward != null) && (Lair.TypeConstruction.TypeReward.Greatness > 0);
            if (lblRewardGreatness.Visible)
            {
                lblRewardGreatness.Text = Lair.TypeConstruction.TypeReward.Greatness.ToString();
            }

            base.Draw(g);
        }

        private Color GetColorCaption()
        {
            if (Lair.PriorityFlag == PriorityExecution.None)
                return Lair.Hidden ? FormMain.Config.ColorMapObjectCaption(false) : Color.MediumAquamarine;

            switch (Lair.TypeAction())
            {
                case TypeFlag.Scout:
                    return Color.LimeGreen;
                case TypeFlag.Attack:
                    return Color.OrangeRed;
                case TypeFlag.Defense:
                    return Color.DodgerBlue;
                default:                    
                    throw new Exception($"Неизвестный тип действия: {Lair.TypeAction()}");
            }
        }

        protected override void PlaySelect()
        {
            if (Lair.Hidden)
                Program.formMain.PlayPushButton();
            else
                TypeLair.PlaySoundSelect();
        }
    }
}