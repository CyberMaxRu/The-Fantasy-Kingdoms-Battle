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
            Interest = descriptor.Interest;
        }

        public ConstructionProduct(DescriptorConstructionVisit descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorConstructionVisit = descriptor;
            Descriptor = descriptor;
            Interest = descriptor.Interest;
        }

        public ConstructionProduct(DescriptorConstructionExtension descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            DescriptorConstructionExtension = descriptor;
            Descriptor = descriptor;
            Interest = descriptor.Interest;
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
        internal int Interest { get; set; }// Интерес героев к сущности
        internal bool Enabled { get; set; } = true;// Товар доступен для продажи

        internal override int GetImageIndex()
        {
            return Descriptor.ImageIndex;
        }

        internal override bool GetNormalImage() => Enabled;

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
                Program.formMain.formHint.AddStep2Header(DescriptorConstructionEvent.NameGoods, GetImageIndex());
                Program.formMain.formHint.AddStep3Type("Событие");
                Program.formMain.formHint.AddStep4Level(Duration > 0 ? $"Осталось дней: {Counter}" : "");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionEvent.ListNeeds);
            }
            else
            {
                Program.formMain.formHint.AddStep2Header(Descriptor.Name, GetImageIndex());
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep3Type("Посещение");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                if (DescriptorItem != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorItem.ListNeeds);
                if (DescriptorConstructionEvent != null)
                    Program.formMain.formHint.AddStep9Interest(Interest, false);
                if (DescriptorConstructionExtension != null)
                    Program.formMain.formHint.AddStep9Interest(Interest, true);
                if (DescriptorConstructionVisit != null)
                {
                    Program.formMain.formHint.AddStep9Interest(Interest, false);
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionVisit.ListNeeds);
                }
            }
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
