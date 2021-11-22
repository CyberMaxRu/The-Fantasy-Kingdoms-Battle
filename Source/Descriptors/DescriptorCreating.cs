using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    internal class DescriptorCreating : Descriptor
    {
        public DescriptorCreating(DescriptorWithID entity, XmlNode n) : base()
        {
            Entity = entity;

            Builders = GetInteger(n, "Builders");
            DaysProcessing = GetInteger(n, "DaysProcessing");
            CostResources = new ListBaseResources(n.SelectSingleNode("Cost"));
            Requirements = new ListDescriptorRequirements(Entity, n.SelectSingleNode("Requirements"));

            Debug.Assert(DaysProcessing >= 0, $"У {entity} отрицательное число дней создания ({DaysProcessing}).");
            Debug.Assert(DaysProcessing <= 100);
        }

        internal DescriptorWithID Entity { get; }// Для какой активной сущности
        internal int Builders { get; }// Количество требуемых строителей
        internal int DaysProcessing { get; }// Количество дней для выполнения действия
        internal ListBaseResources CostResources { get; }// Стоимость
        internal ListDescriptorRequirements Requirements { get; }// Список требований для выполнения действия

        internal override void TuneLinks()
        {
            base.TuneLinks();

            Requirements.TuneLinks();
        }
    }
}
