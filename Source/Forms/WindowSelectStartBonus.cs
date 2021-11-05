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
            int idx = 1;
            foreach (StartBonus sb in listStartBonuses)
            {
                VisualControl text = new VisualControl(ClientControl, nextLeft, FormMain.Config.GridSize);
                VCLabel lblCaption = new VCLabel(text, 0, 4, Program.formMain.fontParagraphC, Color.MediumTurquoise, 24, $"Вариант {idx}");
                lblCaption.StringFormat.LineAlignment = StringAlignment.Center;
                text.PlaySoundOnEnter = true;
                text.PlaySoundOnClick = true;
                text.ShowBorder = true;
                text.Click += Text_Click;

                nextTop = lblCaption.NextTop();

                if (sb.Gold > 0)
                    AddBonus(text, FormMain.Config.Gui48_Gold, "Золото", $"+{sb.Gold}");
                if (sb.Builders > 0)
                    AddBonus(text, FormMain.Config.Gui48_Build, "Строители", $"+{sb.Builders}");
                if (sb.PeasantHouse > 0)
                    AddBonus(text, FormMain.Config.FindConstruction(FormMain.Config.IDPeasantHouse).ImageIndex, "Дом крестьян", $"+{sb.PeasantHouse}");
                if (sb.HolyPlace > 0)
                    AddBonus(text, FormMain.Config.FindConstruction(FormMain.Config.IDHolyPlace).ImageIndex, FormMain.Config.FindConstruction(FormMain.Config.IDHolyPlace).Name, $"+{sb.HolyPlace}");
                if (sb.Scouting > 0)
                    AddBonus(text, FormMain.Config.Gui48_FlagScout, "Разведанных мест", $"+{sb.Scouting}");

                for (int i = 0; i < sb.BaseResources.Count; i++)
                {
                    if (sb.BaseResources[i] > 0)
                        AddBonus(text, FormMain.Config.BaseResources[i].ImageIndex, FormMain.Config.BaseResources[i].Name, $"+{sb.BaseResources[i]}");
                }

                text.Height = lblCaption.NextTop() + (text.Controls[text.Controls.Count - 1].Height + FormMain.Config.GridSize) * 4;
                lblCaption.Width = text.Width;
                nextLeft = text.NextLeft() + FormMain.Config.GridSize;
                idx++;

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

            void AddBonus(VisualControl parent, int imageIndex, string caption, string text)
            {
                Debug.Assert(text != null);

                VCCustomEvent evnt = new VCCustomEvent();
                parent.AddControl(evnt);
                evnt.ShiftX = FormMain.Config.GridSize;
                evnt.ShiftY = nextTop;
                evnt.Width = 232;
                evnt.SetEvent(imageIndex, caption, text, Color.DarkGoldenrod);
                evnt.ClickOnParent = true;

                nextTop = evnt.NextTop();
                parent.Width = evnt.NextLeft();
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
