using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый тип для всех объектов - зданий, логов, существ
    internal abstract class TypeObject
    {
        public TypeObject(XmlNode n)
        {
            ID = XmlUtils.GetStringNotNull(n.SelectSingleNode("ID"));
            Name = XmlUtils.GetStringNotNull(n.SelectSingleNode("Name"));
            Description = XmlUtils.GetDescription(n.SelectSingleNode("Description"));
            ImageIndex = XmlUtils.GetInteger(n.SelectSingleNode("ImageIndex"));
            if (ImageIndex > 0)
                ImageIndex--;

            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Description.Length > 0);
            //Debug.Assert(ImageIndex >= 0);
        }

        public TypeObject(string id, string name, string description, int imageIndex)
        {
            ID = id;
            Name = name;
            Description = description;
            ImageIndex = imageIndex;

            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Description.Length > 0);
        }

        internal string ID { get; }// Уникальный (в пределах списка) код типа объекта
        internal string Name { get; private set; }// Наименование типа объекта
        internal string Description { get; }// Описание типа объекта
        internal int ImageIndex { get; private set; }// Код иконки типа объекта

        internal virtual void TuneDeferredLinks()
        {

        }

        internal void ChangeImageIndex(int newImageIndex)
        {
            ImageIndex = newImageIndex;
        }

        internal void SetName(string name)
        {
            Debug.Assert(name.Length > 0);
            Debug.Assert(name.Length <= FormMain.MAX_LENGTH_USERNAME);

            Name = name;
        }
    }
}
