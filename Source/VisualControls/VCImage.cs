using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{

    internal enum StateRestTime { Active, Pause, Stop };

    // Визуальный контрол - иконка
    internal class VCImage : VisualControl
    {
        private readonly VCLabel lblRestTtimeExecuting;

        public VCImage(VisualControl parent, int shiftX, int shiftY, BitmapList bitmapList, int imageIndex) : base(parent, shiftX, shiftY)
        {
            BitmapList = bitmapList;
            ImageIndex = imageIndex;

            Width = BitmapList.Size.Width;
            Height = BitmapList.Size.Height;

            lblRestTtimeExecuting = new VCLabel(this, 4, 1, Program.formMain.fontSmallC, Color.SkyBlue, 16, "");
            lblRestTtimeExecuting.StringFormat.LineAlignment = StringAlignment.Near;
            lblRestTtimeExecuting.StringFormat.Alignment = StringAlignment.Near;
            lblRestTtimeExecuting.Width = Width - 4;
            lblRestTtimeExecuting.Visible = false;
            lblRestTtimeExecuting.ManualDraw = true;
            StateRestTime = StateRestTime.Active;
        }

        internal BitmapList BitmapList { get; set; }
        internal int ImageIndex { get; set; }
        internal bool ImageIsEnabled { get; set; } = true;
        internal bool HighlightUnderMouse { get; set; } = false;
        internal string RestTimeExecuting { get; set; } = "";
        internal StateRestTime StateRestTime { get; set; }

        internal override void MouseEnter(bool leftButtonDown)
        {
            base.MouseEnter(leftButtonDown);

            Program.formMain.SetNeedRedrawFrame();
            if (PlaySelectSound())
                Program.formMain.PlaySelectButton();
        }

        internal override void MouseLeave()
        {
            base.MouseLeave();

            Program.formMain.SetNeedRedrawFrame();
        }

        internal override void Draw(Graphics g)
        {
            base.Draw(g);

            // Иконка
            if ((Visible || ManualDraw) && (ImageIndex != -1))
            {
                BitmapList.DrawImage(g, ImageIndex, /*UseFilter*/ ImageIsEnabled, HighlightUnderMouse && MouseOver && !MouseClicked, Left, Top);

                // Дней выполнения
                if (RestTimeExecuting.Length > 0)
                {
                    switch (StateRestTime)
                    {
                        case StateRestTime.Active:
                            lblRestTtimeExecuting.Color = Color.SkyBlue;
                            break;
                        case StateRestTime.Pause:
                            lblRestTtimeExecuting.Color = Color.Yellow;
                            break;
                        case StateRestTime.Stop:
                            lblRestTtimeExecuting.Color = Color.Red;
                            break;
                        default:
                            Utils.DoException($"Неизвестное состояние: {StateRestTime}");
                            break;
                    }

                    lblRestTtimeExecuting.Text = RestTimeExecuting;
                    lblRestTtimeExecuting.Draw(g);
                }

            }
        }

        protected virtual bool PlaySelectSound()
        {
            return true;// ImageIsEnabled && ((UseFilter && MouseOver) || HighlightUnderMouse);
        }
    }
}
