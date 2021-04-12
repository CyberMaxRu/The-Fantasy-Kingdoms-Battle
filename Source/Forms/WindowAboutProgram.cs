using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class WindowAboutProgram : VCForm
    {
        private VCTextM2 txtAboutProject;
        private VCTextM2 txtAboutDeveloper;
        private VCTextM2 txtAddInfo;
        private VCLink linkRoadmap;
        private VCLink linkDesignDoc;
        private VCLink linkGithub;
        private VCLink linkRebirdh;
        private VCLink linkDiscord;
        private VCButton btnCheckUpdates;
        private VCButton btnClose;

        public WindowAboutProgram() : base()
        {
            windowCaption.Caption = FormMain.NAME_PROJECT;

            ClientControl.Width = 560;

            txtAboutProject = new VCTextM2(ClientControl, 0, 0, Program.formMain.fontParagraph, Color.White, ClientControl.Width);
            txtAboutProject.ShowBorder = true;
            txtAboutProject.Text = "Автобаттлер на основе \"Majesty 2: The Fantasy Kingdom Sim.\"\n\rПроект разрабатывается для проверки концепции автобаттлера по вселенной Majesty и бесплатен для использования."
                + "\n\rВ игре использованы графические и звуковые ресурсы из Majesty 2 (разработчик Ino-Co при участии Paradox Interactive)."
                + $"\n\rСборка {FormMain.VERSION} от {FormMain.DATE_VERSION}.\n\r \n\r";
            txtAboutProject.Padding = new Padding(4);
            txtAboutProject.Height = txtAboutProject.MinHeigth() + FormMain.Config.GridSize;

            linkRoadmap = new VCLink(txtAboutProject, FormMain.Config.GridSize, 0, "Дорожная карта", "https://docs.google.com/document/d/1LCYOQM2Rxf-KXgc8VmsWx1K0W97vhTwsHMQiwZr4z8Q/edit?usp=sharing");
            linkRoadmap.ShiftY = txtAboutProject.Height - linkRoadmap.Height - 8;
            linkDesignDoc = new VCLink(txtAboutProject, linkRoadmap.NextLeft() + FormMain.Config.GridSize, linkRoadmap.ShiftY, "Дизайн-документ", "https://docs.google.com/document/d/12Jw_20kLgtPcKbpVl9Ry4NawdG9dybXgvNPReBHWH2Q/edit?usp=sharing");
            linkGithub = new VCLink(txtAboutProject, linkDesignDoc.NextLeft() + FormMain.Config.GridSize, linkRoadmap.ShiftY, "GitHub", "https://github.com/CyberMaxRu/The-Fantasy-Kingdoms-Battle");

            txtAboutDeveloper = new VCTextM2(ClientControl, 0, txtAboutProject.NextTop(), Program.formMain.fontParagraph, Color.White, ClientControl.Width);
            txtAboutDeveloper.ShowBorder = true;
            txtAboutDeveloper.Text = "Разработчик: Кузьмин М.А.\n\rИсходный код написан на C# под .NET Framework 4.8 с рендерингом через GDI+. Использованы только стандартные компоненты, кроме работы c zip."
                + "\n\rРазработка ведется как проекта с открытым исходным кодом.";
            txtAboutDeveloper.StringFormat.Alignment = StringAlignment.Near;
            txtAboutDeveloper.Padding = new Padding(4);
            txtAboutDeveloper.Height = txtAboutDeveloper.MinHeigth() + FormMain.Config.GridSize;

            txtAddInfo = new VCTextM2(ClientControl, 0, txtAboutDeveloper.NextTop(), Program.formMain.fontParagraph, Color.White, ClientControl.Width);
            txtAddInfo.Text = "Игра создается при поддержке проекта \"Возрождение\":"
                + "\n\r \n\r \n\rОтдельная благодарность: Владиславу Франёву, участникам Discord-сервера: Феркасс, Герцог Тьмы, Ice_Cube."
                + "\n\rПомочь проекту автобаттлера можно своими предложениями (в Discord), участием в тестировании и распространении игры.";
            txtAddInfo.StringFormat.Alignment = StringAlignment.Near;
            txtAddInfo.Height = txtAddInfo.MinHeigth();

            linkRebirdh = new VCLink(txtAddInfo, 0, 28, "Проект \"Возрождение\" во ВК", "https://vk.com/majesty_2_vozrozhdeniye");
            linkDiscord = new VCLink(txtAddInfo, linkRebirdh.NextLeft() + FormMain.Config.GridSize, linkRebirdh.ShiftY, "Приглашение в Discord", "https://discord.com/invite/3R4PDsR");

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
            CloseForm(DialogResult.OK);
        }
    }
}
