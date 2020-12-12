using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed partial class FormAbout : Form
    {
        Bitmap bmpBackground;

        public FormAbout()
        {
            InitializeComponent();

            bmpBackground = GuiUtils.MakeBackgroundWithBorder(Size, Color.DarkOrange);
            button1.BackgroundImage = GuiUtils.MakeBackground(button1.Size);
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
    }
}
