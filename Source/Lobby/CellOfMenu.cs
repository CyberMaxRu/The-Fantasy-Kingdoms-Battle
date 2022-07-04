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
    internal abstract class CellOfMenu
    {
        static CellOfMenu()
        {
            Descriptors = FormMain.Descriptors;
            Config = FormMain.Config;
        }

        public CellOfMenu(BigEntity bigEntity, DescriptorCellMenu d)
        {
            BigEntity = bigEntity;
            Descriptor = d;
        }

        internal static Descriptors Descriptors { get; }
        internal static Config Config { get; }

        internal DescriptorCellMenu Descriptor { get; }
        internal BigEntity BigEntity { get; }

        internal int DaysLeft { get; set; }// Сколько дней осталось до окончания обработки действия
        internal ListBaseResources PurchaseValue { get; private protected set; }// Стоимость покупки

        internal virtual string GetText() => "";
        internal abstract int GetImageIndex();
        internal virtual bool GetImageIsEnabled() => CheckRequirements();
        internal virtual string GetLevel() => "";
        internal virtual int GetQuantity() => 0;
        internal virtual string GetDaysExecuting() => "";
        internal virtual Color GetColorText() => FormMain.Config.CommonCost;
        internal virtual bool CheckRequirements() => true;
        internal virtual List<TextRequirement> GetTextRequirements() => new List<TextRequirement>();// Переделать на null
        internal virtual void PrepareHint(PanelHint panelHint) { }
        internal virtual void PrepareNewDay() { }
        internal abstract void Click();
        internal abstract void Execute();
        internal abstract bool InstantExecute();

        internal virtual void UpdatePurchase() { }// Обновление стоимости покупки
    }
}
