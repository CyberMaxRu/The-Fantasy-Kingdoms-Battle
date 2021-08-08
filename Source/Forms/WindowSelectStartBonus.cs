using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Окно выбора стартового бонуса
    internal sealed class WindowSelectStartBonus : VCForm
    {
        private readonly VCButton btnOk;
        private readonly List<VisualControl> listBoxes = new List<VisualControl>();
        private List<StartBonus> list;

        public WindowSelectStartBonus(List<StartBonus> listStartBonuses) : base()
        {
            Debug.Assert(listStartBonuses.Count > 0);

            windowCaption.Caption = "Выбор стартового бонуса";
            list = listStartBonuses;

            // Создаем ящики с выбором бонуса
            int nextLeft = FormMain.Config.GridSize;
            int nextTop;
            foreach (StartBonus sb in listStartBonuses)
            {
                nextTop = FormMain.Config.GridSize;

                VisualControl text = new VisualControl(ClientControl, nextLeft, FormMain.Config.GridSize);
                text.ShowBorder = true;
                text.Click += Text_Click;
                if (sb.Gold > 0)
                    AddBonus(text, sb.Gold.ToString(), $"+{sb.Gold} золота", FormMain.GUI_16_GOLD);
                if (sb.Greatness > 0)
                    AddBonus(text, sb.Greatness.ToString(), $"+{sb.Greatness} очков величия", FormMain.GUI_16_GREATNESS);
                if (sb.Builders > 0)
                    AddBonus(text, sb.Builders.ToString(), $"+{sb.Builders} строителей (на 1 ход)", FormMain.GUI_16_BUILDER);
                if (sb.PeasantHouse > 0)
                    AddBonus(text, sb.PeasantHouse.ToString(), $"+{sb.PeasantHouse} крестьянских домов", FormMain.GUI_16_PEASANT_HOUSE);
                if (sb.HolyPlace > 0)
                    AddBonus(text, sb.HolyPlace.ToString(), $"+{sb.HolyPlace} Святых земель", FormMain.GUI_16_HOLYLAND);
                if (sb.TradePlace > 0)
                    AddBonus(text, sb.TradePlace.ToString(), $"+{sb.TradePlace} торговых мест", FormMain.GUI_16_TRADEPOST);
                if (sb.Scouting > 0)
                    AddBonus(text, sb.Scouting.ToString(), $"+{sb.Scouting} разведанных мест", FormMain.GUI_16_SCOUT);

                text.Width = 160;
                text.Height = 200;
                nextLeft = text.NextLeft() + FormMain.Config.GridSize;

                listBoxes.Add(text);
            }

            btnOk = new VCButton(ClientControl, 0, listBoxes[0].NextTop() + (FormMain.Config.GridSize * 2), "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            btnOk.Enabled = false;
            AcceptButton = btnOk;

            //
            ClientControl.Width = nextLeft - FormMain.Config.GridSize;
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;

            void AddBonus(VisualControl parent, string text, string hint, int imageIndex)
            {
                Debug.Assert(text != null);

                VCLabel label = new VCLabel(parent, FormMain.Config.GridSize, nextTop, Program.formMain.fontParagraph, Color.White, Program.formMain.fontParagraph.MaxHeightSymbol, $"+{text}");
                label.StringFormat.Alignment = StringAlignment.Near;
                label.BitmapList = Program.formMain.ilGui16;
                label.ImageIndex = imageIndex;
                label.Width = 120;
                label.ClickOnParent = true;
                label.Hint = hint;
                nextTop = label.ShiftY + label.Height;
            }
        }

        internal StartBonus SelectedBonus { get; private set; }

        private void Text_Click(object sender, EventArgs e)
        {
            btnOk.Enabled = true;

            for (int i = 0; i < listBoxes.Count; i++)
            {
                listBoxes[i].ManualSelected = listBoxes[i] == sender;
                if (listBoxes[i].ManualSelected)
                    SelectedBonus = list[i];
            }

            Program.formMain.NeedRedrawFrame();
        }

        internal override void AdjustSize()
        {
            base.AdjustSize();

            btnOk.ShiftX = (ClientControl.Width - btnOk.Width) / 2;
            btnOk.ShiftY = ClientControl.Height - btnOk.Height;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }
    }
}
