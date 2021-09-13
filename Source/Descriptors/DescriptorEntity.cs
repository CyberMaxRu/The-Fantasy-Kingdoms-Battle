using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый описатель для всех сущностей - сооружений, существ, умений, предметов и т.д.
    internal abstract class DescriptorEntity : DescriptorWithID
    {
        private int imageIndex;

        public DescriptorEntity(XmlNode n) : base(n)
        {
            Description = GetDescription(n, "Description");            
            imageIndex = GetIntegerNotNull(n, "ImageIndex");
            if (imageIndex > 0)
                imageIndex += ShiftImageIndex() - 1;// Для удобства людей, нумерация иконов в конфигурации идет с 1, а не с 0.

            CheckData();
        }

        public DescriptorEntity(string id, string name, string description, int imageIndex) : base(id, name)
        {
            Description = description;
            this.imageIndex = imageIndex;

            CheckData();
        }

        internal string Description { get; set; }// Описание типа объекта
        internal int ImageIndex { get => imageIndex; set { imageIndex = value; CheckData(); } }// Код иконки типа объекта

        protected virtual int ShiftImageIndex() => 0;

        protected virtual void CheckData()
        {
            //Debug.Assert(Description.Length > 0);
            Debug.Assert((ImageIndex >= 0) || (ImageIndex == FormMain.IMAGE_INDEX_CURRENT_AVATAR));
        }
    }
}
