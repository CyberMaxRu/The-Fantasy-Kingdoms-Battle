using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowSelectPersistentBonuses : VCForm
    {
        private readonly VCLabel lblEconomic;
        private readonly VCLabel lblMilitary;
        private readonly VCLabel lblOther;
        private readonly VCButton btnOk;
        private readonly VCButton btnRandom;
        private readonly VCText[,] arrayBonuses;
        private readonly Player player;
        private readonly VCCellSimple[] arraySimpleHeroes;
        private readonly VCCellSimple[] arrayTempleHeroes;
        private readonly VCLabel lblSelectSimpleHero;
        private readonly VCLabel lblSelectTempleHero;

        public WindowSelectPersistentBonuses(Player p) : base()
        {
            windowCaption.Caption = "Выбор постоянных бонусов";
            player = p;

            // Создаем ящики с выбором бонуса
            arrayBonuses = new VCText[(int)TypePersistentBonus.Other + 1, p.Lobby.TypeLobby.VariantPersistentBonus];

            int nextTop;

            lblEconomic = new VCLabel(ClientControl, 0, 0, Program.formMain.fontParagraph, Color.MediumTurquoise, 16, "Выберите экономический бонус:");
            lblEconomic.StringFormat.Alignment = StringAlignment.Center;
            nextTop = lblEconomic.NextTop();
            DrawLine(0, p.VariantPersistentBonus[0]);
            lblMilitary = new VCLabel(ClientControl, 0, nextTop + FormMain.Config.GridSize, Program.formMain.fontParagraph, Color.MediumTurquoise, 16, "Выберите военный бонус:");
            lblMilitary.StringFormat.Alignment = StringAlignment.Center;
            nextTop = lblMilitary.NextTop();
            DrawLine(1, p.VariantPersistentBonus[1]);
            lblOther = new VCLabel(ClientControl, 0, nextTop + FormMain.Config.GridSize, Program.formMain.fontParagraph, Color.MediumTurquoise, 16, "Выберите дополнительный бонус:");
            lblOther.StringFormat.Alignment = StringAlignment.Center;
            nextTop = lblOther.NextTop();
            DrawLine(2, p.VariantPersistentBonus[2]);

            void DrawLine(int line, List<DescriptorPersistentBonus> list)
            {
                int nextLeft2 = FormMain.Config.GridSize;

                for (int x = 0; x < arrayBonuses.GetLength(1); x++)
                {
                    VCText text = new VCText(ClientControl, nextLeft2, nextTop, Program.formMain.fontParagraph, Color.MediumTurquoise, 200);
                    text.StringFormat.LineAlignment = StringAlignment.Center;
                    text.Padding = new System.Windows.Forms.Padding(FormMain.Config.GridSize);
                    text.Color = Color.White;
                    text.Text = list[x].Name;
                    text.PlaySoundOnEnter = true;
                    text.PlaySoundOnClick = true;
                    text.ShowBorder = true;
                    text.Click += Text_Click;
                    text.Height = 96;
                    text.Tag = line;
                    arrayBonuses[line, x] = text;

                    nextLeft2 = text.NextLeft();

                }

                nextTop = arrayBonuses[line, 0].NextTop();
            }

            lblSelectSimpleHero = new VCLabel(ClientControl, 0, nextTop, Program.formMain.fontParagraph, Color.MediumTurquoise, 16, "Выберите бонус обычного героя:");
            lblSelectSimpleHero.SetWidthByText();
            lblSelectSimpleHero.StringFormat.Alignment = StringAlignment.Center;
            nextTop = lblSelectSimpleHero.NextTop();

            arraySimpleHeroes = new VCCellSimple[player.Lobby.TypeLobby.VariantsUpSimpleHero];
            int nextLeft = FormMain.Config.GridSize;
            for (int i = 0; i < arraySimpleHeroes.GetLength(0); i++)
            {
                arraySimpleHeroes[i] = new VCCellSimple(ClientControl, nextLeft, nextTop);
                arraySimpleHeroes[i].ImageIndex = player.VariantsBonusedTypeSimpleHero[i].ImageIndex;
                arraySimpleHeroes[i].Click += SimpleHero_Click;
                arraySimpleHeroes[i].ShowHint += SimpleHero_ShowHint;
                arraySimpleHeroes[i].Tag = i;
                nextLeft = arraySimpleHeroes[i].NextLeft();
            }

            lblSelectTempleHero = new VCLabel(ClientControl, 0, lblSelectSimpleHero.ShiftY, Program.formMain.fontParagraph, Color.MediumTurquoise, 16, "Выберите бонус храмовника:");
            lblSelectTempleHero.SetWidthByText();
            lblSelectTempleHero.StringFormat.Alignment = StringAlignment.Center;

            arrayTempleHeroes = new VCCellSimple[player.Lobby.TypeLobby.VariantsUpTempleHero];
            nextLeft = FormMain.Config.GridSize;
            for (int i = 0; i < arrayTempleHeroes.GetLength(0); i++)
            {
                arrayTempleHeroes[i] = new VCCellSimple(ClientControl, nextLeft, nextTop);
                arrayTempleHeroes[i].ImageIndex = player.VariantsBonusedTypeTempleHero[i].ImageIndex;
                arrayTempleHeroes[i].Click += TempleHero_Click;
                arrayTempleHeroes[i].ShowHint += TempleHero_ShowHint;
                arrayTempleHeroes[i].Tag = i;
                nextLeft = arrayTempleHeroes[i].NextLeft();
            }

            //
            nextTop = arraySimpleHeroes[0].NextTop();

            btnOk = new VCButton(ClientControl, 0, nextTop + (FormMain.Config.GridSize * 2), "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            btnOk.Enabled = false;
            AcceptButton = btnOk;

            btnRandom = new VCButton(ClientControl, 0, btnOk.ShiftY, "Случайный выбор");
            btnRandom.Width = 208;
            btnRandom.Click += BtnRandom_Click;
            CancelButton = btnRandom;

            //
            ClientControl.Width = arrayBonuses[0, arrayBonuses.GetLength(1) - 1].NextLeft();
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;

            lblEconomic.Width = ClientControl.Width;
            lblMilitary.Width = ClientControl.Width;
            lblOther.Width = ClientControl.Width;
        }

        private void SimpleHero_ShowHint(object sender, EventArgs e)
        {
            VisualControl l = sender as VisualControl;
            PanelHint.AddStep2Header(player.VariantsBonusedTypeSimpleHero[l.Tag].Name);
            PanelHint.AddStep5Description(player.VariantsBonusedTypeSimpleHero[l.Tag].AdditionalBonus.TextHint);
        }

        private void TempleHero_ShowHint(object sender, EventArgs e)
        {
            VisualControl l = sender as VisualControl;
            PanelHint.AddStep2Header(player.VariantsBonusedTypeTempleHero[l.Tag].Name);
            PanelHint.AddStep5Description(player.VariantsBonusedTypeTempleHero[l.Tag].AdditionalBonus.TextHint);
        }

        private void TempleHero_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < arrayTempleHeroes.GetLength(0); i++)
            {
                arrayTempleHeroes[i].ManualSelected = arrayTempleHeroes[i] == sender;

                if (arrayTempleHeroes[i].ManualSelected)
                    player.SelectedBonusTempleHero = player.VariantsBonusedTypeTempleHero[i];
            }

            UpdateAllowClose();
        }

        private void SimpleHero_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < arraySimpleHeroes.GetLength(0); i++)
            {
                arraySimpleHeroes[i].ManualSelected = arraySimpleHeroes[i] == sender;

                if (arraySimpleHeroes[i].ManualSelected)
                    player.SelectedBonusSimpleHero = player.VariantsBonusedTypeSimpleHero[i];
            }

            UpdateAllowClose();
        }

        private void BtnRandom_Click(object sender, EventArgs e)
        {
            player.SelectRandomPersistentBonus();
            CloseForm(DialogAction.None);
        }

        private void UpdateAllowClose()
        {
            //
            int selected = 0;
            for (int y = 0; y < arrayBonuses.GetLength(0); y++)
                for (int x = 0; x < arrayBonuses.GetLength(1); x++)
                    if (arrayBonuses[y, x].ManualSelected)
                    {
                        selected++;
                        break;
                    }

            btnOk.Enabled = (player.SelectedBonusSimpleHero != null) && (player.SelectedBonusTempleHero != null) && (selected == arrayBonuses.GetLength(0));
        }

        private void Text_Click(object sender, EventArgs e)
        {
            int line = (sender as VCText).Tag;
            for (int i = 0; i < arrayBonuses.GetLength(1); i++)
            {
                arrayBonuses[line, i].ManualSelected = arrayBonuses[line, i] == sender;
            }

            UpdateAllowClose();
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            if (btnOk != null)
            {
                btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
                btnOk.ShiftY = ClientControl.Height - btnOk.Height;

                btnRandom.ShiftY = btnOk.ShiftY;

                int halfParent = lblSelectSimpleHero.Parent.Width / 2;
                lblSelectSimpleHero.ShiftX = (halfParent - lblSelectSimpleHero.Width) / 2;
                lblSelectTempleHero.ShiftX = halfParent + ((halfParent - lblSelectTempleHero.Width) / 2);

                int nextLeft = (halfParent - (((arraySimpleHeroes[0].Width + FormMain.Config.GridSize) * arraySimpleHeroes.GetLength(0)) - FormMain.Config.GridSize)) / 2;
                for (int i = 0; i < arraySimpleHeroes.GetLength(0); i++)
                {
                    arraySimpleHeroes[i].ShiftX = nextLeft;
                    nextLeft = arraySimpleHeroes[i].NextLeft();
                }

                nextLeft = halfParent + (halfParent - (((arrayTempleHeroes[0].Width + FormMain.Config.GridSize) * arrayTempleHeroes.GetLength(0)) - FormMain.Config.GridSize)) / 2;
                for (int i = 0; i < arrayTempleHeroes.GetLength(0); i++)
                {
                    arrayTempleHeroes[i].ShiftX = nextLeft;
                    nextLeft = arrayTempleHeroes[i].NextLeft();
                }
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < arrayBonuses.GetLength(0); y++)
                for (int x = 0; x < arrayBonuses.GetLength(1); x++)
                    if (arrayBonuses[y, x].ManualSelected)
                    {
                        player.PersistentBonuses.Add(player.VariantPersistentBonus[y][x]);
                        break;
                    }

            CloseForm(DialogAction.None);
        }
    }
}
