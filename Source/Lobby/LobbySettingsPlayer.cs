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
    internal enum TypeSelectBonus { Manual, Random };

    // Настройка игрока лобби
    internal sealed class LobbySettingsPlayer
    {
        private string idPlayer;
        private DescriptorPlayer defaultPlayer;

        public LobbySettingsPlayer(int index, DescriptorPlayer player)
        {
            Index = index;
            Player = player;
            defaultPlayer = player;

            if (player != null)
                TypePlayer = player.TypePlayer;
            else
                TypePlayer = TypePlayer.Computer;

            SetDefault();
        }

        public LobbySettingsPlayer(XmlNode n, DescriptorPlayer defPlayer)
        {
            Index = GetIntegerNotNull(n, "Index");
            if (Index == 0)
                defaultPlayer = defPlayer;
            idPlayer = GetString(n, "Player");

            TypeSelectPersistentBonus = (TypeSelectBonus)Enum.Parse(typeof(TypeSelectBonus), n.SelectSingleNode("TypeSelectPersistentBonus").InnerText);
            TypeSelectStartBonus = (TypeSelectBonus)Enum.Parse(typeof(TypeSelectBonus), n.SelectSingleNode("TypeSelectStartBonus").InnerText);
        }

        internal int Index { get; }
        internal TypePlayer TypePlayer { get; set; }    
        internal DescriptorPlayer Player { get; set; }
        internal TypeSelectBonus TypeSelectPersistentBonus { get; set; }
        internal TypeSelectBonus TypeSelectStartBonus { get; set; }

        internal void SetDefault()
        {
            Player = defaultPlayer;

            if (TypePlayer == TypePlayer.Human)
            {
                TypeSelectPersistentBonus = TypeSelectBonus.Manual;
                TypeSelectStartBonus = TypeSelectBonus.Manual;
            }
            else
            {
                TypeSelectPersistentBonus = TypeSelectBonus.Random;
                TypeSelectStartBonus = TypeSelectBonus.Random;
            }
        }

        internal void TuneLinks()
        {
            if (idPlayer.Length > 0)
                Player = FormMain.Descriptors.FindPlayer(idPlayer);
            else
                TypePlayer = TypePlayer.Computer;
        }

        internal void SaveToXml(XmlWriter writer)
        {
            writer.WriteElementString("Index", Index.ToString());
            if (Player != null)
                writer.WriteElementString("Player", Player.ID);
            else
                writer.WriteElementString("Player", "");

            writer.WriteElementString("TypeSelectPersistentBonus", TypeSelectPersistentBonus.ToString());
            writer.WriteElementString("TypeSelectStartBonus", TypeSelectStartBonus.ToString());
        }
    }
}
