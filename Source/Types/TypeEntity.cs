using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый тип для всех сущностей - сооружений, существ, умений, предметов и т.д.
    internal abstract class TypeEntity
    {
        private string name;
        private int imageIndex;

        public TypeEntity(XmlNode n)
        {
            Config = FormMain.Config;

            ID = GetStringNotNull(n, "ID");
            name = GetStringNotNull(n, "Name");
            Description = GetDescription(n, "Description");
            // Для удобства людей, нумерация иконов в конфигурации идет с 1, а не с 0.
            // Так как нумерация иконок в Gui48 идет с 0, добавляем сдвиг номера при добавлении после объектов128
            imageIndex = GetIntegerNotNull(n, "ImageIndex") + (GetIntegerNotNull(n, "ImageIndex") != FormMain.IMAGE_INDEX_CURRENT_AVATAR ? -1 : 0) + ShiftImageIndex();

            CheckData();
        }

        public TypeEntity(string id, string name, string description, int imageIndex)
        {
            Config = FormMain.Config;

            ID = id;
            this.name = name;
            Description = description;
            this.imageIndex = imageIndex;

            CheckData();
        }

        internal string ID { get; }// Уникальный (в пределах списка) код типа объекта
        internal string Name { get => name; set { name = value; CheckData(); } }// Наименование типа объекта
        internal string Description { get; set; }// Описание типа объекта
        internal int ImageIndex { get => imageIndex; set { imageIndex = value; CheckData(); } }// Код иконки типа объекта
        protected Config Config { get; }

        internal virtual void TuneDeferredLinks() { }
        
        protected virtual int ShiftImageIndex() => 0;

        protected virtual void CheckData()
        {
            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            //Debug.Assert(Name.Length <= Config.MaxLengthObjectName);
            //Debug.Assert(Description.Length > 0);
            Debug.Assert((ImageIndex >= 0) || (ImageIndex == FormMain.IMAGE_INDEX_CURRENT_AVATAR));
        }
    }
}
