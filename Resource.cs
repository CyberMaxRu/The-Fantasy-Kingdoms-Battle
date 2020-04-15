using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace Fantasy_King_s_Battle
{
    internal sealed class Resource
    {
        public Resource(XmlNode n)
        {
            ID = n.SelectSingleNode("ID").InnerText;
            Name = n.SelectSingleNode("Name").InnerText;
            ImageIndex = n.SelectSingleNode("ImageIndex") != null ? Convert.ToInt32(n.SelectSingleNode("ImageIndex").InnerText) : -1;
            Position = FormMain.Config.Resources.Count;

            // Проверяем, что таких же ID и наименования нет
            foreach (Resource r in FormMain.Config.Resources)
            {
                if (r.ID == ID)
                {
                    throw new Exception("В конфигурации ресурсов повторяется ID = " + ID);
                }

                if (r.Name == Name)
                {
                    throw new Exception("В конфигурации ресурсов повторяется Name = " + Name);
                }

                if (r.ImageIndex == ImageIndex)
                {
                    throw new Exception("В конфигурации ресурсов повторяется ImageIndex = " + ImageIndex.ToString());
                }
            }
        }

        internal string ID { get; }
        internal string Name { get; }
        internal int ImageIndex { get;}
        internal int Position { get; }
        internal ToolStripStatusLabel StatusLabel { get; set; }
    }
}
