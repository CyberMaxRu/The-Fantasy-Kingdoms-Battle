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
        }

        internal void PlaySoundSelect()
        {
            Program.formMain.PlaySoundSelect(uriSoundSelect);
        }
    }
}
