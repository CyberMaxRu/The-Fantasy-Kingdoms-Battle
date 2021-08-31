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
    // Класс сущности для конкретного лобби. Общий для игроков, зданий, предметов, абилок, существ, исследованийи т.д.
    internal abstract class Entity
    {
        public Entity(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = XmlUtils.GetDescription(n, "Description");
            ImageIndex = XmlUtils.GetInteger(n, "ImageIndex") + FormMain.Config.ImageIndexFirstItems - 1;
            Cost = XmlUtils.GetInteger(n, "Cost");
        }

        public Entity()
        {
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; set; }
        internal int ImageIndex { get; }
        internal int Cost { get; }

        // Методы для потомков для поддержки работы с ячейкой
        internal abstract int GetImageIndex();
        internal abstract bool GetNormalImage();
        internal abstract int GetLevel();
        internal abstract int GetQuantity();
        internal abstract string GetCost();
        internal abstract void PrepareHint();

        internal virtual void Click(VCCell pe)
        {
        }

        internal virtual void CustomDraw(Graphics g, int x, int y, bool drawState)
        {

        }
    }
}
