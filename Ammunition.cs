using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
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
            if (n.SelectSingleNode("VelocityMissile") != null)
                VelocityMissile = Convert.ToDouble(n.SelectSingleNode("VelocityMissile").InnerText.Replace(".", ","));
            DamageMelee = XmlUtils.GetInteger(n.SelectSingleNode("DamageMelee"));
            DamageRange = XmlUtils.GetInteger(n.SelectSingleNode("DamageArcher"));
            DamageMagic = XmlUtils.GetInteger(n.SelectSingleNode("DamageMagic"));

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
        internal TypeHero ClassHero { get; private set; }
        internal int TimeHit { get; }
        internal double VelocityMissile { get; }
        internal int DamageMelee { get; }
        internal int DamageRange { get; }
        internal int DamageMagic { get; }

        internal void TuneDeferredLinks()
        {
            ClassHero = FormMain.Config.FindTypeHero(nameClassHero);
            nameClassHero = null;
        }


        // Реализация интерфейса
        BitmapList ICell.BitmapList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => GroupWeapon.ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Level() => 0;
        int ICell.Quantity() => 0;
        string ICell.Cost() => null;

        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(GroupWeapon.Name, "", GroupWeapon.Description);
            Program.formMain.formHint.AddStep7Weapon(this);
        }

        void ICell.Click(VCCell pe)
        {

        }

        void ICell.CustomDraw(Graphics g, int x, int y, bool drawState) { }
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

            Description += (Description.Length > 0 ? Environment.NewLine : "") + "- Используется:";

            foreach (Weapon w in Weapons)
            {
                Description += Environment.NewLine + "  - " + w.ClassHero.Name;
            }
        }

        protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost()
        {
            return "Оружие";
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
            DefenseMelee = XmlUtils.GetInteger(n.SelectSingleNode("DefenseMelee"));
            DefenseArcher = XmlUtils.GetInteger(n.SelectSingleNode("DefenseArcher"));
            DefenseMagic = XmlUtils.GetInteger(n.SelectSingleNode("DefenseMagic"));

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
        internal TypeHero ClassHero { get; private set; }
        internal int DefenseMelee { get; }
        internal int DefenseArcher { get; }
        internal int DefenseMagic { get; }

        internal void TuneDeferredLinks()
        {
            ClassHero = FormMain.Config.FindTypeHero(nameClassHero);
            nameClassHero = null;
        }

        // Реализация интерфейса
        BitmapList ICell.BitmapList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => GroupArmour.ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Level() => 0;
        int ICell.Quantity() => 0;
        string ICell.Cost() => null;
        void ICell.PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(GroupArmour.Name, "", GroupArmour.Description);
            Program.formMain.formHint.AddStep8Armour(this);
        }

        void ICell.Click(VCCell pe)
        {

        }
        void ICell.CustomDraw(Graphics g, int x, int y, bool drawState) { }
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

        protected override int GetLevel() => 0;
        protected override int GetQuantity() => 0;
        protected override string GetCost()
        {
            return "Доспех";
        }
    }
}
