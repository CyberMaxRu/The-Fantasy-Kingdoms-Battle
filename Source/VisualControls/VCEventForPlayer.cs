using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypeEventForPlayer { Build, Research, Extension, MassEventBegin, MassEventEnd, TournamentBegin, TournamentEnd };

    internal sealed class VCEventForPlayer : VCCustomEvent
    {
        public VCEventForPlayer(Entity entity, TypeEventForPlayer typeEvent) : base(entity)
        {
            TypeEvent = typeEvent;

            Visible = false;
            cell.Click += Cell_Click;
            cell.RightClick += Cell_RightClick;
            cell.HighlightUnderMouse = true;

            string nameEvent;
            string nameText = "";
            Color colorNameEntity;
            switch (TypeEvent)
            {
                case TypeEventForPlayer.Build:
                    nameEvent = "Строительство завершено:";
                    nameText = (Entity as Construction).NameLair();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeEventForPlayer.Research:
                    nameEvent = "Исследование завершено:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeEventForPlayer.Extension:
                    nameEvent = "Дополнительное сооружение построено:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeEventForPlayer.MassEventBegin:
                    nameEvent = "Мероприятие начато:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeEventForPlayer.MassEventEnd:
                    nameEvent = "Мероприятие завершено:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeEventForPlayer.TournamentBegin:
                    nameEvent = "Турнир начат:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeEventForPlayer.TournamentEnd:
                    nameEvent = "Турнир завершен:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                default:
                    throw new Exception($"Неизвестный тип события: {TypeEvent}.");
            }

            Debug.Assert(nameText.Length > 0);

            SetEvent(nameEvent, nameText, colorNameEntity);

            Width = 52 + 399;
        }

        private void CloseSelf()
        {
            Debug.Assert(Visible);

            Visible = false;
            Program.formMain.CurrentLobby.CurrentPlayer.RemoveEventForPlayer(this);
            Program.formMain.ShowPlayersEvents();
            Program.formMain.NeedRedrawFrame();
            Dispose();
        }

        private void Cell_RightClick(object sender, EventArgs e)
        {
            CloseSelf();
        }

        private void Cell_Click(object sender, EventArgs e)
        {
            if (Entity is Construction c)
                Program.formMain.SelectConstruction(c);
            else if (Entity is ConstructionProduct cp)
            {
                Program.formMain.SelectConstruction(cp.Construction, 0);
            }
            else
                throw new Exception("Неизвестная сущность.");

            CloseSelf();
        }

        internal TypeEventForPlayer TypeEvent { get; }
    }
}
