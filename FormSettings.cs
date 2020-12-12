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
    public partial class FormSettings : Form
    {
        private Bitmap bmpBackground;
        private Settings settings;

        public FormSettings()
        {
            InitializeComponent();

            bmpBackground = GuiUtils.MakeBackground(Size);

            chkbShowSplashVideo.ForeColor = FormMain.Config.CommonCaptionPage;
            chkbFullScreenMode.ForeColor = FormMain.Config.CommonCaptionPage;
            chkbCheckUpdates.ForeColor = FormMain.Config.CommonCaptionPage;
            chkbShowPath.ForeColor = FormMain.Config.CommonCaptionPage;
            chkbShowGrid.ForeColor = FormMain.Config.CommonCaptionPage;

            groupBox1.ForeColor = FormMain.Config.CommonCaptionPage;
            groupBox2.ForeColor = FormMain.Config.CommonCaptionPage;
            groupBox3.ForeColor = FormMain.Config.CommonCaptionPage;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.DrawImage(bmpBackground, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            
            if (DialogResult == DialogResult.OK)
            {
                settings.ShowSplashVideo = chkbShowSplashVideo.Checked;
                settings.FullScreenMode = chkbFullScreenMode.Checked;
                settings.CheckUpdateOnStartup = chkbCheckUpdates.Checked;
                settings.BattlefieldShowPath = chkbShowPath.Checked;
                settings.BattlefieldShowGrid = chkbShowGrid.Checked;

                settings.NamePlayer = txtbNamePlayer.Text;

                settings.SaveSettings();
            }
        }

        internal void ApplySettings(Settings s)
        {
            settings = s;

            chkbShowSplashVideo.Checked = settings.ShowSplashVideo;
            chkbFullScreenMode.Checked = settings.FullScreenMode;
            chkbCheckUpdates.Checked = settings.CheckUpdateOnStartup;
            chkbShowPath.Checked = settings.BattlefieldShowPath;
            chkbShowGrid.Checked = settings.BattlefieldShowGrid;

            txtbNamePlayer.Text = settings.NamePlayer;
        }
    }
}
