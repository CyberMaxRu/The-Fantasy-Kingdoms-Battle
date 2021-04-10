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
        private VisualLayer layer;
        private DialogResult dialogResult;

        public CustomWindow()
        {
            layer = Program.formMain.AddLayer(this);
        }

        internal virtual void AdjustSize()
        {
        }

        internal override void Draw(Graphics g)
        {

            base.Draw(g);
        }

        internal void CloseForm(DialogResult dr)
        {
            dialogResult = dr;
            Program.formMain.RemoveLayer(layer);
            frame.Continue = false;
        }

        internal void ToCentre()
        {
            SetPos(Program.formMain.ShiftControls.X + (Program.formMain.sizeGamespace.Width - Width) / 2, Program.formMain.ShiftControls.Y + (Program.formMain.sizeGamespace.Height - Height - 13) / 2);
            ArrangeControls();
        }

        internal DialogResult ShowModal()
        {
            Program.formMain.formHint.HideHint();

            AdjustSize();
            ToCentre();
            Program.formMain.LayerChanged();

            frame = new DispatcherFrame();
            Dispatcher.PushFrame(frame);

            return dialogResult;
        }
    }
}
