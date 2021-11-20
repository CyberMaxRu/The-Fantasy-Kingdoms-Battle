namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс описателя
    internal abstract class Descriptor
    {
        internal static Descriptors Descriptors { get; set; }

        internal virtual void TuneLinks() { }
        internal virtual void AfterTuneLinks() { }
    }
}