using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    public sealed class XmlUtils
    {
        internal static void LoadRequirements(List<Requirement> list, XmlNode n)
        {
            XmlNode nr = n.SelectSingleNode("Requirements");

            if (nr != null)
            {
                foreach (XmlNode r in nr.SelectNodes("Requirement"))
                    list.Add(new Requirement(r));
            }
        }

        internal static int GetInteger(XmlNode n, string name)
        {
            XmlNode nn = n.SelectSingleNode(name);
            //Debug.Assert(nn != null, $"Узел {name} отсутствует.");

            return nn is null ? 0 : string.IsNullOrEmpty(nn.InnerText) ? 0 : Convert.ToInt32(nn.InnerText);
        }

        internal static int GetIntegerNotNull(XmlNode n, string name)
        {
            XmlNode nn = n.SelectSingleNode(name);
            Debug.Assert(nn != null, $"Поле {name} отсутствует.");
            Debug.Assert(nn.InnerText != null, $"У поля {name} нет значения.");

            return Convert.ToInt32(nn.InnerText);
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

            return n is null ? 0 : Convert.ToDouble(nn.InnerText.Replace(".", ","));
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
