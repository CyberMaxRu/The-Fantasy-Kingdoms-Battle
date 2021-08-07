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
        public PlayerMapObject(LobbyPlayer player, TypeMapObject typeMapObject)
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
                                Researches.Add(new PlayerResearch(this, typeMapObject.Researches[z, y, x]));
            }
        }

        internal LobbyPlayer Player { get; }
        internal TypeMapObject TypeMapObject { get; }
        internal List<PlayerResearch> Researches { get; } = new List<PlayerResearch>();

        internal abstract bool CheckRequirementsForResearch(PlayerResearch research);
        internal abstract List<TextRequirement> GetTextRequirements(PlayerResearch research);
        internal virtual void ResearchCompleted(PlayerResearch research)
        {
            Debug.Assert(CheckRequirementsForResearch(research));

            Player.SpendGold(research.Cost());
            Researches.Remove(research);

        }
    }
}
