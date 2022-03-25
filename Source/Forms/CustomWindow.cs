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
    internal abstract class CustomWindow : LayerCustom
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
            PanelHint.HideHint();

            AdjustSize();
            ToCentre();
            Program.formMain.LayerChanged();

            frame = new DispatcherFrame();
            // Если использовать DispatcherFrame, то при выходе курсора за пределы клиентской области он не меняется на системный.
            // И чтобы закрыть окно, надо кликнуть 2 раза на крестике - сначала для активации окна, потом для действия
            // Переход на свой цикл устраняет эту проблему
            //Dispatcher.PushFrame(frame);

            while (frame.Continue)
            {
                Application.DoEvents();
            }
            

            return dialogResult;
        }
    }
}
