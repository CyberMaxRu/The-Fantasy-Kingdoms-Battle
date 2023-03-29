using System;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Состояния прогресса - неактивно (не поставлено в очередь), активно, ожидание (стоит в очереди, не первое), ожидание строителей (первое)
    internal enum StateProgress { Inactive, Active, WaitInQueue, WaitBuilders };

    // Класс - компонента прогресса подготовки действия
    internal sealed class ComponentProgressExecuting
    {
        public ComponentProgressExecuting(int seconds, int needBuilders, int milliTicksPerTicks)
        {
            Assert(seconds > 0);
            Assert(milliTicksPerTicks > 0);

            TotalMilliTicks = seconds * FormMain.Config.TicksInSecond * 1000;
            RestMilliTicks = TotalMilliTicks;
            MilliTicksPerTick = milliTicksPerTicks;
            RestTimeExecuting = -1;
            NeedBuilders = needBuilders;
            State = StateProgress.Inactive;
        }

        // Свойства для расчета прогресса
        internal int TotalMilliTicks { get; private set; }// Всего миллитиков для выполнения действия
        internal int PassedMilliTicks { get; private set; }// Прошло миллитиков
        internal int RestMilliTicks { get; private set; }// Осталось миллитиков
        internal int MilliTicksPerTick { get; private set; }// Количество миллитиков в одном тике
        internal int RestTimeExecuting { get; private set; }// Сколько секунд осталось до конца выполнения

        // Дополнительные флаги
        internal StateProgress State { get; set; }// Состояние выполнения
        internal bool InQueue { get; set; }// Действие в очереди
        internal int NeedBuilders { get; set; }// Количество необходимых строителей
        internal int UsedBuilders { get; set; }// Количество задействованных строителей

        // Обработка тика игры
        internal void CalcTick(int addMilliTicks)
        {
            Assert(State == StateProgress.Active);
            if (TotalMilliTicks > 0)
            {
                Assert(PassedMilliTicks < TotalMilliTicks);
                Assert(addMilliTicks > 0);
                Assert(InQueue);
                Assert(((NeedBuilders > 0) && (UsedBuilders == NeedBuilders)) || ((NeedBuilders == 0) && (UsedBuilders == 0)));

                PassedMilliTicks += addMilliTicks;
                if (PassedMilliTicks > TotalMilliTicks)
                    PassedMilliTicks = TotalMilliTicks;

                RestMilliTicks = TotalMilliTicks - PassedMilliTicks;

                MilliTicksPerTick = addMilliTicks;
            }
            else
            {
                Assert(PassedMilliTicks == 0);
            }
        }

        // В метод передаем текущее количество миллитиков в тике, иначе если действие не выполняется,
        // а стартовое значение MilliTicksPerTick больше не меняется и время не пересчитывается
        internal void UpdateRestTimeExecuting(int milliTicksPerTick)
        {
            RestTimeExecuting = Program.formMain.CalcRestTime(RestMilliTicks, milliTicksPerTick);
        }

        internal void RefreshProgress(int seconds, int milliTicksPerTicks)
        {
            TotalMilliTicks = seconds * FormMain.Config.TicksInSecond * 1000;
            RestMilliTicks = TotalMilliTicks;
            MilliTicksPerTick = milliTicksPerTicks;
            PassedMilliTicks = 0;
            RestTimeExecuting = -1;
        }
    }
}