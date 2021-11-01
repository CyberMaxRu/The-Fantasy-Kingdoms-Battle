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
        private string nameDefaultConstruction;

        public TypeLobbyLocationSettings(TypeLobby typeLobby, XmlNode n, int quantitySlotLairs) : base(n)
        {
            TypeLobby = typeLobby;

            nameTypeLandscape = XmlUtils.GetStringNotNull(n, "TypeLandscape");
            Number = XmlUtils.GetInteger(n, "Number");
            Ownership = XmlUtils.GetBooleanNotNull(n, "Ownership");
            CostScout = XmlUtils.GetInteger(n, "CostScout");
            CostAttack = XmlUtils.GetInteger(n, "CostAttack");
            CostDefense = XmlUtils.GetInteger(n, "CostDefense");
            nameDefaultConstruction = XmlUtils.GetStringNotNull(n, "DefaultConstruction");

            Debug.Assert(CostScout > 0);
            Debug.Assert(CostScout <= 10_000);
            Debug.Assert(CostAttack > 0);
            Debug.Assert(CostAttack <= 10_000);
            Debug.Assert(CostDefense > 0);
            Debug.Assert(CostDefense <= 10_000);

            TypeLobbyLairSettings tlls;

            // Загружаем конфигурацию логов
            XmlNode ne = n.SelectSingleNode("TypeConstructions");
            foreach (XmlNode l in ne.SelectNodes("TypeConstruction"))
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
                foreach (TypeLobbyLairSettings ls1 in LairsSettings)
                    Debug.Assert(ls1.NameTypeLair != nameDefaultConstruction);

                TypeLobbyLairSettings ls = new TypeLobbyLairSettings(nameDefaultConstruction, quantitySlotLairs - maxQuantity);
                LairsSettings.Add(ls);
                minQuantity += ls.MinQuantity;
                maxQuantity += ls.MaxQuantity;
            }

            Debug.Assert(minQuantity <= quantitySlotLairs);
            Debug.Assert(maxQuantity >= quantitySlotLairs);

        }

        internal TypeLobby TypeLobby { get; }// Тип лобби
        internal DescriptorTypeLandscape TypeLandscape { get; private set; }
        internal int Number { get; }// Номер слоя
        internal bool Ownership { get; set; }// Локация является владением короля
        internal int CostScout { get; }// Стоимость разведки
        internal int CostAttack { get; }// Стоимость атаки
        internal int CostDefense { get; }// Стоимость защиты
        internal DescriptorConstruction DefaultConstruction { get; private set; }
        internal List<TypeLobbyLairSettings> LairsSettings { get; } = new List<TypeLobbyLairSettings>();// Настройки типов логов для слоя

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            TypeLandscape = FormMain.Config.FindTypeLandscape(nameTypeLandscape);
            nameTypeLandscape = "";

            DefaultConstruction = FormMain.Config.FindConstruction(nameDefaultConstruction);
            nameDefaultConstruction = "";

            foreach (TypeLobbyLairSettings ls in LairsSettings)
            {
                ls.TuneDeferredLinks();
            }
        }
    }    
}
