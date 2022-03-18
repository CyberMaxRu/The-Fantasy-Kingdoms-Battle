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
        private readonly VCText[,] arrayBonuses;
        private readonly Player player;

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
                int nextLeft = FormMain.Config.GridSize;

                for (int x = 0; x < arrayBonuses.GetLength(1); x++)
                {
                    VCText text = new VCText(ClientControl, nextLeft, nextTop, Program.formMain.fontParagraph, Color.MediumTurquoise, 200);
                    text.StringFormat.LineAlignment = StringAlignment.Center;
                    text.Padding = new System.Windows.Forms.Padding(FormMain.Config.GridSize);
                    text.Text = list[x].Name;
                    text.PlaySoundOnEnter = true;
                    text.PlaySoundOnClick = true;
                    text.ShowBorder = true;
                    text.Click += Text_Click;
                    text.Height = 96;
                    text.Tag = line;
                    arrayBonuses[line, x] = text;

                    nextLeft = text.NextLeft();

                }

                nextTop = arrayBonuses[line, 0].NextTop();
            }

            btnOk = new VCButton(ClientControl, 0, nextTop + (FormMain.Config.GridSize * 2), "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            btnOk.Enabled = false;
            AcceptButton = btnOk;

            //
            ClientControl.Width = arrayBonuses[0, arrayBonuses.GetLength(1) - 1].NextLeft();
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;

            lblEconomic.Width = ClientControl.Width;
            lblMilitary.Width = ClientControl.Width;
            lblOther.Width = ClientControl.Width;
        }

        private void Text_Click(object sender, EventArgs e)
        {
            int line = (sender as VCText).Tag;
            for (int i = 0; i < arrayBonuses.GetLength(1); i++)
            {
                arrayBonuses[line, i].ManualSelected = arrayBonuses[line, i] == sender;
            }

            //
            int selected = 0;
            for (int y = 0; y < arrayBonuses.GetLength(0); y++)
                for (int x = 0; x < arrayBonuses.GetLength(1); x++)
                    if (arrayBonuses[y, x].ManualSelected)
                    {
                        selected++;
                        break;
                    }

            btnOk.Enabled = selected == arrayBonuses.GetLength(0);

            //
            Program.formMain.NeedRedrawFrame();
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            if (btnOk != null)
            {
                btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
                btnOk.ShiftY = ClientControl.Height - btnOk.Height;
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
