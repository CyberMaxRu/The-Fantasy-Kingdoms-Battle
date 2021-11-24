using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ListSmallEntity : List<DescriptorSmallEntity>
    {
        private List<string> nameEntity;

        public ListSmallEntity(DescriptorConstruction construction, XmlNode n)
        {
            Construction = construction;

            if (n != null)
            {
                nameEntity = new List<string>();

                foreach (XmlNode l in n.SelectNodes("Entity"))
                {
                    string nameEntity = l.InnerText;
                    Debug.Assert(nameEntity.Length > 0);

                    // Проверяем, что такой перк не повторяется
                    foreach (string e2 in this.nameEntity)
                    {
                        Debug.Assert(e2 != nameEntity);
                    }

                    this.nameEntity.Add(nameEntity);
                }
            }
        }

        internal DescriptorConstruction Construction { get; }

        internal void TuneLinks()
        {
            if (nameEntity != null)
            {
                Capacity = nameEntity.Count;

                DescriptorSmallEntity e;
                foreach (string entity in nameEntity)
                {
                    e = Construction.FindExtension(entity, false);
                    if (e is null)
                        e = FormMain.Descriptors.FindItem(entity, false);
                    if (e is null)
                        e = FormMain.Descriptors.FindResource(entity, false);

                    Debug.Assert(e != null);

                    Add(e);
                }

                nameEntity = null;
            }
        }
    }
}