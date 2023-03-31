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

        public LobbySettingsPlayer(LobbySettingsPlayer lsp)
        {
            Index = lsp.Index;
            defaultPlayer = lsp.defaultPlayer;
            TypePlayer = lsp.TypePlayer;
            Player = lsp.Player;
            TypeSelectPersistentBonus = lsp.TypeSelectPersistentBonus;
            TypeSelectStartBonus = lsp.TypeSelectStartBonus;
        }

        internal int Index { get; }
        internal TypePlayer TypePlayer { get; set; }    
        internal DescriptorPlayer Player { get; set; }
        internal TypeSelectBonus TypeSelectPersistentBonus { get; set; }
        internal TypeSelectBonus TypeSelectStartBonus { get; set; }
        internal DescriptorTypeTradition TypeTradition1 { get; private set; }
        internal DescriptorTypeTradition TypeTradition2 { get; private set; }
        internal DescriptorTypeTradition TypeTradition3 { get; private set; }

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

        internal void SetTypeTraditions(DescriptorTypeTradition type1, DescriptorTypeTradition type2, DescriptorTypeTradition type3)
        {
            List<DescriptorTypeTradition> listAccepted = new List<DescriptorTypeTradition>();
            List<DescriptorTypeTradition> listCandidates = new List<DescriptorTypeTradition>();
            listCandidates.AddRange(FormMain.Descriptors.TypeTraditions);

            ApplyType(type1);
            ApplyType(type2);
            ApplyType(type3);

            MakeExistType(ref type1);
            MakeExistType(ref type2);
            MakeExistType(ref type3);

            Assert(type1 != null);
            Assert(type2 != null);
            Assert(type3 != null);
            Assert(type1 != type2);
            Assert(type1 != type3);
            Assert(type2 != type3);

            TypeTradition1 = type1;
            TypeTradition2 = type2;
            TypeTradition3 = type3;

            void ApplyType(DescriptorTypeTradition t)
            {
                if (t != null)
                {
                    Assert(listAccepted.IndexOf(t) == -1);
                    listAccepted.Add(t);

                    Assert(listCandidates.IndexOf(t) != -1);
                    listCandidates.Remove(t);
                }

            }

            void MakeExistType(ref DescriptorTypeTradition t)
            {
                if (t is null)
                {
                    t = GenerateRandomTypeTradition();
                    ApplyType(t);
                }
            }

            DescriptorTypeTradition GenerateRandomTypeTradition()
            {
                Random r = new Random();
                return listCandidates[r.Next(listCandidates.Count)];
            }
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
