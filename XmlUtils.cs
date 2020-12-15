using System;
using System.Collections.Generic;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    public sealed class XmlUtils
    {
        internal static void LoadRequirements(List<Requirement> list, XmlNode n)
        {
            XmlNode nr = n.SelectSingleNode("Requirements");

            if (nr != null)
            {
                foreach (XmlNode r in nr.SelectNodes("Requirement"))
                    list.Add(new Requirement(r.SelectSingleNode("Building").InnerText, GetParamFromXmlInteger(r.SelectSingleNode("Level"))));
            }
        }

        internal static int GetParamFromXmlInteger(XmlNode n)
        {
            return n != null ? Convert.ToInt32(n.InnerText) : 0;
        }

        internal static string GetParamFromXmlString(XmlNode n)
        {
            return n != null ? n.InnerText : "";
        }

        internal static bool GetParamFromXmlBool(XmlNode n, bool defValue)
        {
            return n != null ? Convert.ToBoolean(n.InnerText) : defValue;
        }

        public static Version GetVersionFromXml(XmlNode n, string name)
        {
            return new Version(n.SelectSingleNode(name).InnerText);
        }
    }
}
