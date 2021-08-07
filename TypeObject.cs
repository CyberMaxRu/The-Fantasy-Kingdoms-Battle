using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый тип для всех объектов - зданий, логов, существ
    internal abstract class TypeObject
    {
        public TypeObject(XmlNode n)
        {
            ID = GetStringNotNull(n.SelectSingleNode("ID"));
            Name = GetStringNotNull(n.SelectSingleNode("Name"));
            Description = GetDescription(n.SelectSingleNode("Description"));
            ImageIndex = GetIntegerNotNull(n.SelectSingleNode("ImageIndex"));
            if (ImageIndex != FormMain.IMAGE_INDEX_CURRENT_AVATAR)
                ImageIndex--;

            CheckData();
        }

        public TypeObject(string id, string name, string description, int imageIndex)
        {
            ID = id;
            Name = name;
            Description = description;
            ImageIndex = imageIndex;

            CheckData();
        }

        internal string ID { get; }// Уникальный (в пределах списка) код типа объекта
        internal string Name { get; private set; }// Наименование типа объекта
        internal string Description { get; }// Описание типа объекта
        internal int ImageIndex { get; private set; }// Код иконки типа объекта

        internal virtual void TuneDeferredLinks()
        {
        }

        internal void ChangeImageIndex(int imageIndex)
        {
            ImageIndex = imageIndex;

            CheckData();
        }

        internal void SetName(string name)
        {
            Name = name;

            CheckData();
        }

        private void CheckData()
        {
            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Name.Length <= FormMain.Config.MaxLengthObjectName);
            Debug.Assert(Description.Length > 0);
            Debug.Assert((ImageIndex >= 0) || (ImageIndex == FormMain.IMAGE_INDEX_CURRENT_AVATAR));
        }
    }
}
