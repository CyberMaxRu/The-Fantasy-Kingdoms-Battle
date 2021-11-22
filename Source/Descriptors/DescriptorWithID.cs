using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

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
                Creating = new DescriptorCreating(this, nc);

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
        internal DescriptorCreating Creating { get; }

        protected virtual string GetName(XmlNode n) => GetStringNotNull(n, "Name");

        private void CheckData()
        {
            Debug.Assert(ID.Length > 0);
            Debug.Assert(ID.Length <= Config.MaxLengthIDEntity);
            Debug.Assert(Name.Length > 0);
            Debug.Assert(Name.Length <= Config.MaxLengthNameEntity);
        }
    }
}
