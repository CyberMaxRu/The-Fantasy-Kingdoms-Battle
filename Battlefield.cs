using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;
using System.Diagnostics;

namespace Fantasy_King_s_Battle
{
    internal sealed class CellBattlefield
    {
        public CellBattlefield(CategoryHero category)
        {
            CategoryHero = category;
        }

        internal CategoryHero CategoryHero { get; }
        internal PlayerHero Hero { get; set; }
    }

    // Класс игрового поля
    internal sealed class Battlefield
    {
        public Battlefield(string filenameConfigBattlefield)
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(filenameConfigBattlefield);
            XmlNode n = XmlDoc.SelectSingleNode("Battlefield");

            Size = new Size(Convert.ToInt32(n.Attributes["width"].Value), Convert.ToInt32(n.Attributes["height"].Value));
            Cells = new CellBattlefield[Size.Height, Size.Width];

            int y, x;
            XmlNode xmlCells = n.SelectSingleNode("Cells");
            foreach (XmlNode c in xmlCells.SelectNodes("Cell"))
            {
                x = Convert.ToInt32(c.Attributes["x"].Value);
                y = Convert.ToInt32(c.Attributes["y"].Value);
                Debug.Assert(Cells[y, x] == null);
                if (c.InnerText.Length > 0)
                {
                    Cells[y, x] = new CellBattlefield((CategoryHero)Enum.Parse(typeof(CategoryHero), c.InnerText));
                }
            }
        }

        internal Size Size { get; }
        internal CellBattlefield[,] Cells { get; }
    }
}
