using System;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс - компонента прогресса выполнения действия
    internal sealed class ComponentProgressExecuting
    {
        public ComponentProgressExecuting(int seconds, int milliTicksPerTicks)
        {
            Assert(seconds > 0);
            Assert(milliTicksPerTicks > 0);

            TotalMilliTicks = seconds * FormMain.Config.TicksInSecond * 1000;
            RestMilliTicks = TotalMilliTicks;
            MilliTicksPerTick = milliTicksPerTicks;
            RestTimeExecuting = -1;
        }

        // Свойства для расчета прогресса
        internal int TotalMilliTicks { get; }// Всего миллитиков для выполнения действия
        internal int PassedMilliTicks { get; private set; }// Прошло миллитиков
        internal int RestMilliTicks { get; private set; }// Осталось миллитиков
        internal int MilliTicksPerTick { get; private set; }// Количество миллитиков в одном тике
        internal int RestTimeExecuting { get; private set; }// Сколько секунд осталось до конца выполнения

        // Дополнительные флаги
        internal bool InQueue { get; set; }// Действие в очереди

        // Обработка тика игры
        internal void CalcTick(int addMilliTicks)
        {
            Assert(PassedMilliTicks < TotalMilliTicks);
            Assert(addMilliTicks > 0);

            PassedMilliTicks += addMilliTicks;
            if (PassedMilliTicks > TotalMilliTicks)
                PassedMilliTicks = TotalMilliTicks;

            RestMilliTicks = TotalMilliTicks - PassedMilliTicks;

            MilliTicksPerTick = addMilliTicks;
        }

        internal void UpdateRestTimeExecuting()
        {
            // Прибавляем секунду, чтобы когда оставалось менее 1 секунды, индикатор не становился 0, а продолжал показывать 1
            RestTimeExecuting = (int)Math.Truncate(RestMilliTicks * 1.00000 / (MilliTicksPerTick * FormMain.Config.TicksInSecond) + 0.99);
            Assert(RestTimeExecuting > 0);
        }
    }
}