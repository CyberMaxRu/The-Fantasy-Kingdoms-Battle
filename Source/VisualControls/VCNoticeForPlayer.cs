using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal enum TypeNoticeForPlayer { None, Build, LevelUp, Research, Extension, Improvement, HireHero, MassEventBegin, MassEventEnd, TournamentBegin, TournamentEnd,
        ReceivedBaseResource, Explore, HeroIsDead, FoundLocation, ConstructionDamaged, ConstructionRepaired };

    internal sealed class VCNoticeForPlayer : VCCustomNotice
    {
        public VCNoticeForPlayer(Entity entity, TypeNoticeForPlayer typeNotice, int addParam) : base(52 + 399)
        {
            Debug.Assert(entity != null);
            Debug.Assert(typeNotice != TypeNoticeForPlayer.None);

            Entity = entity;

            TypeNotice = typeNotice;

            Visible = false;
            Cell.RightClick += Cell_RightClick;

            if (typeNotice != TypeNoticeForPlayer.ReceivedBaseResource)
            {
                Cell.Click += Cell_Click;
                Cell.HighlightUnderMouse = true;
            }

            string nameNotice;
            string nameText = "";
            Color colorNameEntity;
            switch (TypeNotice)
            {
                case TypeNoticeForPlayer.Build:
                    nameNotice = "Строительство завершено:";
                    nameText = (Entity as Construction).GetName();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.LevelUp:
                    nameNotice = "Сооружение улучшено:";
                    nameText = (Entity as Construction).GetName() + " Уровень " + (Entity as Construction).Level.ToString();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Research:
                    nameNotice = "Исследование завершено:";
                    nameText = (Entity as EntityForConstruction).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Extension:
                    nameNotice = "Дополнительное сооружение построено:";
                    nameText = (Entity as ConstructionExtension).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Improvement:
                    nameNotice = "Улучшение завершено:";
                    nameText = (Entity as ConstructionImprovement).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.HireHero:
                    nameNotice = "Герой нанят:";
                    nameText = (Entity as Hero).GetNameHero();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.MassEventBegin:
                    nameNotice = "Мероприятие начато:";
                    nameText = (Entity as ConstructionEvent).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.MassEventEnd:
                    nameNotice = "Мероприятие завершено:";
                    nameText = (Entity as ConstructionEvent).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.TournamentBegin:
                    nameNotice = "Турнир начат:";
                    nameText = (Entity as ConstructionTournament).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.TournamentEnd:
                    nameNotice = "Турнир завершен:";
                    nameText = (Entity as ConstructionTournament).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.ReceivedBaseResource:
                    BaseResource br = Entity as BaseResource;
                    nameNotice = $"Поступил ресурс:";
                    nameText = $"+{br.Quantity}";
                    colorNameEntity = Color.LimeGreen;
                    break;
                case TypeNoticeForPlayer.Explore:
                    if (Entity is Construction c)
                    {
                        nameNotice = $"В {c.Location.Settings.Name2} обнаружен объект:";
                        nameText = $"{c.GetName()}";
                        colorNameEntity = Color.DarkGoldenrod;
                    }
                    else
                        throw new Exception("Неизвестный тип сущности");
                    break;
                case TypeNoticeForPlayer.FoundLocation:
                    if (Entity is Location l)
                    {
                        nameNotice = $"Обнаружена локация:";
                        nameText = $"{l.Settings.Name}";
                        colorNameEntity = Color.DarkGoldenrod;
                    }
                    else
                        throw new Exception("Неизвестный тип сущности");
                    break;
                case TypeNoticeForPlayer.HeroIsDead:
                    Hero h = Entity as Hero;
                    nameNotice = $"Герой погиб ({h.TypeCreature.Name}):";
                    nameText = $"{h.FullName}";
                    colorNameEntity = Color.SteelBlue;
                    break;
                case TypeNoticeForPlayer.ConstructionDamaged:
                    Debug.Assert(addParam > 0);
                    nameNotice = $"Сооружение повреждено ({-addParam}):";
                    nameText = (Entity as Construction).GetName();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.ConstructionRepaired:
                    nameNotice = "Сооружение отремонтировано:";
                    nameText = (Entity as Construction).GetName();
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
            Program.formMain.layerGame.CurrentLobby.CurrentPlayer.RemoveNoticeForPlayer(this);
            Program.formMain.layerGame.ShowPlayerNotices();
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
                Program.formMain.layerGame.SelectConstruction(c);
            else if (Entity is Hero h)
            {
                if (h.IsLive)
                    Program.formMain.layerGame.SelectPlayerObject(h);
            }
            else if (Entity is ConstructionProduct cp)
            {
                Program.formMain.layerGame.SelectConstruction(cp.Construction, 0);
            }
            else if (Entity is ConstructionExtension ce)
            {
                Program.formMain.layerGame.SelectConstruction(ce.Construction, 0);
            }
            else if (Entity is ConstructionImprovement ci)
            {
                Program.formMain.layerGame.SelectConstruction(ci.Construction, 0);
            }
            else if (Entity is Location l)
            {
                Program.formMain.layerGame.SelectPlayerObject(l, 0);
            }
            else
                throw new Exception("Неизвестная сущность.");

            CloseSelf();
        }

        internal TypeNoticeForPlayer TypeNotice { get; }
    }
}
