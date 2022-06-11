using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    public sealed class XmlUtils
    {
        internal static int GetInteger(XmlNode n, string name)
        {
            XmlNode nn = n.SelectSingleNode(name);
            //Debug.Assert(nn != null, $"Узел {name} отсутствует.");

            return nn is null ? 0 : string.IsNullOrEmpty(nn.InnerText) ? 0 : Convert.ToInt32(nn.InnerText);
        }

        // Этот метод устаревший, удалить
        internal static int GetIntegerNotNull(XmlNode n, string name, string forEntity = "")
        {
            XmlNode nn = n.SelectSingleNode(name);
            Debug.Assert(nn != null, $"Поле {(forEntity.Length > 0 ? forEntity + ".": "")}{name} отсутствует.");
            Debug.Assert(nn.InnerText != null, $"У поля {(forEntity.Length > 0 ? forEntity : " ")}{name} нет значения.");

            return Convert.ToInt32(nn.InnerText);
        }

        internal static int GetIntegerNotNull(XmlNode n, string name, string idEntity, int minValue, int maxValue)
        {
            Assert(name.Length > 0);
            Assert(idEntity.Length > 0);
            Assert(maxValue >= minValue, $"MinValue: {minValue}, MaxValue: {maxValue}");

            XmlNode nn = n.SelectSingleNode(name);
            Assert(nn != null, $"{idEntity}.{name}: поле отсутствует.");
            Assert(nn.InnerText != null, $"{idEntity}.{name}: нет значения.");

            int val;
            if (!int.TryParse(nn.InnerText, out val))
                DoException($"{idEntity}.{name}: содержит не int ({nn.InnerText})");

            Assert(val >= minValue, $"{idEntity}.{name}: значение {val} меньше минимального ({minValue})");
            Assert(val <= maxValue, $"{idEntity}.{name}: значение {val} больше максимального ({maxValue})");

            return val;
        }

        internal static int GetPercent(XmlNode n, string name, string forEntity = "")
        {
            XmlNode nn = n.SelectSingleNode(name);

            return nn is null ? 0 : string.IsNullOrEmpty(nn.InnerText) ? 0 : Convert.ToInt32(Convert.ToDouble(nn.InnerText.Replace('.', ',')) * 10);
        }

        internal static int GetPercentNotNull(XmlNode n, string name, string forEntity = "")
        {
            XmlNode nn = n.SelectSingleNode(name);
            Debug.Assert(nn != null, $"Поле {(forEntity.Length > 0 ? forEntity + "." : "")}{name} отсутствует.");
            Debug.Assert(nn.InnerText != null, $"У поля {(forEntity.Length > 0 ? forEntity : " ")}{name} нет значения.");

            return Convert.ToInt32(Convert.ToDouble(nn.InnerText.Replace('.', ',')) * 10);
        }

        internal static string GetString(XmlNode n, string name)
        {
            string s = n.SelectSingleNode(name)?.InnerText;
            return s is null ? "" : s;
        }

        internal static string GetStringNotNull(XmlNode n, string name)
        {
            XmlNode nn = n.SelectSingleNode(name);
            Debug.Assert(nn != null, $"Поле {name} отсутствует.");
            Debug.Assert(nn.InnerText.Length > 0, $"Поле {name} пусто.");

            return nn.InnerText;
        }

        internal static string GetDescription(XmlNode n, string name)
        {
            return GetString(n, name).Replace("//", Environment.NewLine);
            //return GetStringNotNull(n, name).Replace("//", Environment.NewLine);
        }

        internal static bool GetBoolean(XmlNode n, string name, bool defValue)
        {
            XmlNode nn = n.SelectSingleNode(name);
            //Debug.Assert(nn != null, $"Поле {name} отсутствует.");

            return nn is null ? defValue : Convert.ToBoolean(nn.InnerText);

        }

        internal static bool GetBooleanNotNull(XmlNode n, string name)
        {
            XmlNode nn = n.SelectSingleNode(name);
            Debug.Assert(nn != null, $"Поле {name} отсутствует.");
            Debug.Assert(nn.InnerText != null, $"У поля {name} нет значения.");

            return Convert.ToBoolean(nn.InnerText);
        }

        internal static double GetDouble(XmlNode n, string name)
        {
            XmlNode nn = n.SelectSingleNode(name);
            Debug.Assert(nn != null, $"Поле {name} отсутствует.");

            return n is null ? 0 : Convert.ToDouble(nn.InnerText.Replace(".", System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
        }

        internal static Point GetPoint(XmlNode n, string name)
        {
            string pos = GetStringNotNull(n, name);
            Debug.Assert(pos.Length > 0);

            string[] parts = pos.Split(',');
            Debug.Assert(parts.Length == 2);

            if (!int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
                throw new Exception($"Не могу распарсить координаты: {pos}.");

            Debug.Assert(x > 0);
            Debug.Assert(y > 0);

            return new Point(x - 1, y - 1);
        }

        internal static void XmlFieldNotExist(XmlNode n, string name)
        {
            Debug.Assert(n.SelectSingleNode(name) is null, $"Поле {name} должно отсутствовать.");
        }

        public static Version GetVersionFromXml(XmlNode n, string name)
        {
            return new Version(n.SelectSingleNode(name).InnerText);
        }
    }
}
