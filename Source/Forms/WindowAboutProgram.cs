using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowAboutProgram : VCForm
    {
        private VCText txtAboutProject;
        private VCText txtAboutDeveloper;
        private VCText txtAddInfo;
        private VCButton btnRoadmap;
        private VCButton btnDesignDoc;
        private VCButton btnGithub;
        private VCButton btnRebirth;
        private VCButton btnDiscord;
        private VCButton btnCheckUpdates;
        private VCButton btnClose;

        public WindowAboutProgram() : base()
        {
            windowCaption.Caption = FormMain.NAME_PROJECT;

            ClientControl.Width = 560;

            txtAboutProject = new VCText(ClientControl, 0, 0, Program.formMain.fontParagraph, Color.White, ClientControl.Width);
            txtAboutProject.ShowBorder = true;
            txtAboutProject.Text = "Автобаттлер на основе \"Majesty 2: The Fantasy Kingdom Sim\".\n\rПроект разрабатывается для проверки концепции автобаттлера во вселенной Majesty и бесплатен для использования."
                + "\n\rВ игре использованы графические и звуковые ресурсы из Majesty 2 (разработчик Ino-Co при участии Paradox Interactive)."
                + $"\n\rСборка {FormMain.VERSION} от {FormMain.DATE_VERSION}.\n\r \n\r";
            txtAboutProject.Padding = new Padding(4);
            txtAboutProject.Height = txtAboutProject.MinHeigth() + (FormMain.Config.GridSize * 2);

            btnRoadmap = new VCButton(txtAboutProject, FormMain.Config.GridSize, txtAboutProject.Height - (FormMain.Config.GridSize * 5), "Дорожная карта");
            btnRoadmap.Width = 200;
            btnRoadmap.Click += BtnRoadmap_Click;
            //btnRoadmap.ShiftY = txtAboutProject.Height - btnRoadmap.Height - 8;
            btnDesignDoc = new VCButton(txtAboutProject, btnRoadmap.NextLeft(), btnRoadmap.ShiftY, "Дизайн-документ");
            btnDesignDoc.Width = 200;
            btnDesignDoc.Click += BtnDesignDoc_Click;

            txtAboutDeveloper = new VCText(ClientControl, 0, txtAboutProject.NextTop(), Program.formMain.fontParagraph, Color.White, ClientControl.Width);
            txtAboutDeveloper.ShowBorder = true;
            txtAboutDeveloper.Text = "Разработчик: Кузьмин М.А.\n\rИсходный код написан на C# под .NET Framework 4.8 с рендерингом через GDI+. Использованы только стандартные компоненты, кроме работы c zip."
                + "\n\rРазработка ведется как проекта с открытым исходным кодом.\n\r \n\r";
            txtAboutDeveloper.StringFormat.Alignment = StringAlignment.Near;
            txtAboutDeveloper.Padding = new Padding(4);
            txtAboutDeveloper.Height = txtAboutDeveloper.MinHeigth() + (FormMain.Config.GridSize * 2);
            btnGithub = new VCButton(txtAboutDeveloper, FormMain.Config.GridSize, txtAboutDeveloper.Height - (FormMain.Config.GridSize * 5), "GitHub");
            btnGithub.Width = 200;
            btnGithub.Click += BtnGithub_Click;

            txtAddInfo = new VCText(ClientControl, 0, txtAboutDeveloper.NextTop(), Program.formMain.fontParagraph, Color.White, ClientControl.Width);
            txtAddInfo.Text = "Игра создается при поддержке проекта \"Возрождение\":"
                + "\n\r \n\r \n\rОтдельная благодарность: Владиславу Франёву, участникам Discord-сервера: Феркасс, Герцог Тьмы, Ice_Cube."
                + "\n\rПомочь проекту автобаттлера можно своими предложениями (в Discord), участием в тестировании и распространении информации об игре.";
            txtAddInfo.StringFormat.Alignment = StringAlignment.Near;
            txtAddInfo.Height = txtAddInfo.MinHeigth();

            btnRebirth = new VCButton(txtAddInfo, 0, 28, "Проект \"Возрождение\" в ВК");
            btnRebirth.Width = 276;
            btnRebirth.Click += BtnRebirth_Click;
            btnDiscord = new VCButton(txtAddInfo, btnRebirth.NextLeft(), btnRebirth.ShiftY, "Приглашение в Discord");
            btnDiscord.Width = 276;
            btnDiscord.Click += BtnDiscord_Click;

            btnCheckUpdates = new VCButton(ClientControl, 0, txtAddInfo.NextTop(), "Проверить обновление");
            btnCheckUpdates.Width = 256;
            btnCheckUpdates.Click += BtnCheckUpdates_Click;
            
            btnClose = new VCButton(ClientControl, 0, txtAddInfo.NextTop(), "Закрыть");
            btnClose.ShiftX = ClientControl.Width - btnClose.Width;
            btnClose.Click += BtnClose_Click;

            AcceptButton = btnClose;
            CancelButton = btnClose;

            ClientControl.Height = btnClose.NextTop();
        }

        private void OpenLink(string link)
        {
            Debug.Assert(link.Length > 0);
            Process.Start(link);

        }

        private void BtnDiscord_Click(object sender, EventArgs e)
        {
            OpenLink("https://discord.com/invite/3R4PDsR");
        }

        private void BtnRebirth_Click(object sender, EventArgs e)
        {
            OpenLink("https://vk.com/majesty_2_vozrozhdeniye");
        }

        private void BtnGithub_Click(object sender, EventArgs e)
        {
            OpenLink("https://github.com/CyberMaxRu/The-Fantasy-Kingdoms-Battle");
        }

        private void BtnDesignDoc_Click(object sender, EventArgs e)
        {
            OpenLink("https://docs.google.com/document/d/12Jw_20kLgtPcKbpVl9Ry4NawdG9dybXgvNPReBHWH2Q/edit?usp=sharing");
        }

        private void BtnRoadmap_Click(object sender, EventArgs e)
        {
            OpenLink("https://docs.google.com/document/d/1LCYOQM2Rxf-KXgc8VmsWx1K0W97vhTwsHMQiwZr4z8Q/edit?usp=sharing");
        }

        private void BtnCheckUpdates_Click(object sender, EventArgs e)
        {
            if (btnCheckUpdates.Enabled)
            {
                btnCheckUpdates.Caption = Program.formMain.CheckForNewVersion() ? "Версия " + Program.formMain.MainConfig.ActualVersion.ToString() : "Новой версии не найдено";
                btnCheckUpdates.Enabled = false;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }
    }
}
