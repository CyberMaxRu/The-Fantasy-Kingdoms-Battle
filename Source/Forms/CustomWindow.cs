using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class CustomWindow : VisualControl
    {
        private DispatcherFrame frame;
        private DialogAction dialogResult;

        public CustomWindow()
        {
            Program.formMain.AddLayer(this, ToString());
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

        protected virtual void BeforeClose()
        {

        }

        internal void CloseForm(DialogAction dr)
        {
            BeforeClose();

            dialogResult = dr;
            Program.formMain.RemoveLayer(this);
            if (frame != null)
                frame.Continue = false;

            Dispose();
        }

        internal void ToCentre()
        {
            SetPos((Program.formMain.sizeGamespace.Width - Width) / 2, (Program.formMain.sizeGamespace.Height - Height - 13) / 2);
            ArrangeControls();
        }

        internal DialogAction ShowDialog()
        {
            Program.formMain.formHint.HideHint();
            Program.formMain.StopShowHint();

            AdjustSize();
            ToCentre();
            Program.formMain.LayerChanged();

            frame = new DispatcherFrame();
            Dispatcher.PushFrame(frame);

            return dialogResult;
        }

        internal void Show(bool toCentre = true)
        {
            Program.formMain.formHint.HideHint();
            Program.formMain.StopShowHint();

            AdjustSize();
            if (toCentre)
                ToCentre();
            Program.formMain.LayerChanged();

            if (FormMain.Config.AutoCreatedPlayer)
            {
                WindowPlayerPreferences wpf = new WindowPlayerPreferences();
                wpf.ShowDialog();
            }
        }
    }
}
