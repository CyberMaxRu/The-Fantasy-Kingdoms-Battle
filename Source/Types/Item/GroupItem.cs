using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    // Группа предметов
    internal sealed class GroupItem : Entity
    {
        public GroupItem(XmlNode n) : base(n)
        {
            // Проверяем, что таких ID, Name и ImageIndex нет
            foreach (GroupWeapon gw in FormMain.Config.GroupWeapons)
            {
                if (gw.ID == ID)
                    throw new Exception("Группа оружия с ID = " + gw.ID + " уже существует.");

                if (gw.Name == Name)
                    throw new Exception("Группа оружия с Name = " + gw.Name + " уже существует.");

                if (gw.ImageIndex == ImageIndex)
                    throw new Exception("Группа оружия с ImageIndex = " + gw.ImageIndex.ToString() + " уже существует.");
            }

            XmlNode nl = n.SelectSingleNode("Weapons");
            if (nl != null)
            {
                Weapon w;

                foreach (XmlNode l in nl.SelectNodes("Weapon"))
                {
                    w = new Weapon(this, l);

                    Weapons.Add(w);
                }
            }
        }

        internal string ShortName { get; }//
        internal List<Item> Items { get; } = new List<Item>();

        internal void TuneDeferredLinks()
        {
            Description += (Description.Length > 0 ? Environment.NewLine : "") + "- Используется:";

            foreach (Item i in Items)
            {
                Description += Environment.NewLine + "  - " + i.ClassHero.Name;
            }
        }

        protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost() => ShortName;
    }
}