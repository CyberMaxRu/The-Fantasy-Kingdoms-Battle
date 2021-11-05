using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypeNoticeForPlayer { Build, Research, Extension, MassEventBegin, MassEventEnd, TournamentBegin, TournamentEnd };

    internal sealed class VCNoticeForPlayer : VCCustomNotice
    {
        public VCNoticeForPlayer(Entity entity, TypeNoticeForPlayer typeNotice) : base(52 + 399)
        {
            Debug.Assert(entity != null);

            Entity = entity;

            TypeNotice = typeNotice;

            Visible = false;
            Cell.Click += Cell_Click;
            Cell.RightClick += Cell_RightClick;
            Cell.HighlightUnderMouse = true;

            string nameNotice;
            string nameText = "";
            Color colorNameEntity;
            switch (TypeNotice)
            {
                case TypeNoticeForPlayer.Build:
                    nameNotice = "Строительство завершено:";
                    nameText = (Entity as Construction).NameLair();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Research:
                    nameNotice = "Исследование завершено:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Extension:
                    nameNotice = "Дополнительное сооружение построено:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.MassEventBegin:
                    nameNotice = "Мероприятие начато:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.MassEventEnd:
                    nameNotice = "Мероприятие завершено:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.TournamentBegin:
                    nameNotice = "Турнир начат:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.TournamentEnd:
                    nameNotice = "Турнир завершен:";
                    nameText = (Entity as ConstructionProduct).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                default:
                    throw new Exception($"Неизвестный тип события: {TypeNotice}.");
            }

            Debug.Assert(nameText.Length > 0);

            SetNotice(Entity.GetImageIndex(), nameNotice, nameText, colorNameEntity);
        }

        private void CloseSelf()
        {
            Debug.Assert(Visible);

            Visible = false;
            Program.formMain.CurrentLobby.CurrentPlayer.RemoveNoticeForPlayer(this);
            Program.formMain.ShowPlayersNotices();
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

        internal TypeNoticeForPlayer TypeNotice { get; }
    }
}
