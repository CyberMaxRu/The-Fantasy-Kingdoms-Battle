using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Fantasy_King_s_Battle
{
    // Класс предмета
    internal sealed class Item : Entity
    {
        public Item(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = n.SelectSingleNode("Description").InnerText.Replace("/", Environment.NewLine);
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            TypeItem = FormMain.Config.FindTypeItem(n.SelectSingleNode("TypeItem").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);            
            TimeHit = n.SelectSingleNode("TimeHit") == null ? 0 : Convert.ToInt32(n.SelectSingleNode("TimeHit").InnerText);

            DamageMelee = n.SelectSingleNode("DamageMelee") != null ? Convert.ToInt32(n.SelectSingleNode("DamageMelee").InnerText) : 0;
            DamageMissile = n.SelectSingleNode("DamageMissile") != null ? Convert.ToInt32(n.SelectSingleNode("DamageMissile").InnerText) : 0;
            DamageMagic = n.SelectSingleNode("DamageMagic") != null ? Convert.ToInt32(n.SelectSingleNode("DamageMagic").InnerText) : 0;
            DefenseMelee = n.SelectSingleNode("DefenseMelee") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseMelee").InnerText) : 0;
            DefenseMissile = n.SelectSingleNode("DefenseMissile") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseMissile").InnerText) : 0;
            DefenseMagic = n.SelectSingleNode("DefenseMagic") != null ? Convert.ToInt32(n.SelectSingleNode("DefenseMagic").InnerText) : 0;

            /*switch (TypeAttack)
            {
                case TypeAttack.None:
                    Debug.Assert((DamageMelee == 0) && (DamageMissile == 0) && (DamageMagic == 0));
                    Debug.Assert(TimeHit == 0);

                    break;
                case TypeAttack.Melee:
                    Debug.Assert(DamageMelee > 0);
                    Debug.Assert(TimeHit > 0);
                    Debug.Assert(TimeHit * 10 % Config.STEP_IN_MSEC == 0);

                    break;
                case TypeAttack.Missile:
                    Debug.Assert((DamageMissile > 0) || (DamageMagic > 0));
                    Debug.Assert(TimeHit > 0);
                    Debug.Assert(TimeHit * 10 % Config.STEP_IN_MSEC == 0);

                    break;
                default:
                    throw new Exception("Неизвестный тип атаки.");
            }*/

            Position = FormMain.Config.Items.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Item i in FormMain.Config.Items)
            {
                if (i.ID == ID)
                    throw new Exception("В конфигурации предметов повторяется ID = " + ID);

                if (i.Name == Name)
                    throw new Exception("В конфигурации предметов повторяется Name = " + Name);

                if (i.ImageIndex == ImageIndex)
                    throw new Exception("В конфигурации предметов повторяется ImageIndex = " + ImageIndex.ToString());
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
        internal TypeItem TypeItem { get; }
        internal int TimeHit { get; }
        internal int Position { get; }
        internal int Cost { get; }
        internal int DamageMelee { get; }
        internal int DamageMissile { get; }
        internal int DamageMagic { get; }
        internal int DefenseMelee { get; }
        internal int DefenseMissile { get; }
        internal int DefenseMagic { get; }
    }
}