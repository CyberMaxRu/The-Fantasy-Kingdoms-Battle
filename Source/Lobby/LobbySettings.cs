using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Fantasy_Kingdoms_Battle.Utils;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Настройки лобби
    internal sealed class LobbySettings
    {
        public LobbySettings(TypeLobby typeLobby, DescriptorPlayer player) : base()
        {
            TypeLobby = typeLobby;

            Players = new LobbySettingsPlayer[typeLobby.QuantityPlayers];
            Players[0] = new LobbySettingsPlayer(0, player);// Текущий игрок-человек всегда первый

            // Подбираем компьютерных игроков из пула доступных
            List<ComputerPlayer> listCompPlayers = new List<ComputerPlayer>();
            listCompPlayers.AddRange(FormMain.Descriptors.ComputerPlayers.Where(cp => cp.Active));
            Assert(listCompPlayers.Count >= TypeLobby.QuantityPlayers - 1);

            int idx;
            Random rnd = new Random();
            for (int i = 1; i < TypeLobby.QuantityPlayers; i++)
            {
                idx = rnd.Next(listCompPlayers.Count);
                Players[i] = new LobbySettingsPlayer(i, null);
                //Players[i] = new LobbySettingsPlayer(i, listCompPlayers[idx]);
                listCompPlayers.RemoveAt(idx);
            }
        }

        public LobbySettings(XmlNode n, DescriptorPlayer player) : base()
        {
            Players = new LobbySettingsPlayer[TypeLobby.QuantityPlayers];
            XmlNode np = n.SelectSingleNode("Players");
            if (np != null)
            {
                foreach (XmlNode lp in np.SelectNodes("Player"))
                {
                    LobbySettingsPlayer lsp = new LobbySettingsPlayer(lp, player);

                    Assert(Players[lsp.Index] is null);
                    Players[lsp.Index] = lsp;
                }
            }

            for (int i = 0; i < Players.Length; i++)
            {
                Assert(Players[i] != null);
            }

            Assert(Players[0].TypePlayer == TypePlayer.Human);
        }

        public LobbySettings(LobbySettings ls)
        {
            TypeLobby = ls.TypeLobby;

            Players = new LobbySettingsPlayer[ls.Players.Length];
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = new LobbySettingsPlayer(ls.Players[i]);
            }
        }

        internal TypeLobby TypeLobby { get; }
        internal LobbySettingsPlayer[] Players { get; }

        internal void TuneLinks()
        {
            for (int i = 0; i < Players.Length; i++)
                Players[i].TuneLinks();
        }

        internal void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("Players");
            foreach (LobbySettingsPlayer lsp in Players)
            {
                writer.WriteStartElement("Player");
                lsp.SaveToXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        internal void SetDefault()
        {
            for (int i = 0; i < Players.Length; i++)
                Players[i].SetDefault();
        }
    }
}
