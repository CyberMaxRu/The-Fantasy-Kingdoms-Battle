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
        ReceivedBaseResource, Explore, HeroIsDead, FoundLocation, ConstructionDamaged, ConstructionRepaired, AddQuest };

    internal sealed class VCNoticeForPlayer : VCCustomNotice
    {
        public VCNoticeForPlayer(Entity entity, TypeNoticeForPlayer typeNotice, int addParam) : base()
        {
            Debug.Assert(entity != null);
            Debug.Assert(typeNotice != TypeNoticeForPlayer.None);

            Entity = entity;

            TypeNotice = typeNotice;

            Visible = false;
            CellEntity.RightClick += Cell_RightClick;
            RightClick += Cell_RightClick;

            if (typeNotice != TypeNoticeForPlayer.ReceivedBaseResource)
            {
                CellOwner.Click += Cell_Click;
                CellEntity.Click += Cell_Click;
                CellEntity.HighlightUnderMouse = true;
            }

            int imageIndexOwner = -1;
            string nameNotice;
            string nameText = "";
            Color colorNameEntity;
            switch (TypeNotice)
            {
                case TypeNoticeForPlayer.Build:
                    nameNotice = "Построено:";
                    nameText = (Entity as Construction).GetName();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.LevelUp:
                    nameNotice = "Улучшено:";

                    if ((Entity as Construction).Descriptor.Levels[(Entity as Construction).Level].NewName)
                        nameText = (Entity as Construction).GetNameForLevel((Entity as Construction).Level - 1) + " до " + (Entity as Construction).GetName() + " (Ур. " + (Entity as Construction).Level.ToString() + ")";
                    else
                        nameText = (Entity as Construction).GetName() + " Уровень " + (Entity as Construction).Level.ToString();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Research:
                    imageIndexOwner = (Entity as EntityForConstruction).Construction.GetImageIndex();
                    nameNotice = "Исследовано:";
                    nameText = (Entity as EntityForConstruction).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Extension:
                    imageIndexOwner = (Entity as ConstructionExtension).Construction.GetImageIndex();
                    nameNotice = "Построено:";
                    nameText = (Entity as ConstructionExtension).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.Improvement:
                    imageIndexOwner = (Entity as ConstructionImprovement).Construction.GetImageIndex();
                    nameNotice = "Улучшено:";
                    nameText = (Entity as ConstructionImprovement).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.HireHero:
                    imageIndexOwner = (Entity as Creature).Construction.GetImageIndex();
                    nameNotice = "Герой нанят:";
                    nameText = (Entity as Creature).GetNameHero();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.MassEventBegin:
                    imageIndexOwner = (Entity as ConstructionEvent).Construction.GetImageIndex();
                    nameNotice = "Мероприятие:";
                    nameText = (Entity as ConstructionEvent).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.MassEventEnd:
                    imageIndexOwner = (Entity as ConstructionEvent).Construction.GetImageIndex();
                    nameNotice = "Завершено:";
                    nameText = (Entity as ConstructionEvent).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.TournamentBegin:
                    imageIndexOwner = (Entity as ConstructionTournament).Construction.GetImageIndex();
                    nameNotice = "Турнир:";
                    nameText = (Entity as ConstructionTournament).Descriptor.Name;
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.TournamentEnd:
                    imageIndexOwner = (Entity as ConstructionTournament).Construction.GetImageIndex();
                    nameNotice = "Завершено:";
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
                    Creature h = Entity as Creature;
                    nameNotice = $"Герой погиб ({h.TypeCreature.Name}):";
                    nameText = $"{h.FullName}";
                    colorNameEntity = Color.SteelBlue;
                    break;
                case TypeNoticeForPlayer.ConstructionDamaged:
                    Debug.Assert(addParam > 0);
                    nameNotice = $"Повреждено ({-addParam}):";
                    nameText = (Entity as Construction).GetName();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.ConstructionRepaired:
                    nameNotice = "Отремонтировано:";
                    nameText = (Entity as Construction).GetName();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                case TypeNoticeForPlayer.AddQuest:
                    imageIndexOwner = (Entity as PlayerQuest).Player.GetImageIndex();
                    nameNotice = "Новое задание:";
                    nameText = (Entity as PlayerQuest).GetName();
                    colorNameEntity = Color.DarkGoldenrod;
                    break;
                default:
                    throw new Exception($"Неизвестный тип события: {TypeNotice}.");
            }

            Debug.Assert(nameText.Length > 0);

            SetNotice(imageIndexOwner, Entity.GetImageIndex(), nameNotice, nameText, colorNameEntity);
        }

        public VCNoticeForPlayer(int imageIndexOwner, int imageIndexEnity, string caption, string text, Color color) : base()
        {
            SetNotice(imageIndexOwner, imageIndexEnity, caption, text, color);

            CellEntity.RightClick += Cell_RightClick;
            RightClick += Cell_RightClick;
        }

        internal void CloseSelf()
        {
            Debug.Assert(Visible);

            Visible = false;
            Program.formMain.layerGame.CurrentLobby.CurrentPlayer.RemoveNoticeForPlayer(this);
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
            else if (Entity is Creature h)
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
