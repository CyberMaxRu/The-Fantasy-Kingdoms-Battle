﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс игрока-человека
    internal sealed class HumanPlayer: Player
    {
        public HumanPlayer(XmlNode n) : base(n, TypePlayer.Human)
        {
            Debug.Assert(Name.Length <= 31);
            Debug.Assert(Description.Length <= 100);

            DirectoryAvatar = XmlUtils.GetString(n, "DirectoryAvatar");

            foreach (HumanPlayer hp in FormMain.Config.HumanPlayers)
            {
                Debug.Assert(ID != hp.ID);
                Debug.Assert(Name != hp.Name);
                Debug.Assert(ImageIndex != hp.ImageIndex);
            }

            foreach (ComputerPlayer cp in FormMain.Config.ComputerPlayers)
            {
                Debug.Assert(ID != cp.ID);
                Debug.Assert(Name != cp.Name);
            }

            DisableComputerPlayerByAvatar();
        }

        public HumanPlayer(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex, TypePlayer.Human)
        {
            Debug.Assert(Name.Length <= 31);
            Debug.Assert(Description.Length <= 100);

            DisableComputerPlayerByAvatar();
        }

        internal string DirectoryAvatar { get; set; }

        protected override void CheckData()
        {
            base.CheckData();

            Debug.Assert(ImageIndex >= FormMain.Config.ImageIndexFirstAvatar);
            Debug.Assert(ImageIndex < FormMain.Config.ImageIndexFirstAvatar + FormMain.Config.QuantityInternalAvatars + FormMain.Config.ExternalAvatars.Count);
        }

        private void DisableComputerPlayerByAvatar()
        {
            foreach (ComputerPlayer cp in FormMain.Config.ComputerPlayers)
            {
                if (cp.ImageIndex == ImageIndex)
                {
                    cp.Active = false;
                    break;
                }
            }
        }

        internal void SaveToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("Player");
            writer.WriteElementString("ID", ID);
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Description", Description);
            writer.WriteElementString("ImageIndex", (ImageIndex - FormMain.Config.ImageIndexFirstAvatar + 1).ToString());
            writer.WriteElementString("DirectoryAvatar", DirectoryAvatar);
            writer.WriteEndElement();
        }
    }
}