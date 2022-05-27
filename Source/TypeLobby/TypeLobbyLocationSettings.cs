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

            Name2 = XmlUtils.GetStringNotNull(n, "Name2");
            nameTypeLandscape = XmlUtils.GetStringNotNull(n, "TypeLandscape");
            Visible = XmlUtils.GetBooleanNotNull(n, "Visible");
            PercentScoutedArea = XmlUtils.GetPercentNotNull(n, "PercentScoutedArea");
            PercentScoutAreaByUnit = XmlUtils.GetPercentNotNull(n, "PercentScoutAreaByUnit");

            TypeLobbyLairSettings tlls;

            // Загружаем конфигурацию логов
            XmlNode ne = n.SelectSingleNode("Constructions");
            foreach (XmlNode l in ne.SelectNodes("Construction"))
            {
                tlls = new TypeLobbyLairSettings(l, this);

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
            Debug.Assert(PercentScoutedArea >= 0);
            Debug.Assert(PercentScoutedArea <= 1000);
            Debug.Assert(PercentScoutAreaByUnit > 0);
            Debug.Assert(PercentScoutAreaByUnit <= 1000);
        }

        internal string Name2 { get; }// Наименование в падеже
        internal TypeLobby TypeLobby { get; }// Тип лобби
        internal DescriptorTypeLandscape TypeLandscape { get; private set; }
        internal List<TypeLobbyLairSettings> LairsSettings { get; } = new List<TypeLobbyLairSettings>();// Настройки типов логов для слоя
        internal bool Visible { get; }
        internal int PercentScoutedArea { get; }// Процент разведанной части локации
        internal int PercentScoutAreaByUnit { get; }// Процент разведки локации за единицу разведки

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
