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

            ShowBorder = true;

            btnTypePlayer = new VCIconButton48(this, 0, 0, 1);
            btnTypePlayer.Click += BtnTypePlayer_Click;
            btnAvatar = new VCIconButton48(this, btnTypePlayer.NextLeft(), 0, 2);
            lblName = new VCLabel(this, btnAvatar.NextLeft(), 0, Program.formMain.fontMedCaptionC, Color.White, btnTypePlayer.Height, "");
            lblName.StringFormat.LineAlignment = StringAlignment.Center;
            lblName.Width = 160;
            btnPersistentBonus = new VCIconButton48(this, lblName.NextLeft(), 0, 3);
            btnStartBonus = new VCIconButton48(this, btnPersistentBonus.NextLeft(), 0, 4);

            Height = btnTypePlayer.Height;
            Width = btnStartBonus.EndLeft();

            UpdateData();
        }

        private void BtnTypePlayer_Click(object sender, EventArgs e)
        {
            setting.TypePlayer = setting.TypePlayer == TypePlayer.Human ? TypePlayer.Computer : TypePlayer.Human;

            UpdateData();
        }

        private void UpdateData()
        {
            int imIndexTypePlayer = -1;

            switch (setting.TypePlayer)
            {
                case TypePlayer.Human:
                    imIndexTypePlayer = 1;
                    break;
                case TypePlayer.Computer:
                    imIndexTypePlayer = 2;
                    break;
                default:
                    DoException($"Неизвестный тип игрока: {setting.TypePlayer}");
                    break;
            }

            btnTypePlayer.ImageIndex = imIndexTypePlayer;
            btnAvatar.ImageIndex = setting.Player.ImageIndex;
        }
    }
}
