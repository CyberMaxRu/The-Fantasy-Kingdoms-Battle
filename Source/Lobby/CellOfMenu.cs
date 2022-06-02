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
        public CellOfMenu(BigEntity forEntity, DescriptorCellMenu d)
        {
            BigEntity = forEntity;
            Descriptor = d;
            PosInQueue = 0;
        }

        internal static Descriptors Descriptors { get; set; }
        internal static Config Config { get; set; }
        internal DescriptorCellMenu Descriptor { get; }
        internal BigEntity BigEntity { get; }

        internal int DaysProcessed { get; set; }// Количество дней, прошедшее с начала обработки действия ячейки
        internal int DaysLeft { get; set; }// Сколько дней осталось до окончания обработки действия
        internal int PosInQueue { get; set; }// Номер в очереди
        internal ListBaseResources PurchaseValue { get; set; }// Стоимость покупки

        internal virtual string GetText() => GetCost().ValueGold().ToString();
        internal virtual ListBaseResources GetCost() => null;
        internal abstract int GetImageIndex();
        internal virtual bool GetImageIsEnabled() => CheckRequirements() && (DaysProcessed == 0);
        internal virtual string GetLevel() => "";
        internal virtual Color GetColorText() => FormMain.Config.CommonCost;
        internal virtual bool CheckRequirements() => true;
        internal virtual List<TextRequirement> GetTextRequirements() => new List<TextRequirement>();
        internal virtual void PrepareHint(PanelHint panelHint) { }
        internal abstract void Click();
        internal abstract void Execute();
        internal abstract bool InstantExecute();
        internal virtual void PrepareNewDay() { }
    }
}