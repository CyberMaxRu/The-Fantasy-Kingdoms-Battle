using System.Drawing;

namespace Fantasy_Kingdoms_Battle
{
    // Класс сущности для конкретного лобби. Общий для игроков, зданий, предметов, абилок, существ, исследованийи т.д.
    internal abstract class Entity
    {

        public Entity()
        {
        }

        // Методы для поддержки работы с ячейкой
        internal abstract int GetImageIndex();
        internal virtual bool GetNormalImage() => true;
        internal virtual string GetLevel() => "";
        internal virtual int GetQuantity() => 0;
        internal virtual string GetText() => "";
        internal abstract void PrepareHint(PanelHint panelHint);

        internal virtual void Click(VCCell pe)
        {
        }

        internal virtual void CustomDraw(Graphics g, int x, int y, bool drawState)
        {

        }
    }
}
