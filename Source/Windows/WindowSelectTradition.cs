﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace Fantasy_Kingdoms_Battle
{
    // Кнопка выбранной традиции
    internal sealed class VCSelectTradition : VisualControl
    {
        public VCSelectTradition(VisualControl parent, int shiftX, int shiftY) : base(parent, shiftX, shiftY)
        {
            ShowBorder = true;
            PlaySoundOnEnter = true;
            PlaySoundOnClick = true;

            Width = 204;
            Height = 176;

            CellTypeTradition = new VCCellSimple(this, FormMain.Config.GridSize, FormMain.Config.GridSize);
            LblPercentCost = new VCLabel(CellTypeTradition, 0, 0, Program.formMain.FontMedCaptionC, Color.PaleGreen, CellTypeTradition.Height, "");
            LblPercentCost.Width = CellTypeTradition.Width;
            LblPercentCost.StringFormat.Alignment = StringAlignment.Center;
            LblPercentCost.StringFormat.LineAlignment = StringAlignment.Center;
            LblNameTypeTradition = new VCText(this, CellTypeTradition.NextLeft(), CellTypeTradition.ShiftY, Program.formMain.FontParagraph, Color.DarkKhaki, Width - CellTypeTradition.NextLeft() - FormMain.Config.GridSize);
            LblNameTypeTradition.Height = CellTypeTradition.Height;
            LblNameTypeTradition.StringFormat.Alignment = StringAlignment.Near;
            LblNameTypeTradition.StringFormat.LineAlignment = StringAlignment.Center;

            Text = new VCText(this, FormMain.Config.GridSize, CellTypeTradition.NextTop(), Program.formMain.FontParagraph, Color.White, Width - (FormMain.Config.GridSize * 2));
            Text.Height = 140;
            Text.IsActiveControl = false;
            Level = new VCLabel(this, 0, Height - 32, Program.formMain.FontBigCaptionC, Color.DarkGoldenrod, 24, "1");
            Level.Width = Width;
            Level.StringFormat.Alignment = StringAlignment.Center;
            Level.StringFormat.LineAlignment = StringAlignment.Far;
            Level.IsActiveControl = false;
        }

        internal VCCellSimple CellTypeTradition { get; }
        internal VCLabel LblPercentCost { get; }
        internal VCText LblNameTypeTradition { get; }
        internal VCText Text { get; }
        internal VCLabel Level { get; }
        internal DescriptorTradition Tradition { get; set; }
        internal int LevelTradition { get; set; }
    }

    // Окно выбора принимаемой традиции
    internal sealed class WindowSelectTradition : WindowOkCancel
    {
        private List<VCSelectTradition> listBoxes;
        private VCCustomNotice notice;
        private Player player;

        public WindowSelectTradition(VCCustomNotice n, Player p) : base("Выбор традиции", false)
        {
            Assert(p.ListVariantsTraditions.Count > 1);

            notice = n;
            player = p;

            btnOk.Caption = "Выбрать";
            btnOk.Enabled = false;
            btnCancel.Caption = "Позже";

            listBoxes = new List<VCSelectTradition>();

            int x = 0;
            int y = 0;
            int shiftX = 0;
            int shiftY = 0;
            VCSelectTradition st = null;

            foreach (KeyValuePair<DescriptorTradition, int> t in p.ListVariantsTraditions)
            {
                st = new VCSelectTradition(ClientControl, shiftX, shiftY);
                listBoxes.Add(st);
                shiftX = st.NextLeft();
                st.Tradition = t.Key;
                st.LevelTradition = t.Value;
                st.CellTypeTradition.ImageIndex = t.Key.TypeTradition.ImageIndex;
                st.CellTypeTradition.Hint = t.Key.TypeTradition.Description;
                st.LblPercentCost.Text = CalcPercent(t.Key.TypeTradition);
                st.LblNameTypeTradition.Text = t.Key.TypeTradition.Name;
                st.LblNameTypeTradition.Hint = t.Key.TypeTradition.Description;
                st.Text.Text = t.Key.Name;
                st.Level.Text = t.Value.ToString() + " уровень";
                st.CellTypeTradition.Click += St_Click;
                st.LblNameTypeTradition.Click += St_Click;
                st.Click += St_Click;

                x++;
                if (x == 3)
                {
                    x = 0;
                    y++;
                    shiftX = 0;
                    shiftY = st.NextTop();
                }
            }

            btnOk.ShiftY = st.NextTop() + FormMain.Config.GridSize;
            btnCancel.ShiftY = btnOk.ShiftY;
            ClientControl.ApplyMaxSize();
            ApplyMaxSize();

            ArrangeControls();

            string CalcPercent(DescriptorTypeTradition tt)
            {
                int percent;
                if (tt == player.TypeTradition1)
                    percent = FormMain.Config.PercentCostFirstTypeTradition;
                else if (tt == player.TypeTradition2)
                    percent = FormMain.Config.PercentCostSecondTypeTradition;
                else if (tt == player.TypeTradition3)
                    percent = FormMain.Config.PercentCostThirdTypeTradition;
                else
                    percent = 100;

                return percent == 100 ? "" : percent.ToString() + "%";
            }
        }

        internal DescriptorTradition SelectedTradition { get; set; }
        internal int SelectedTraditionLevel { get; set; }

        private void St_Click(object sender, EventArgs e)
        {
            btnOk.Enabled = true;

            VisualControl vc = sender as VisualControl;
            if (!(vc is VCSelectTradition))
                vc = vc.Parent;

            for (int i = 0; i < listBoxes.Count; i++)
            {
                listBoxes[i].ManualSelected = listBoxes[i] == vc;
                if (listBoxes[i].ManualSelected)
                {
                    SelectedTradition = listBoxes[i].Tradition;
                    SelectedTraditionLevel = listBoxes[i].LevelTradition;
                }
            }
        }

        protected override void AfterClose(DialogAction da)
        {
            base.AfterClose(da);

            if (da == DialogAction.OK)
            {
                Assert(SelectedTradition != null);

                player.SelectTradition(SelectedTradition, SelectedTraditionLevel);
                player.NoticeForTradition = null;
                player.ListVariantsTraditions.Clear();
                notice.CloseSelf();
            }
        }
    }
}
