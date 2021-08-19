using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Окно с советом от Советника
    internal sealed class WindowAdvice : VCForm
    {
        private readonly VCImage128 imgAdvisor;
        private readonly VCText textAdvice;

        public WindowAdvice() : base()
        {
            windowCaption.Caption = "Королевский советник";

            imgAdvisor = new VCImage128(ClientControl, 0, 0);
            imgAdvisor.ImageIndex = FormMain.Config.FindTypeCreature(FormMain.Config.IDHeroAdvisor).ImageIndex;

            textAdvice = new VCText(ClientControl, imgAdvisor.NextLeft(), imgAdvisor.ShiftY, Program.formMain.fontParagraph, Color.White, 200);
            textAdvice.Height = 160;
            textAdvice.StringFormat.Alignment = StringAlignment.Near;

            ApplyMaxSize();
            Visible = false;
        }

        internal void ShowAdvice(string text)
        {
            textAdvice.Text = text;
            Visible = true;
        }
    }
}
