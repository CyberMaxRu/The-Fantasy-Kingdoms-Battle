using System;
using System.Xml;
using System.Diagnostics;
using static Fantasy_Kingdoms_Battle.XmlUtils;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class DescriptorWithID : Descriptor
    {
        private string name;

        public DescriptorWithID(XmlNode n) : base()
        {
            ID = GetStringNotNull(n, "ID");
            name = GetName(n);

            XmlNode nc = n.SelectSingleNode("Creating");
            if (nc != null)
                Creating = new DescriptorComponentCreating(this, nc);

            CheckData();
        }

        public DescriptorWithID(string id, string name) : base()
        {
            ID = id;
            this.name = name;

            CheckData();
        }

        internal string ID { get; }// Уникальный код сущности
        internal string Name { get => name; set { name = value; CheckData(); } }// Наименование сущности
        internal DescriptorComponentCreating Creating { get; }

        protected virtual string GetName(XmlNode n) => GetStringNotNull(n, "Name");

        private void CheckData()
        {
            Debug.Assert(ID.Length > 0);
            Debug.Assert(ID.Length <= Config.MaxLengthIDEntity);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Name.Length <= Config.MaxLengthNameEntity);
        }

        internal int GetIntegerFromXmlNode(XmlNode n, string name, int minValue, int maxValue)
        {
            XmlNode nn = n.SelectSingleNode(name);

            if (minValue > 0)
            {
                Assert(nn != null, $"{ID}: поле {name} отсутствует.");
                Assert(nn.InnerText != null, $"{ID}: поле {name} не имеет значения.");
            }
            string text = nn?.InnerText != null ? nn.InnerText : "0";

            int value = Convert.ToInt32(text);
            Assert(value >= minValue, $"{ID}: значение ({value}) поля {name} меньше допустимого ({minValue}).");
            Assert(value <= maxValue, $"{ID}: значение ({value}) поля {name} больше допустимого ({maxValue}).");

            return value;
        }

        internal string GetStringFromXmlNode(XmlNode n, string name, bool required = true)
        {
            XmlNode nn = n.SelectSingleNode(name);
            if (required)
                Assert(nn != null, $"{ID}: поле {name} отсутствует.");

            string s = nn?.InnerText;
            if (required)
                Assert(s.Length > 0, $"{ID}: поле {name} не имеет значения.");

            return s;
        }

        internal override void TuneLinks()
        {
            base.TuneLinks();

            if (Creating != null)
                Creating.TuneLinks();
        }
    }
}
