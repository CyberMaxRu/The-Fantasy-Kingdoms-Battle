using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    internal sealed class ConstructionProduct : SmallEntity
    {
        public ConstructionProduct(DescriptorAbility descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorAbility = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorItem descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorItem = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorGroupItems descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorGroupItem = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorConstructionEvent descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorConstructionEvent = descriptor;
            Descriptor = descriptor;

            Duration = descriptor.Duration;
            Counter = Duration;
        }

        internal DescriptorSmallEntity Descriptor { get; }
        internal DescriptorAbility DescriptorAbility { get; }
        internal DescriptorItem DescriptorItem { get; }
        internal DescriptorGroupItems DescriptorGroupItem { get; }
        internal DescriptorConstructionEvent DescriptorConstructionEvent { get; }
        internal int Duration { get; private set; }// Длительность нахождения товара в сооружении
        internal int Counter { get; set; }// Счетчик дней товара в сооружении

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override bool GetNormalImage()
        {
            return true;
        }

        internal override string GetText()
        {
            if (Duration > 0)
                return Counter.ToString();
            //if (DescriptorAbility != null)
            //    return DescriptorAbility.TypeAbility.ShortName;

//            if (DescriptorItem != null)
                //return DescriptorItem.CategoryItem;
            return base.GetText();
        }

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Descriptor.Name, "", Descriptor.Description);
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
