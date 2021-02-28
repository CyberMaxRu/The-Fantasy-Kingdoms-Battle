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
    // Класс сущности. Общий для предметов, абилок, героев и т.д.
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

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; set; }
        internal int ImageIndex { get; }
        internal int Cost { get; }

        // Методы для потомков для возврата значений в интерфейс
        protected abstract int GetValue();
        protected virtual void DoPrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Name, "", Description);
        }

        // Реализация интерфейса
        VCCell ICell.Panel { get; set; }
        BitmapList ICell.BitmapList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => ImageIndex;
        bool ICell.NormalImage() => true;
        int ICell.Value() => GetValue();

        void ICell.PrepareHint() 
        {
            DoPrepareHint();
        }

        void ICell.Click(VCCell pe)
        {

        }

        void ICell.CustomDraw(Graphics g, int x, int y) { }
    }
}
