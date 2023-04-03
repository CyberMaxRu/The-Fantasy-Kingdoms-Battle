using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal class VCCustomNotice : VisualControl
    {
        private static List<Bitmap> cacheBackground = new List<Bitmap>();

        protected readonly VCLabel lblCaption;
        protected readonly VCLabel lblText;
        private Bitmap bmpBackground;
        private int tickForHide;

        public VCCustomNotice() : base()
        {
            Width = Program.formMain.layerGame.panelNotices.Width;

            CellOwner = new VCCellSimple(this, 0, 0);
            CellOwner.ShiftX = Width - CellOwner.Width;
            CellEntity = new VCCellSimple(this, 0, 0);
            CellEntity.ShiftX = CellOwner.ShiftX - CellEntity.Width - FormMain.Config.GridSize;

            lblCaption = new VCLabel(this, 2, 0, Program.formMain.FontParagraphC, Color.White, 16, "");
            lblCaption.StringFormat.Alignment = StringAlignment.Near;
            lblCaption.IsActiveControl = false;

            lblText = new VCLabel(this, lblCaption.ShiftX, 24, Program.formMain.FontParagraphC, Color.White, 16, "");
            lblText.StringFormat.Alignment = StringAlignment.Near;
            lblText.TruncLongText = true;
            lblCaption.IsActiveControl = false;

            Height = CellEntity.Height;
        }

        internal VCCellSimple CellOwner { get; }
        internal VCCellSimple CellEntity { get; }
        internal bool AutoHide { get; private set; }// Автоскрытие извещения
        internal int CounterForBeginHide { get; set; }// Счетчик до начала скрытия
        internal int CounterForRemove { get; set; }// Счетчик до удаления

        internal override void DrawBackground(Graphics g)
        {
            if (CounterForBeginHide == 0)
            {
                double alphaPerTick = 255.0 / tickForHide;
                byte opacity = Convert.ToByte(CounterForRemove * alphaPerTick);
                Opacity = opacity;
            }

            base.DrawBackground(g);

            DrawImage(g, bmpBackground, Left, Top);
        }

        private Bitmap PrepareBackground(int width)
        {
            Debug.Assert(width >= 48);

            foreach (Bitmap b in cacheBackground)
            {
                if (b.Width == width)
                    return b;
            }

            Bitmap bmp = new Bitmap(width, Height);
            int beginAlpha = 140;
            int endAlpha = 50;
            float stepAlpha = (float)width / (beginAlpha - endAlpha);
            // Инициализируем цветом и градиентной прозрачностью
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(Math.Max(0, Convert.ToInt32(beginAlpha - (x / stepAlpha))), Color.SkyBlue));
                }

            cacheBackground.Add(bmp);

            return bmp;
        }

        internal void SetNotice(int imageIndexOwner, int imageIndexEntity, string caption, string text, Color colorText, bool autoHide)
        {
            CellOwner.ImageIndex = imageIndexOwner;
            CellEntity.ImageIndex = imageIndexEntity;
            lblCaption.Text = caption;
            lblText.Text = text;
            lblText.Color = colorText;

            if (CellOwner.ImageIndex == -1)
            {
                CellOwner.Visible = false;
                CellEntity.ShiftX = CellOwner.ShiftX;
            }

            lblCaption.Width = CellEntity.ShiftX - 6 - lblCaption.ShiftX;
            lblText.Width = lblCaption.Width;
            bmpBackground = PrepareBackground(CellEntity.ShiftX - 6);

            AutoHide = autoHide;
            CounterForBeginHide = FormMain.Config.NoticeSecondsBeforeHide * FormMain.Config.TicksInSecond;
            CounterForRemove = FormMain.Config.NoticeSecondsHide * FormMain.Config.TicksInSecond;
            tickForHide = CounterForRemove;
        }

        internal void CloseSelf()
        {
            Debug.Assert(Visible);

            Visible = false;
            Program.formMain.layerGame.CurrentLobby.CurrentPlayer.RemoveNoticeForPlayer(this);
            Dispose();
        }
    }
}
