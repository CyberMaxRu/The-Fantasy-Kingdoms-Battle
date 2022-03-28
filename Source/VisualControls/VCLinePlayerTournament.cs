using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class VCLinePlayerTournament : VisualControl
    {
        private readonly VCIconButton48 btnTypePlayer;
        private readonly VCIconButton48 btnAvatar;
        private readonly VCLabel lblName;
        private readonly VCIconButton48 btnPersistentBonus;
        private readonly VCIconButton48 btnStartBonus;

        private LobbySettingsPlayer setting;

        public VCLinePlayerTournament(VisualControl parent, int shiftX, int shiftY, LobbySettingsPlayer lsp) : base(parent, shiftX, shiftY)
        {
            setting = lsp;

            btnTypePlayer = new VCIconButton48(this, 0, 0, 1);
            btnTypePlayer.Click += BtnTypePlayer_Click;
            btnAvatar = new VCIconButton48(this, btnTypePlayer.NextLeft(), 0, 2);
            lblName = new VCLabel(this, btnAvatar.NextLeft(), 0, Program.formMain.fontMedCaptionC, Color.White, btnTypePlayer.Height, "");
            lblName.StringFormat.LineAlignment = StringAlignment.Center;
            lblName.Width = 240;
            lblName.ShowBorder = true;
            btnPersistentBonus = new VCIconButton48(this, lblName.NextLeft(), 0, 3);
            btnPersistentBonus.Click += BtnPersistentBonus_Click;
            btnStartBonus = new VCIconButton48(this, btnPersistentBonus.NextLeft(), 0, 4);
            btnStartBonus.Click += BtnStartBonus_Click;

            Height = btnTypePlayer.Height;
            Width = btnStartBonus.EndLeft();

            UpdateData();
        }

        private void BtnStartBonus_Click(object sender, EventArgs e)
        {
            switch (setting.TypeSelectStartBonus)
            {
                case TypeSelectBonus.Manual:
                    setting.TypeSelectStartBonus = TypeSelectBonus.Random;
                    break;
                case TypeSelectBonus.Random:
                    setting.TypeSelectStartBonus = TypeSelectBonus.Manual;
                    break;
            }

            UpdateData();
        }

        private void BtnPersistentBonus_Click(object sender, EventArgs e)
        {
            switch (setting.TypeSelectPersistentBonus)
            {
                case TypeSelectBonus.Manual:
                    setting.TypeSelectPersistentBonus = TypeSelectBonus.Random;
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается игроком";
                    break;
                case TypeSelectBonus.Random:
                    setting.TypeSelectPersistentBonus = TypeSelectBonus.Manual;
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается случайно";
                    break;
            }

            UpdateData();
        }

        private void BtnTypePlayer_Click(object sender, EventArgs e)
        {
            //setting.TypePlayer = setting.TypePlayer == TypePlayer.Human ? TypePlayer.Computer : TypePlayer.Human;

            //UpdateData();
        }

        private void UpdateData()
        {
            //
            int imIndexTypePlayer = -1;
            switch (setting.TypePlayer)
            {
                case TypePlayer.Human:
                    imIndexTypePlayer = FormMain.Config.Gui48_HumanPlayer;
                    btnTypePlayer.Hint = "Игрок - человек";
                    break;
                case TypePlayer.Computer:
                    imIndexTypePlayer = FormMain.Config.Gui48_ComputerPlayer;
                    btnTypePlayer.Hint = "Игрок - компьютер";
                    break;
                default:
                    DoException($"Неизвестный тип игрока: {setting.TypePlayer}");
                    break;
            }

            //
            btnTypePlayer.ImageIndex = imIndexTypePlayer;
            btnAvatar.ImageIndex = setting.Player.ImageIndex;
            lblName.Text = setting.Player.Name;
            btnPersistentBonus.ImageIndex = GetImageIndexBonus(setting.TypeSelectPersistentBonus);
            btnPersistentBonus.ImageIsEnabled = setting.TypePlayer == TypePlayer.Human;
            btnStartBonus.ImageIndex = GetImageIndexBonus(setting.TypeSelectStartBonus);
            btnStartBonus.ImageIsEnabled = setting.TypePlayer == TypePlayer.Human;

            switch (setting.TypeSelectStartBonus)
            {
                case TypeSelectBonus.Manual:
                    btnStartBonus.Hint = "Стартовый бонус выбирается игроком";
                    break;
                case TypeSelectBonus.Random:
                    btnStartBonus.Hint = "Стартовый бонус выбирается случайно";
                    break;
            }

            switch (setting.TypeSelectPersistentBonus)
            {
                case TypeSelectBonus.Manual:
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается игроком";
                    break;
                case TypeSelectBonus.Random:
                    btnPersistentBonus.Hint = "Постоянный бонус выбирается случайно";
                    break;
            }

            int GetImageIndexBonus(TypeSelectBonus type)
            {
                int imageIndex = -1;
                switch (type)
                {
                    case TypeSelectBonus.Manual:
                        imageIndex = FormMain.Config.Gui48_ManualSelect;
                        break;
                    case TypeSelectBonus.Random:
                        imageIndex = FormMain.Config.Gui48_RandomSelect;
                        break;
                    default:
                        DoException($"Неизвестный выбор бонуса: {type}");
                        break;
                }

                return imageIndex;
            }
        }
    }
}
