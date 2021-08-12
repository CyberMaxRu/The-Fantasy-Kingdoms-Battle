using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс сущности. Общий для игроков, зданий, предметов, абилок, существ, исследованийи т.д.
    internal abstract class Entity : ICell
    {
        public Entity(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = XmlUtils.GetDescription(n.SelectSingleNode("Description"));
            ImageIndex = XmlUtils.GetInteger(n.SelectSingleNode("ImageIndex"));
            Cost = XmlUtils.GetInteger(n.SelectSingleNode("Cost"));
        }

        public Entity()
        {
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; set; }
        internal int ImageIndex { get; }
        internal int Cost { get; }

        // Методы для потомков для возврата значений в интерфейс
        protected abstract int GetLevel();
        protected abstract int GetQuantity();
        protected abstract string GetCost();
        protected virtual void DoPrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Name, "", Description);
        }

        // Реализация интерфейса
        BitmapList ICell.BitmapList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Level() => GetLevel();
        int ICell.Quantity() => GetQuantity();
        string ICell.Cost() => GetCost();

        void ICell.PrepareHint() 
        {
            DoPrepareHint();
        }

        void ICell.Click(VCCell pe)
        {

        }

        void ICell.CustomDraw(Graphics g, int x, int y, bool drawState) { }
    }
}
