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
    // Класс описателя локации
    internal sealed class DescriptorLocation : DescriptorEntity
    {
        public DescriptorLocation(XmlNode n) : base(n)
        { 
            NameTexture = XmlUtils.GetStringNotNull(n, "NameTexture");

            // Загружаем конфигурацию логов
/*            XmlNode ne = n.SelectSingleNode("TypeConstructions");
            TypeLobbyLairSettings tlls;
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
                foreach (TypeLobbyLairSettings ls in LairsSettings)
                    Debug.Assert(ls.NameTypeLair != FormMain.Config.IDEmptyPlace);

                TypeLobbyLairSettings s = new TypeLobbyLairSettings(FormMain.Config.IDEmptyPlace, quantitySlotLairs - maxQuantity);
                LairsSettings.Add(s);
                minQuantity += s.MinQuantity;
                maxQuantity += s.MaxQuantity;
            }

            Debug.Assert(minQuantity <= quantitySlotLairs);
            Debug.Assert(maxQuantity >= quantitySlotLairs);*/
        }

        internal string NameTexture { get; }// Текстура фона
        internal Bitmap BackgroundImage { get; }// Картинка для фона
    }
}