using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс объекта карты игрока
    internal abstract class PlayerMapObject : PlayerObject
    {
        public PlayerMapObject(LobbyPlayer player, TypeObjectOfMap typeMapObject)
        {
            Player = player;
            TypeMapObject = typeMapObject;

            // Настраиваем исследования 
            if (typeMapObject.Researches != null)
            {
                for (int z = 0; z < typeMapObject.Researches.GetLength(0); z++)
                    for (int y = 0; y < typeMapObject.Researches.GetLength(1); y++)
                        for (int x = 0; x < typeMapObject.Researches.GetLength(2); x++)
                            if (typeMapObject.Researches[z, y, x] != null)
                                Researches.Add(new PlayerCellMenu(this, typeMapObject.Researches[z, y, x]));
            }
        }

        internal LobbyPlayer Player { get; }
        internal TypeObjectOfMap TypeMapObject { get; }
        internal List<PlayerCellMenu> Researches { get; } = new List<PlayerCellMenu>();

        internal abstract bool CheckRequirementsForResearch(PlayerCellMenu research);
        internal abstract List<TextRequirement> GetTextRequirements(PlayerCellMenu research);
        internal abstract bool ShowMenuForPlayer();

        internal virtual void ResearchCompleted(PlayerCellMenu research)
        {
            Debug.Assert(CheckRequirementsForResearch(research));

            Player.SpendGold(research.Cost());
            Researches.Remove(research);

        }
    }
}
