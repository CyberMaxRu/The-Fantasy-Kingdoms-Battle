using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настройки локации типа лобби
    internal sealed class TypeLobbyLocationSettings : DescriptorWithID
    {
        private string nameTypeLandscape;

        public TypeLobbyLocationSettings(TypeLobby typeLobby, XmlNode n, int quantitySlotLairs) : base(n)
        {
            TypeLobby = typeLobby;

            nameTypeLandscape = XmlUtils.GetStringNotNull(n, "TypeLandscape");
            VisibleByDefault = XmlUtils.GetBooleanNotNull(n, "VisibleByDefault");
            Area = XmlUtils.GetIntegerNotNull(n, "Area");
            ScoutedArea = XmlUtils.GetIntegerNotNull(n, "ScoutedArea");
            BaseScoutingArea = XmlUtils.GetIntegerNotNull(n, "BaseScoutingArea");

            TypeLobbyLairSettings tlls;

            // Загружаем конфигурацию логов
            XmlNode ne = n.SelectSingleNode("TypeConstructions");
            foreach (XmlNode l in ne.SelectNodes("TypeConstruction"))
            {
                tlls = new TypeLobbyLairSettings(l);

                /*// Проверяем, что тип логова не повторяется
                foreach (TypeLobbyLairSettings ls in LairsSettings)
                {
                    if (tlls.NameTypeLair == ls.NameTypeLair)
                        throw new Exception($"Тип логова {tlls.NameTypeLair} повторяется в списке типов логов локации {ID}.");
                }*/

                LairsSettings.Add(tlls);
            }

            // Если количество сооружений меньше количества слотов, добиваем их пустыми местами
            /*if (maxQuantity < quantitySlotLairs)
            {
                // Если надо добавлять пустые места, то не должно быть в настройках пустого места. Если есть - значит, неправильно настроено количество
                foreach (TypeLobbyLairSettings ls1 in LairsSettings)
                    Debug.Assert(ls1.NameTypeLair != nameDefaultConstruction);

                TypeLobbyLairSettings ls = new TypeLobbyLairSettings(nameDefaultConstruction, quantitySlotLairs - maxQuantity);
                LairsSettings.Add(ls);
                minQuantity += ls.MinQuantity;
                maxQuantity += ls.MaxQuantity;
            }*/

            //Debug.Assert(minQuantity <= quantitySlotLairs);
            //Debug.Assert(maxQuantity >= quantitySlotLairs);
            Debug.Assert(Area >= 1);
            Debug.Assert(Area <= 1_000_000);
            Debug.Assert(ScoutedArea >= 0);
            Debug.Assert(ScoutedArea <= Area);
            Debug.Assert(BaseScoutingArea > 0);
            Debug.Assert(BaseScoutingArea <= Area);
        }

        internal TypeLobby TypeLobby { get; }// Тип лобби
        internal DescriptorTypeLandscape TypeLandscape { get; private set; }
        internal List<TypeLobbyLairSettings> LairsSettings { get; } = new List<TypeLobbyLairSettings>();// Настройки типов логов для слоя
        internal bool VisibleByDefault { get; }
        internal int Area { get; }// Площадь локации
        internal int ScoutedArea { get; }// Разведанная площадь локации (изначально)
        internal int BaseScoutingArea { get; }// Базовая разведываемая площадь

        internal override void TuneLinks()
        {
            base.TuneLinks();

            TypeLandscape = FormMain.Descriptors.FindTypeLandscape(nameTypeLandscape);
            nameTypeLandscape = "";

            foreach (TypeLobbyLairSettings ls in LairsSettings)
            {
                ls.TuneDeferredLinks();
            }
        }
    }    
}
