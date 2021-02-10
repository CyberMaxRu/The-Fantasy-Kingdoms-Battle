using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed partial class FormAbout : Form
    {
        Bitmap bmpBackground;

        public FormAbout()
        {
            InitializeComponent();

            bmpBackground = GuiUtils.MakeBackgroundWithBorder(Size, Color.DarkOrange);
            button1.BackgroundImage = GuiUtils.MakeBackground(button1.Size);
            button2.BackgroundImage = GuiUtils.MakeBackground(button2.Size);

            label5.Text = "Сборка " + FormMain.VERSION + " от " + FormMain.DATE_VERSION;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.DrawImage(bmpBackground, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/majesty_2_vozrozhdeniye");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.com/invite/3R4PDsR");        
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/document/d/1LCYOQM2Rxf-KXgc8VmsWx1K0W97vhTwsHMQiwZr4z8Q/edit?usp=sharing");        
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/document/d/12Jw_20kLgtPcKbpVl9Ry4NawdG9dybXgvNPReBHWH2Q/edit?usp=sharing");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = Program.formMain.CheckForNewVersion() ? "Версия " + Program.formMain.MainConfig.ActualVersion.ToString() : "Новой версии не найдено";
        }
    }
}
