using System;
using System.Collections.Generic;
using System.Xml;

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
                    list.Add(new Requirement(r.SelectSingleNode("TypeConstruction").InnerText, GetInteger(r.SelectSingleNode("Level"))));
            }
        }

        internal static int GetInteger(XmlNode n)
        {
            return n != null ? Convert.ToInt32(n.InnerText) : 0;
        }

        internal static int GetIntegerNotNull(XmlNode n)
        {
            return Convert.ToInt32(n.InnerText);
        }

        internal static string GetString(XmlNode n)
        {
            return n != null ? n.InnerText : "";
        }

        internal static string GetStringNotNull(XmlNode n)
        {
            if (n == null)
                throw new Exception("Параметр пуст");

            return n.InnerText;
        }

        internal static string GetDescription(XmlNode n)
        {
            return GetStringNotNull(n).Replace("/", Environment.NewLine);
        }

        internal static bool GetBool(XmlNode n, bool defValue)
        {
            return n != null ? Convert.ToBoolean(n.InnerText) : defValue;
        }

        internal static bool GetBoolNotNull(XmlNode n)
        {
            return Convert.ToBoolean(n.InnerText);
        }

        internal static double GetDouble(XmlNode n)
        {
            return n != null ? Convert.ToDouble(n.InnerText.Replace(".", ",")) : 0;
        }

        public static Version GetVersionFromXml(XmlNode n, string name)
        {
            return new Version(n.SelectSingleNode(name).InnerText);
        }
    }
}
