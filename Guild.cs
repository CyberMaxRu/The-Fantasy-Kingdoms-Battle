using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс гильдии
    internal sealed class Guild
    {
        public Guild(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
            LevelCastle = Convert.ToInt32(n.SelectSingleNode("LevelCastle").InnerText);
            Position = FormMain.Config.Guilds.Count;
            MaxHeroes = Convert.ToInt32(n.SelectSingleNode("MaxHeroes").InnerText);

            // Проверяем, что таких же ID и наименования нет
            foreach (Guild g in FormMain.Config.Guilds)
            {
                if (g.ID == ID)
                {
                    throw new Exception("В конфигурации гильдий повторяется ID = " + ID);
                }

                if (g.Name == Name)
                {
                    throw new Exception("В конфигурации гильдий повторяется Name = " + Name);
                }

                if (g.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации гильдий повторяется ImageIndex = " + ImageIndex.ToString());
                }
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int Position { get; }
        internal int ImageIndex { get; }
        internal int MaxHeroes { get; }
        internal int Cost { get; }
        internal int LevelCastle { get; }
        internal PanelGuild Panel { get; set; }
        internal Hero TrainedHero { get; set; }
    }
}
