using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    internal abstract class ActionForEntity
    {
        private ListTextRequirement textRequirements = new ListTextRequirement();

        static ActionForEntity()
        {
            Descriptors = FormMain.Descriptors;
            Config = FormMain.Config;
        }

        public ActionForEntity(BigEntity bigEntity, DescriptorActionForEntity d)
        {
            BigEntity = bigEntity;
            Descriptor = d;
        }

        internal static Descriptors Descriptors { get; }
        internal static Config Config { get; }

        internal DescriptorActionForEntity Descriptor { get; }
        internal BigEntity BigEntity { get; }
        internal ListBaseResources PurchaseValue { get; } = new ListBaseResources();// Стоимость покупки

        internal bool Destroyed { get; set; }// Действие необходимо удалить
        internal abstract int GetImageIndex();
        internal virtual bool GetImageIsEnabled() => CheckRequirements();
        internal virtual string GetText() => "";
        internal virtual string GetLevel() => "";
        internal virtual int GetQuantity() => 0;
        internal virtual string GetExtInfo() { int t = GetTimeExecuting(); return t == -1 ? "" : t > 0 ? t.ToString() : "*"; }
        internal virtual StateRestTime GetStateRestTime() => StateRestTime.Active;
        protected virtual int GetTimeExecuting() => -1;
        internal virtual Color GetColorText() => FormMain.Config.CommonCost;
        internal virtual bool CheckRequirements() => true;
        internal ListTextRequirement GetTextRequirements()
        {
            textRequirements.Clear();
            UpdateTextRequirements(textRequirements);
            return textRequirements;
        }
        internal virtual void PrepareHint(PanelHint panelHint) { }
        internal virtual void PrepareNewDay() { }
        internal abstract void Click();

        internal virtual void UpdatePurchase() { }// Обновление стоимости покупки
        protected virtual void UpdateTextRequirements(ListTextRequirement list) { }
    }
}
