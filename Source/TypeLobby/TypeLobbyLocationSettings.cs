﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Класс настройки локации типа лобби
    internal sealed class TypeLobbyLocationSettings
    {
        public TypeLobbyLocationSettings(TypeLobby typeLobby, XmlNode n, int quantitySlotLairs)
        {
            TypeLobby = typeLobby;

            Number = XmlUtils.GetInteger(n, "Number");
            Location = FormMain.Config.FindLocation(XmlUtils.GetStringNotNull(n, "Location"));
            Ownership = XmlUtils.GetBooleanNotNull(n, "Ownership");
            CostScout = XmlUtils.GetInteger(n, "CostScout");
            CostAttack = XmlUtils.GetInteger(n, "CostAttack");
            CostDefense = XmlUtils.GetInteger(n, "CostDefense");

            Debug.Assert(CostScout > 0);
            Debug.Assert(CostScout <= 10_000);
            Debug.Assert(CostAttack > 0);
            Debug.Assert(CostAttack <= 10_000);
            Debug.Assert(CostDefense > 0);
            Debug.Assert(CostDefense <= 10_000);

            TypeLobbyLairSettings tlls;
            // Загружаем форт для каждой локации
            tlls = new TypeLobbyLairSettings("Fort", 1);
            LairsSettings.Add(tlls);

            // Загружаем конфигурацию логов
            XmlNode ne = n.SelectSingleNode("TypeConstructions");
            foreach (XmlNode l in ne.SelectNodes("TypeConstruction"))
            {
                tlls = new TypeLobbyLairSettings(l);

                // Проверяем, что тип логова не повторяется
                foreach (TypeLobbyLairSettings ls in LairsSettings)
                {
                    Debug.Assert(tlls.NameTypeLair != "Fort");
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

        internal TypeLobby TypeLobby { get; }// Тип лобби
        internal int Number { get; }// Номер слоя
        internal DescriptorLocation Location { get; }
        internal bool Ownership { get; set; }// Локация является владением короля
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
}