namespace Fantasy_Kingdoms_Battle
{
    // Базовый класс описателя
    internal abstract class Descriptor
    {
        internal static Config Descriptors { get; set; }

        internal virtual void TuneLinks() { }

        internal virtual void AfterTuneLinks() { }
    }
}