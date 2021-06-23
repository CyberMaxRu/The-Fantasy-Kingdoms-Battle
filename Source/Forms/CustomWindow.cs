using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class CustomWindow : VisualControl
    {
        private DispatcherFrame frame;
        private VisualLayer layer;
        private DialogResult dialogResult;
        private Point shiftControls;

        public CustomWindow()
        {
            layer = Program.formMain.AddLayer(this);
            shiftControls = Program.formMain.ShiftControls;
        }

        protected VCButton AcceptButton { get; set; }
        protected VCButton CancelButton { get; set; }

        internal override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);

            if ((e.KeyCode == Keys.Enter) && (AcceptButton != null))
                AcceptButton.DoClick();
            if ((e.KeyCode == Keys.Escape) && (CancelButton != null))
                CancelButton.DoClick();
        }

        internal virtual void AdjustSize()
        {
        }

        internal override void Draw(Graphics g)
        {
            if (!shiftControls.Equals(Program.formMain.ShiftControls))
            {
                shiftControls = Program.formMain.ShiftControls;
                ToCentre();
            }

            base.Draw(g);
        }

        internal void CloseForm(DialogResult dr)
        {
            dialogResult = dr;
            Program.formMain.RemoveLayer(layer);
            if (frame != null)
                frame.Continue = false;

            Dispose();
        }

        internal void ToCentre()
        {
            SetPos(Program.formMain.ShiftControls.X + (Program.formMain.sizeGamespace.Width - Width) / 2, Program.formMain.ShiftControls.Y + (Program.formMain.sizeGamespace.Height - Height - 13) / 2);
            ArrangeControls();
        }

        internal DialogResult ShowDialog()
        {
            Program.formMain.formHint.HideHint();

            AdjustSize();
            ToCentre();
            Program.formMain.LayerChanged();

            frame = new DispatcherFrame();
            Dispatcher.PushFrame(frame);

            return dialogResult;
        }

        internal void Show()
        {
            Program.formMain.formHint.HideHint();

            AdjustSize();
            ToCentre();
            Program.formMain.LayerChanged();

            if (FormMain.Config.AutoCreatedPlayer)
            {
                WindowPlayerPreferences wpf = new WindowPlayerPreferences();
                wpf.ShowDialog();
                wpf.Dispose();
            }

        }
    }
}
