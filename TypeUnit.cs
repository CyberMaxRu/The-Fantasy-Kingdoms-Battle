using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    internal enum CategoryUnit { Warrior, Ranger, Mage }
    internal enum TypeMove { Ground, Fly };

    // Класс типа юнита
    internal sealed class TypeUnit
    {
        public TypeUnit(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Fraction = FormMain.Config.FindFraction(n.SelectSingleNode("Fraction").InnerText);
            TypeMove = (TypeMove)Enum.Parse(typeof(TypeMove), n.SelectSingleNode("TypeMove").InnerText);

            // Стоимость покупки
            XmlNode l;
            string parName;
            int value;
            l = n.SelectSingleNode("Parameters");
            for (int k = 0; k < l.ChildNodes.Count; k++)
            {
                parName = l.ChildNodes[k].LocalName;
                value = Convert.ToInt32(l.ChildNodes[k].InnerText);
                if (value < 0)
                    throw new Exception("У типа юнита " + ID + " параметр " + parName.ToString() + " меньше ноля (" + value.ToString() + ").");

                switch (parName)
                {
                    case "DamageMin":
                        DamageMin = value;
                        break;
                    case "DamageMax":
                        DamageMax = value;
                        break;
                    case "Health":
                        Health = value;
                        break;
                    case "Morale":
                        Morale = value;
                        break;
                    default:
                        throw new Exception("Неизвестный параметр: " + parName);
                }
            }

        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get; }
        internal Fraction Fraction { get; }
        internal TypeMove TypeMove { get; }
        internal int DamageMin { get; }
        internal int DamageMax { get; }
        internal int Health { get; }
        internal int Morale { get; }
    }
}
