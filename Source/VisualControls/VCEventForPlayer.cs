using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypeEventForPlayer { Build };

    internal sealed class VCEventForPlayer : VisualControl
    {
        private VCCellSimple cell;
        private static Bitmap bmpBackground;
        private VCLabel lblCaption;
        private VCLabel lblText;

        public VCEventForPlayer(BigEntity entity, TypeEventForPlayer typeEvent) : base()
        {
            Debug.Assert(entity != null);

            Entity = entity;
            TypeEvent = typeEvent;

            Visible = false;

            cell = new VCCellSimple(this, 0, 3);
            cell.ImageIndex = Entity.GetImageIndex();
            cell.Click += Cell_Click;
            cell.HighlightUnderMouse = true;

            string nameEvent;
            string nameText;
            Color colorNameEntity;
            switch (TypeEvent)
            {
                case TypeEventForPlayer.Build:
                    nameEvent = "Строительство завершено:";
                    nameText = (Entity as Construction).NameLair();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                default:
                    throw new Exception($"Неизвестный тип события: {TypeEvent}.");
            }

            lblCaption = new VCLabel(this, cell.NextLeft(), 4, Program.formMain.fontMedCaptionC, Color.Gray, 16, nameEvent);
            lblCaption.Width = lblCaption.Font.WidthText(lblCaption.Text);
            lblText = new VCLabel(this, lblCaption.ShiftX, 27, Program.formMain.fontMedCaptionC, colorNameEntity, 16, nameText);
            lblText.Width = lblText.Font.WidthText(lblText.Text);

            Width = 399;
            Height = 54;
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            if (Entity is Construction c)
                Program.formMain.SelectConstruction(c);
            else
                throw new Exception("Неизвестная сущность.");
        }

        internal TypeEventForPlayer TypeEvent { get; }

        internal override void DrawBackground(Graphics g)
        {
            if (bmpBackground is null)
                bmpBackground = Program.formMain.LoadBitmap("BackgroundEvent.png");

            base.DrawBackground(g);

            g.DrawImageUnscaled(bmpBackground, Left + 52, Top);
        }
    }
}
