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
            name = GetStringNotNull(n, "Name");

            CheckData();
        }

        public DescriptorWithID(string id, string name) : base()
        {
            ID = id;
            this.name = name;

            CheckData();
        }

        internal string ID { get; }// Уникальный (в пределах списка) код типа объекта
        internal string Name { get => name; set { name = value; CheckData(); } }// Наименование типа объекта
        
        private void CheckData()
        {
            Debug.Assert(ID.Length > 0);
            Debug.Assert(Name.Length > 0);
            //Debug.Assert(Name.Length <= Config.MaxLengthObjectName);
        }
    }
}
