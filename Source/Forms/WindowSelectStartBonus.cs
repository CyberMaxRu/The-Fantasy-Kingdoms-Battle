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
        private readonly List<VCText> listBoxes = new List<VCText>();
        private List<StartBonus> list;

        public WindowSelectStartBonus(List<StartBonus> listStartBonuses) : base()
        {
            Debug.Assert(listStartBonuses.Count > 0);

            windowCaption.Caption = "Выбор стартового бонуса";
            list = listStartBonuses;

            // Создаем ящики с выбором бонуса
            int nextLeft = FormMain.Config.GridSize;
            int maxHeight = 0;
            foreach (StartBonus sb in listStartBonuses)
            {
                VCText text = new VCText(ClientControl, nextLeft, FormMain.Config.GridSize, Program.formMain.fontParagraph, Color.White, 200);
                text.StringFormat.Alignment = StringAlignment.Near;
                text.Padding = new Padding(FormMain.Config.GridSize);
                text.ShowBorder = true;
                text.Click += Text_Click;
                if (sb.Gold > 0)
                    text.Text += $"+{sb.Gold} золота{Environment.NewLine}";
                if (sb.Greatness > 0)
                    text.Text += $"+{sb.Greatness} очков величия{Environment.NewLine}";
                if (sb.PointConstructionGuild > 0)
                    text.Text += $"+{sb.PointConstructionGuild} очко(-ов) строительства гильдий{Environment.NewLine}";
                if (sb.PointConstructionTemple > 0)
                    text.Text += $"+{sb.PointConstructionTemple} Святая земля{Environment.NewLine}";
                if (sb.ScoutPlace > 0)
                    text.Text += $"+{sb.ScoutPlace} разведанных места{Environment.NewLine}";

                text.Height = 200;// text.MinHeigth();
                maxHeight = Math.Max(maxHeight, text.Height);
                nextLeft = text.NextLeft() + FormMain.Config.GridSize;

                listBoxes.Add(text);
            }

            // Выравниваем боксы по высоте
            foreach (VCText t in listBoxes)
            {
                t.Height = maxHeight;
            }

            btnOk = new VCButton(ClientControl, 0, listBoxes[0].NextTop() + (FormMain.Config.GridSize * 2), "ОК");
            btnOk.Width = 160;
            btnOk.Click += BtnOk_Click;
            btnOk.Enabled = false;
            AcceptButton = btnOk;

            //
            ClientControl.Width = nextLeft - FormMain.Config.GridSize;
            ClientControl.Height = btnOk.ShiftY + btnOk.Height;
        }

        internal StartBonus SelectedBonus { get; private set; }

        private void Text_Click(object sender, EventArgs e)
        {
            btnOk.Enabled = true;

            for (int i = 0; i < listBoxes.Count; i++)
            {
                listBoxes[i].ManualSelected = listBoxes[i] == sender;
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
