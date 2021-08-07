using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Тип объекта карты - базовый класс для всех зданий, построек и логов
    internal abstract class TypeMapObject : TypeObject
    {
        private Uri uriSoundSelect;// Звук при выборе объекта

        public TypeMapObject(XmlNode n) : base(n)
        {
            string filenameSoundSelect = XmlUtils.GetString(n.SelectSingleNode("SoundSelect"));
            if (filenameSoundSelect.Length > 0)
                uriSoundSelect = new Uri(Program.formMain.dirResources + @"Sound\Interface\ConstructionSelect\" + filenameSoundSelect);

            // Загружаем исследования
            XmlNode nr = n.SelectSingleNode("Researches");
            if (nr != null)
            {
                Researches = new Research[Convert.ToInt32(n.SelectSingleNode("LayersResearches").InnerText), FormMain.Config.PlateHeight, FormMain.Config.PlateWidth];

                Research research;

                foreach (XmlNode l in nr.SelectNodes("Research"))
                {
                    research = new Research(l);
                    Debug.Assert(Researches[research.Layer, research.Coord.Y, research.Coord.X] == null);
                    Researches[research.Layer, research.Coord.Y, research.Coord.X] = research;
                }
            }
        }

        internal Research[,,] Researches;

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
                            Researches[z, y, x]?.FindItem();
            }
        }
    }
}
