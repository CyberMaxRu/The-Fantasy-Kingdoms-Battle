using System;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Класс - компонента прогресса выполнения действия
    internal sealed class ComponentExecutingAction
    {
        public ComponentExecutingAction(int seconds, int milliTicksPerTicks)
        {
            Assert(seconds > 0);

            TotalMilliTicks = seconds * FormMain.Config.TicksInSecond * 1000;
            RestMilliTicks = TotalMilliTicks;
            MilliTicksPerTick = milliTicksPerTicks;
            RestTimeExecuting = -1;
        }

        internal int TotalMilliTicks { get; }// Всего миллитиков для выполнения действия
        internal int PassedMilliTicks { get; private set; }// Прошло миллитиков
        internal int RestMilliTicks { get; private set; }// Осталось миллитиков
        internal int MilliTicksPerTick { get; private set; }// Количество миллитиков в одном тике
        internal int RestTimeExecuting { get; private set; }// Сколько секунд осталось до конца выполнения
        internal int Percent { get; private set; }// Процент выполнения
        internal bool InQueue { get; set; }// Действие в очереди

        // Добавление миллитика
        internal void CalcTick(int milliTicks)
        {
            Assert(PassedMilliTicks < TotalMilliTicks);
            Assert(milliTicks > 0);

            PassedMilliTicks += milliTicks;
            if (PassedMilliTicks > TotalMilliTicks)
                PassedMilliTicks = TotalMilliTicks;

            RestMilliTicks = TotalMilliTicks - PassedMilliTicks;

            MilliTicksPerTick = milliTicks;
        }

        internal void UpdateRestTimeExecuting()
        {
            // Прибавляем секунду, чтобы когда оставалось менее 1 секунды, индикатор не становился 0, а продолжал показывать 1
            RestTimeExecuting = (int)Math.Truncate(RestMilliTicks * 1.00000 / (MilliTicksPerTick * FormMain.Config.TicksInSecond) + 0.99);
            Assert(RestTimeExecuting > 0);
            Percent = PassedMilliTicks * 100 / TotalMilliTicks;
        }
    }
}