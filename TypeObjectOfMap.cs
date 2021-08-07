using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип объекта карты - базовый класс для всех зданий, построек и мест
    internal abstract class TypeObjectOfMap : TypeObject
    {
        private Uri uriSoundSelect;// Звук при выборе объекта

        public TypeObjectOfMap(XmlNode n) : base(n)
        {
            uriSoundSelect = new Uri(Program.formMain.dirResources + @"Sound\Interface\ConstructionSelect\" + XmlUtils.GetStringNotNull(n.SelectSingleNode("SoundSelect")));

            // Загружаем исследования
            int layersResearches = XmlUtils.GetInteger(n.SelectSingleNode("LayersResearches"));
            XmlNode nr = n.SelectSingleNode("Researches");
            if (nr != null)
            {
                Debug.Assert(layersResearches > 0);
                Researches = new CellMenu[layersResearches, FormMain.Config.PlateHeight, FormMain.Config.PlateWidth];

                CellMenu research;

                foreach (XmlNode l in nr.SelectNodes("Research"))
                {
                    research = new CellMenu(l);
                    Debug.Assert(Researches[research.Layer, research.Coord.Y, research.Coord.X] == null);
                    Researches[research.Layer, research.Coord.Y, research.Coord.X] = research;
                }
            }
            else
            {
                Debug.Assert(layersResearches == 0);
            }
        }

        internal CellMenu[,,] Researches;

        internal void PlaySoundSelect()
        {
            Program.formMain.PlaySoundSelect(uriSoundSelect);
        }

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            if (Researches != null)
            {
                for (int z = 0; z < Researches.GetLength(0); z++)
                    for (int y = 0; y < Researches.GetLength(1); y++)
                        for (int x = 0; x < Researches.GetLength(2); x++)
                            Researches[z, y, x]?.TuneDeferredLinks();
            }
        }
    }
}
