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
        public ConstructionProduct(Construction construction, DescriptorAbility descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            Construction = construction;
            DescriptorAbility = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(Construction construction, DescriptorItem descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            Construction = construction;
            DescriptorItem = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(Construction construction, DescriptorGroupItems descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            Construction = construction;
            DescriptorGroupItem = descriptor;
            Descriptor = descriptor;
        }

        public ConstructionProduct(Construction construction, DescriptorConstructionEvent descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            Construction = construction;
            DescriptorConstructionEvent = descriptor;
            Descriptor = descriptor;

            Duration = descriptor.Duration;
            Counter = Duration;
            Interest = descriptor.Interest;
        }

        public ConstructionProduct(Construction construction, DescriptorConstructionVisit descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            Construction = construction;
            DescriptorConstructionVisit = descriptor;
            Descriptor = descriptor;
            Interest = descriptor.Interest;
        }

        public ConstructionProduct(Construction construction, DescriptorConstructionExtension descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            Construction = construction;
            DescriptorConstructionExtension = descriptor;
            Descriptor = descriptor;
            Interest = descriptor.Interest;
        }

        public ConstructionProduct(Construction construction, DescriptorResource descriptor) : base()
        {
            Debug.Assert(descriptor != null);

            Construction = construction;
            DescriptorResource = descriptor;
            Descriptor = descriptor;
        }

        internal Construction Construction { get; }
        internal DescriptorSmallEntity Descriptor { get; }
        internal DescriptorAbility DescriptorAbility { get; }
        internal DescriptorItem DescriptorItem { get; }
        internal DescriptorGroupItems DescriptorGroupItem { get; }
        internal DescriptorConstructionEvent DescriptorConstructionEvent { get; }
        internal DescriptorConstructionVisit DescriptorConstructionVisit { get; }
        internal DescriptorConstructionExtension DescriptorConstructionExtension { get; }
        internal DescriptorResource DescriptorResource { get; }
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
                Program.formMain.formHint.AddStep3Type("Мероприятие");
                Program.formMain.formHint.AddStep4Level(Duration > 0 ? $"Осталось дней: {Counter}" : "");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionEvent.ListNeeds, true);
            }
            else
            {
                Program.formMain.formHint.AddStep2Header(Descriptor.Name, GetImageIndex());
                if (DescriptorAbility != null)
                    Program.formMain.formHint.AddStep3Type(DescriptorAbility.TypeAbility.Name);
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep3Type("Посещение");
                if (DescriptorResource != null)
                    Program.formMain.formHint.AddStep3Type("Ресурс");
                Program.formMain.formHint.AddStep5Description(Descriptor.Description);
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep9Interest(Interest, false);
                if (DescriptorItem != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorItem.ListNeeds, false);
                if (DescriptorConstructionExtension != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionExtension.ListNeeds, true);
                if (DescriptorConstructionEvent != null)
                    Program.formMain.formHint.AddStep9Interest(Interest, false);
                if (DescriptorConstructionExtension != null)
                    Program.formMain.formHint.AddStep9Interest(Interest, true);
                if (DescriptorConstructionVisit != null)
                    Program.formMain.formHint.AddStep9ListNeeds(DescriptorConstructionVisit.ListNeeds, false);
            }
        }

        internal bool IsAvailableForCreature(DescriptorCreature dc)
        {
            return Descriptor.AvailableForAllHeroes || (Descriptor.AvailableForHeroes.IndexOf(dc) != -1);
        }
    }
}
