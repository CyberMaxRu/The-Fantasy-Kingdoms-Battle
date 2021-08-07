using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настроек типа лобби
    internal sealed class TypeLobbyLayerSettings
    {
        public TypeLobbyLayerSettings(XmlNode n, int quantitySlotLairs)
        {
            Number = XmlUtils.GetInteger(n.SelectSingleNode("Number"));
            CostScout = XmlUtils.GetInteger(n.SelectSingleNode("CostScout"));
            CostAttack = XmlUtils.GetInteger(n.SelectSingleNode("CostAttack"));
            CostDefense = XmlUtils.GetInteger(n.SelectSingleNode("CostDefense"));

            Debug.Assert(CostScout > 0);
            Debug.Assert(CostScout <= 10_000);
            Debug.Assert(CostAttack > 0);
            Debug.Assert(CostAttack <= 10_000);
            Debug.Assert(CostDefense > 0);
            Debug.Assert(CostDefense <= 10_000);

            // Загружаем конфигурацию логов
            XmlNode ne = n.SelectSingleNode("Lairs");
            TypeLobbyLairSettings tlls;
            foreach (XmlNode l in ne.SelectNodes("Lair"))
            {
                tlls = new TypeLobbyLairSettings(l);

                // Проверяем, что тип логова не повторяется
                foreach (TypeLobbyLairSettings ls in LairsSettings)
                {
                    if (tlls.NameTypeLair == ls.NameTypeLair)
                        throw new Exception($"Тип логова {tlls.NameTypeLair} повторяется в списке типов логов слоя {Number}.");
                }

                LairsSettings.Add(tlls);
            }

            // Проверяем, что максимального количества хватает для заполнения логов, а минимальное не превышает число слотов
            int minQuantity = 0;
            int maxQuantity = 0;
            foreach (TypeLobbyLairSettings ls in LairsSettings)
            {
                minQuantity += ls.MinQuantity;
                maxQuantity += ls.MaxQuantity;
            }

            // Если количество сооружений меньше количества слотов, добиваем их пустыми местами
            if (maxQuantity < quantitySlotLairs)
            {
                // Если надо добавлять пустые места, то не должно быть в настройках пустого места. Если есть - значит, неправильно настроено количество
                foreach (TypeLobbyLairSettings ls in LairsSettings)
                    Debug.Assert(ls.NameTypeLair != FormMain.Config.IDEmptyPlace);

                TypeLobbyLairSettings s = new TypeLobbyLairSettings(FormMain.Config.IDEmptyPlace, quantitySlotLairs - maxQuantity);
                LairsSettings.Add(s);
                minQuantity += s.MinQuantity;
                maxQuantity += s.MaxQuantity;
            }

            Debug.Assert(minQuantity <= quantitySlotLairs);
            Debug.Assert(maxQuantity >= quantitySlotLairs);
        }

        internal int Number { get; }// Номер слоя
        internal int CostScout { get; }// Стоимость разведки
        internal int CostAttack { get; }// Стоимость атаки
        internal int CostDefense { get; }// Стоимость защиты
        internal List<TypeLobbyLairSettings> LairsSettings { get; } = new List<TypeLobbyLairSettings>();// Настройки типов логов для слоя

        internal void TuneDeferredLinks()
        {
            foreach (TypeLobbyLairSettings ls in LairsSettings)
            {
                ls.TuneDeferredLinks();
            }
        }
    }

    internal sealed class TypeLobbyLairSettings
    {
        public TypeLobbyLairSettings(XmlNode n)
        {
            NameTypeLair = n.SelectSingleNode("ID").InnerText;
            MinQuantity = XmlUtils.GetInteger(n.SelectSingleNode("MinQuantity"));
            MaxQuantity = XmlUtils.GetInteger(n.SelectSingleNode("MaxQuantity"));

            Debug.Assert(MinQuantity >= 0);
            Debug.Assert(MaxQuantity < 20);
            Debug.Assert(MinQuantity <= MaxQuantity);
        }

        public TypeLobbyLairSettings(string ID, int quantity)
        {
            NameTypeLair = ID;
            MinQuantity = quantity;
            MaxQuantity = quantity;

            Debug.Assert(MinQuantity >= 0);
            Debug.Assert(MaxQuantity < 20);
            Debug.Assert(MinQuantity <= MaxQuantity);
        }

        internal void TuneDeferredLinks()
        {
            TypeLair = FormMain.Config.FindTypeConstruction(NameTypeLair);
            NameTypeLair = null;
        }

        internal string NameTypeLair { get; private set; }
        internal TypeConstruction TypeLair { get; private set; }
        internal int MinQuantity { get; }
        internal int MaxQuantity { get; }
    }
}
