using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    // Класс сущности. Общий для предметов, абилок, героев и т.д.
    internal abstract class Entity : ICell
    {
        public Entity(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            Description = Utils.AdaptDescription(n.SelectSingleNode("Description").InnerText);
            ImageIndex = Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText);
            Cost = Convert.ToInt32(n.SelectSingleNode("Cost").InnerText);
        }

        internal string ID { get; }
        internal string Name { get; }
        internal string Description { get; }
        internal int ImageIndex { get; }
        internal int Cost { get; }

        // Методы для потомков для возврата значений в интерфейс
        protected abstract int GetValue();
        protected virtual void DoPrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Name, "", Description);
        }

        // Реализация интерфейса
        PanelEntity ICell.Panel { get; set; }
        ImageList ICell.ImageList() => Program.formMain.ilItems;
        int ICell.ImageIndex() => ImageIndex;
        int ICell.Value() => GetValue();

        void ICell.PrepareHint() 
        {
            DoPrepareHint();
        }

        void ICell.Click()
        {

        }
    }
}
