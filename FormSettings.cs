using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Fantasy_Kingdoms_Battle
{
    public partial class FormSettings : Form
    {
        private Bitmap bmpBackground;
        private Settings settings;
        private string filenameAvatar;
        private string directoryAvatar;
        public FormSettings()
        {
            InitializeComponent();

            bmpBackground = GuiUtils.MakeBackground(Size);
            button1.BackgroundImage = GuiUtils.MakeBackground(button1.Size);
            button2.BackgroundImage = GuiUtils.MakeBackground(button1.Size);

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
                settings.FileNameAvatar = filenameAvatar;
                settings.DirectoryAvatar = directoryAvatar;
                settings.SaveSettings();

                Program.formMain.ValidateAvatars();
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
            filenameAvatar = settings.FileNameAvatar;
            directoryAvatar = settings.DirectoryAvatar;
            ShowAvatar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog();
            OPF.InitialDirectory = filenameAvatar.Length > 0 ? Path.GetDirectoryName(filenameAvatar) : directoryAvatar?.Length > 0 ? directoryAvatar : Environment.CurrentDirectory;
            OPF.FileName = filenameAvatar.Length > 0 ? Path.GetFileName(filenameAvatar) : "";
            OPF.CheckFileExists = true;
            OPF.Multiselect = false;
            OPF.Filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg";
            if (OPF.ShowDialog() == DialogResult.OK)
            {
                filenameAvatar = OPF.FileName;
                directoryAvatar = Path.GetDirectoryName(filenameAvatar);
                ShowAvatar();
            }
        }

        private void ShowAvatar()
        {
            picxBoxAvatar.Image = filenameAvatar.Length > 0 ? GuiUtils.PrepareAvatar(filenameAvatar) : null;
            if (picxBoxAvatar.Image == null)
                filenameAvatar = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            filenameAvatar = "";
            ShowAvatar();
        }
    }
}
