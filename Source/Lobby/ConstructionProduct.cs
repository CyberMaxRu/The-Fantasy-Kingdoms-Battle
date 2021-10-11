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

        public ConstructionProduct(DescriptorConstructionVisit descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorConstructionVisit = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(DescriptorConstructionExtension descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorConstructionExtension = descriptor;
            Descriptor = descriptor;
        }

        internal DescriptorSmallEntity Descriptor { get; }
        internal DescriptorAbility DescriptorAbility { get; }
        internal DescriptorItem DescriptorItem { get; }
        internal DescriptorGroupItems DescriptorGroupItem { get; }
        internal DescriptorConstructionEvent DescriptorConstructionEvent { get; }
        internal DescriptorConstructionVisit DescriptorConstructionVisit { get; }
        internal DescriptorConstructionExtension DescriptorConstructionExtension { get; }
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
            if (DescriptorConstructionEvent != null)
            {
                Program.formMain.formHint.AddStep2Header(DescriptorConstructionEvent.NameGoods);
                Program.formMain.formHint.AddStep3Type("Событие");
                Program.formMain.formHint.AddStep4Level(Duration > 0 ? $"Осталось дней: {Counter}" : "");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                Program.formMain.formHint.AddStep9ListNeeds(DescriptorItem.ListNeeds);
            }
            else
            {
                Program.formMain.formHint.AddStep2Header(Descriptor.Name);
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep3Type("Посещение");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                if (DescriptorItem != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorItem.ListNeeds);
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionVisit.ListNeeds);
            }
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
