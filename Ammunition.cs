using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс оружия
    internal sealed class Weapon : ICell
    {
        private string nameClassHero;

        public Weapon(GroupWeapon gw, XmlNode n)
        {
            GroupWeapon = gw;

            ID = n.SelectSingleNode("ID").InnerText;
            nameClassHero = n.SelectSingleNode("Hero").InnerText;
            TimeHit = Convert.ToInt32(n.SelectSingleNode("TimeHit").InnerText);
            DamageMelee = Utils.GetParamFromXml(n.SelectSingleNode("DamageMelee"));
            DamageArcher = Utils.GetParamFromXml(n.SelectSingleNode("DamageArcher"));
            DamageMagic = Utils.GetParamFromXml(n.SelectSingleNode("DamageMagic"));

            Debug.Assert(ID.Length > 0);
            Debug.Assert(nameClassHero.Length > 0);
            Debug.Assert(TimeHit > 0);

            // Проверяем, что такого же оружия нет
            foreach (GroupWeapon g in FormMain.Config.GroupWeapons)
                foreach (Weapon w in g.Weapons)
                    if (w.ID == ID)
                        throw new Exception("Оружие ID = " + ID + " уже существует.");
        }

        internal string ID { get; }
        internal GroupWeapon GroupWeapon { get; }
        internal Hero ClassHero { get; private set; }
        internal int TimeHit { get; }
        internal int DamageMelee { get; }
        internal int DamageArcher { get; }
        internal int DamageMagic { get; }

        internal void TuneDeferredLinks()
        {
            ClassHero = FormMain.Config.FindHero(nameClassHero);
            nameClassHero = null;

            switch (ClassHero.KindHero.TypeAttack)
            {
                case TypeAttack.Melee:
                    Debug.Assert(DamageArcher == 0);
                    Debug.Assert(DamageMagic == 0);
                    break;
                case TypeAttack.Archer:
                    Debug.Assert(DamageMelee == 0);
                    Debug.Assert(DamageMagic == 0);
                    break;
                case TypeAttack.Magic:
                    Debug.Assert(DamageMelee == 0);
                    Debug.Assert(DamageArcher == 0);
                    break;
                default:
                    throw new Exception("Неизвестный тип атаки.");
            }
        }

        // Реализация интерфейса
        PanelEntity ICell.Panel { get; set; }
        ImageList ICell.ImageList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => GroupWeapon.ImageIndex;
        int ICell.Value() => 0;

        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(GroupWeapon.Name, "", GroupWeapon.Description);
            Program.formMain.formHint.AddStep7Weapon(this);
        }

        void ICell.Click()
        {

        }
    }

    // Класс группы оружий

    internal sealed class GroupWeapon : Entity
    {
        public GroupWeapon(XmlNode n) : base(n)
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

        internal List<Weapon> Weapons { get; } = new List<Weapon>();

        internal void TuneDeferredLinks()
        {
            foreach (Weapon w in Weapons)
                w.TuneDeferredLinks();

            Description += (Description.Length > 0 ? Environment.NewLine : "") + "Используется:";

            foreach (Weapon w in Weapons)
            {
                Description += Environment.NewLine + "  - " + w.ClassHero.Name;
            }
        }

        protected override int GetValue()
        {
            return 0;
        }
    }

    // Класс доспехов
    internal sealed class Armour : ICell
    {
        private string nameClassHero;
        public Armour(GroupArmour ga, XmlNode n)
        {
            GroupArmour = ga;

            ID = n.SelectSingleNode("ID").InnerText;
            nameClassHero = n.SelectSingleNode("Hero").InnerText;
            DefenseMelee = Utils.GetParamFromXml(n.SelectSingleNode("DefenseMelee"));
            DefenseArcher = Utils.GetParamFromXml(n.SelectSingleNode("DefenseArcher"));
            DefenseMagic = Utils.GetParamFromXml(n.SelectSingleNode("DefenseMagic"));

            Debug.Assert(ID.Length > 0);
            Debug.Assert(nameClassHero.Length > 0);

            // Проверяем, что такого же доспеха нет
            foreach (GroupArmour g in FormMain.Config.GroupArmours)
                foreach (Armour a in g.Armours)
                    if (a.ID == ID)
                        throw new Exception("Доспех ID = " + ID + " уже существует.");
        }

        internal string ID { get; }
        internal GroupArmour GroupArmour { get; }
        internal Hero ClassHero { get; private set; }
        internal int DefenseMelee { get; }
        internal int DefenseArcher { get; }
        internal int DefenseMagic { get; }

        internal void TuneDeferredLinks()
        {
            ClassHero = FormMain.Config.FindHero(nameClassHero);
            nameClassHero = null;
        }

        // Реализация интерфейса
        PanelEntity ICell.Panel { get; set; }
        ImageList ICell.ImageList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => GroupArmour.ImageIndex;
        int ICell.Value() => 0;

        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(GroupArmour.Name, "", GroupArmour.Description);
            Program.formMain.formHint.AddStep8Armour(this);
        }

        void ICell.Click()
        {

        }
    }

    // Класс группы доспехов
    internal sealed class GroupArmour : Entity
    {
        public GroupArmour(XmlNode n) : base(n)
        {
            // Проверяем, что таких ID, Name и ImageIndex нет
            foreach (GroupArmour ga in FormMain.Config.GroupArmours)
            {
                if (ga.ID == ID)
                    throw new Exception("Группа доспехов с ID = " + ga.ID + " уже существует.");

                if (ga.Name == Name)
                    throw new Exception("Группа доспехов с Name = " + ga.Name + " уже существует.");

                if (ga.ImageIndex == ImageIndex)
                    throw new Exception("Группа доспехов с ImageIndex = " + ga.ImageIndex.ToString() + " уже существует.");
            }

            XmlNode nl = n.SelectSingleNode("Armours");
            if (nl != null)
            {
                Armour a;

                foreach (XmlNode l in nl.SelectNodes("Armour"))
                {
                    a = new Armour(this, l);

                    Armours.Add(a);
                }
            }
        }

        internal List<Armour> Armours { get; } = new List<Armour>();

        internal void TuneDeferredLinks()
        {
            foreach (Armour a in Armours)
                a.TuneDeferredLinks();

            Description += (Description.Length > 0 ? Environment.NewLine : "") + "Используется:";

            foreach (Armour a in Armours)
            {
                Description += Environment.NewLine + "  - " + a.ClassHero.Name;
            }
        }

        protected override int GetValue()
        {
            return 0;
        }
    }
}
