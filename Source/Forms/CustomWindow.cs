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
        private bool showButtonClose;
        private VCImage imgClose;

        public CustomWindow(bool deactivatePriorWindow) : base()
        {
            Program.formMain.AddLayer(this, deactivatePriorWindow);
        }

        internal bool ShowButtonClose// Показывать крестик в правом верхнем углу
        {   
            get => showButtonClose;
            set
            {
                showButtonClose = value;
                if (showButtonClose && (imgClose is null))
                {
                    imgClose = new VCImage(this, 0, 0, Program.formMain.BmpListGui32, FormMain.GUI_32_CLOSE)
                    {
                        HighlightUnderMouse = true
                    };
                    imgClose.Click += ImgClose_Click;
                }
                else if (!showButtonClose && (imgClose != null))
                { 
                    imgClose.Dispose();
                    imgClose = null;
                }
            }
        }

        private void ImgClose_Click(object sender, EventArgs e)
        {
            CloseForm(DialogAction.None);
        }

        internal override void ArrangeControls()
        {
            if (imgClose != null)
                imgClose.ShiftX = Width - imgClose.Width;

            base.ArrangeControls();
        }

        protected VCButton AcceptButton { get; set; }
        protected VCButton CancelButton { get; set; }

        internal override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);

            if ((e.KeyCode == Keys.Enter) && (AcceptButton != null))
                AcceptButton.DoClick();
            if (e.KeyCode == Keys.Escape)
            {
                if (CancelButton != null)
                    CancelButton.DoClick();
                if (showButtonClose)
                    CloseForm(DialogAction.None);
            }
        }

        internal override void Draw(Graphics g)
        {            
            base.Draw(g);
        }

        internal virtual void AdjustSize()
        {
        }

        protected virtual void BeforeClose(DialogAction da)
        {

        }

        protected virtual void AfterClose(DialogAction da)
        {

        }

        internal void CloseForm(DialogAction dr)
        {
            BeforeClose(dr);

            dialogResult = dr;
            Program.formMain.RemoveLayer(this);
            if (frame != null)
                frame.Continue = false;

            AfterClose(dr);

            Dispose();
        }

        internal void ToCentre()
        {
            SetPos((Program.formMain.sizeGamespace.Width - Width) / 2, (Program.formMain.sizeGamespace.Height - Height - 13) / 2);
            ArrangeControls();
        }

        internal void Show()
        {
            PanelHint.HideHint();

            AdjustSize();
            ToCentre();
        }
    }
}
