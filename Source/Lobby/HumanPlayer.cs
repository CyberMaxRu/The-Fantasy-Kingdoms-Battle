using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс игрока-человека
    internal sealed class HumanPlayer: DescriptorPlayer
    {
        public HumanPlayer(XmlNode n) : base(n, TypePlayer.Human)
        {
            Debug.Assert(Name.Length <= 31);
            Debug.Assert(Description.Length <= 100);

            DirectoryAvatar = XmlUtils.GetString(n, "DirectoryAvatar");

            foreach (HumanPlayer hp in FormMain.Descriptors.HumanPlayers)
            {
                Debug.Assert(ID != hp.ID);
                Debug.Assert(Name != hp.Name);
                Debug.Assert(ImageIndex != hp.ImageIndex);
            }

            foreach (ComputerPlayer cp in FormMain.Descriptors.ComputerPlayers)
            {
                Debug.Assert(ID != cp.ID);
                Debug.Assert(Name != cp.Name);
            }

            DisableComputerPlayerByAvatar();

            // Создаем настройки всех типов лобби
            Assert(Descriptors.TypeLobbies.Count > 0);
            TournamentSettings = new LobbySettings[Descriptors.TypeLobbies.Count];

            XmlNode ns = n.SelectSingleNode("TournamentSettings");
            if (ns != null)
            { 
                foreach (XmlNode nt in ns.SelectNodes("Tournament"))
                {
                    LobbySettings ls = new LobbySettings(nt, this);

                    Assert(TournamentSettings[ls.TypeLobby.Index] is null);
                    TournamentSettings[ls.TypeLobby.Index] = ls;
                }
            }

            for (int i = 0; i < TournamentSettings.Length; i++)
            {
                if (TournamentSettings[i] is null)
                    TournamentSettings[i] = new LobbySettings(Descriptors.TypeLobbies[i], this);
            }
        }

        public HumanPlayer(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex, TypePlayer.Human)
        {
            Debug.Assert(Name.Length <= 31);
            Debug.Assert(Description.Length <= 100);

            DisableComputerPlayerByAvatar();
        }

        internal string DirectoryAvatar { get; set; }
        internal LobbySettings[] TournamentSettings { get; }

        protected override void CheckData()
        {
            base.CheckData();

            Debug.Assert(ImageIndex >= FormMain.Config.ImageIndexFirstAvatar);
            if (ImageIndex >= FormMain.Config.ImageIndexFirstAvatar + FormMain.Config.QuantityInternalAvatars + FormMain.Config.ExternalAvatars.Count)
                ImageIndex = FormMain.Config.ImageIndexFirstAvatar;
            Debug.Assert(ImageIndex < FormMain.Config.ImageIndexFirstAvatar + FormMain.Config.QuantityInternalAvatars + FormMain.Config.ExternalAvatars.Count);
        }

        private void DisableComputerPlayerByAvatar()
        {
            foreach (ComputerPlayer cp in FormMain.Descriptors.ComputerPlayers)
            {
                if (cp.ImageIndex == ImageIndex)
                {
                    cp.Active = false;
                    break;
                }
            }
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            foreach (LobbySettings ls in TournamentSettings)
                ls.TuneLinks();
        }

        internal void SaveToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("Player");
            writer.WriteElementString("ID", ID);
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("Description", Description);
            writer.WriteElementString("ImageIndex", (ImageIndex - FormMain.Config.ImageIndexFirstAvatar + 1).ToString());
            writer.WriteElementString("DirectoryAvatar", DirectoryAvatar);

            writer.WriteStartElement("TournamentSettings");
            foreach (LobbySettings lb in TournamentSettings)
            {
                writer.WriteStartElement("Tournament");
                lb.WriteToXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            // Конец данных игрока
            writer.WriteEndElement();
        }
    }
}
