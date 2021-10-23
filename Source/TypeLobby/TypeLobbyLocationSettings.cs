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
    internal sealed class TypeLobbyLocationSettings : DescriptorEntity
    {
        public TypeLobbyLocationSettings(TypeLobby typeLobby, XmlNode n, int quantitySlotLairs) : base(n)
        {
            TypeLobby = typeLobby;

            Number = XmlUtils.GetInteger(n, "Number");
            NameTexture = XmlUtils.GetStringNotNull(n, "NameTexture");
            Ownership = XmlUtils.GetBooleanNotNull(n, "Ownership");
            CostScout = XmlUtils.GetInteger(n, "CostScout");
            CostAttack = XmlUtils.GetInteger(n, "CostAttack");
            CostDefense = XmlUtils.GetInteger(n, "CostDefense");
            DefaultElement = FormMain.Config.FindElementLandscape(XmlUtils.GetStringNotNull(n, "DefaultElement"));

            Debug.Assert(CostScout > 0);
            Debug.Assert(CostScout <= 10_000);
            Debug.Assert(CostAttack > 0);
            Debug.Assert(CostAttack <= 10_000);
            Debug.Assert(CostDefense > 0);
            Debug.Assert(CostDefense <= 10_000);

            TypeLobbyLairSettings tlls;
            TypeLobbyLocationElement tlle;
            // Загружаем форт для каждой локации
            tlls = new TypeLobbyLairSettings("Fort", 1);
            LairsSettings.Add(tlls);

            // Загружаем конфигурацию ландшафтов
            XmlNode ne1 = n.SelectSingleNode("Elements");
            if (ne1 != null)
            {
                foreach (XmlNode l in ne1.SelectNodes("Element"))
                {
                    tlle = new TypeLobbyLocationElement(l);

                    // Проверяем, что тип ландшафта не повторяется
                    foreach (TypeLobbyLocationElement le in ElementSettings)
                    {
                        if (tlle.NameElement == le.NameElement)
                            throw new Exception($"Ландшафт {tlle.NameElement} повторяется в списке элементов ландшафта локации {ID}.");
                    }

                    ElementSettings.Add(tlle);
                }
            }

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

            foreach (TypeLobbyLocationElement le in ElementSettings)
            {
                minQuantity += le.MinQuantity;
                maxQuantity += le.MaxQuantity;
            }

            // Если количество сооружений меньше количества слотов, добиваем их пустыми местами
            if (maxQuantity < quantitySlotLairs)
            {
                // Если надо добавлять пустые места, то не должно быть в настройках пустого места. Если есть - значит, неправильно настроено количество
                foreach (TypeLobbyLocationElement le1 in ElementSettings)
                    Debug.Assert(le1.NameElement != DefaultElement.ID);

                TypeLobbyLocationElement le = new TypeLobbyLocationElement(DefaultElement.ID, quantitySlotLairs - maxQuantity);
                ElementSettings.Add(le);
                minQuantity += le.MinQuantity;
                maxQuantity += le.MaxQuantity;
            }

            Debug.Assert(minQuantity <= quantitySlotLairs);
            Debug.Assert(maxQuantity >= quantitySlotLairs);

        }

        internal TypeLobby TypeLobby { get; }// Тип лобби
        internal int Number { get; }// Номер слоя
        internal string NameTexture { get; }
        internal Bitmap BackgroundImage { get; }// Картинка для фона
        internal bool Ownership { get; set; }// Локация является владением короля
        internal int CostScout { get; }// Стоимость разведки
        internal int CostAttack { get; }// Стоимость атаки
        internal int CostDefense { get; }// Стоимость защиты
        internal DescriptorElementLandscape DefaultElement { get; }
        internal List<TypeLobbyLocationElement> ElementSettings { get; } = new List<TypeLobbyLocationElement>();// Настройки ландшафта
        internal List<TypeLobbyLairSettings> LairsSettings { get; } = new List<TypeLobbyLairSettings>();// Настройки типов логов для слоя

        internal override void TuneDeferredLinks()
        {
            base.TuneDeferredLinks();

            foreach (TypeLobbyLocationElement le in ElementSettings)
            {
                le.TuneDeferredLinks();
            }

            foreach (TypeLobbyLairSettings ls in LairsSettings)
            {
                ls.TuneDeferredLinks();
            }
        }
    }    
}
