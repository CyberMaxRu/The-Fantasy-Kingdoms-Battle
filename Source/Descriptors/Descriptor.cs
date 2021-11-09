namespace Fantasy_Kingdoms_Battle
{
    internal abstract class Descriptor
    {
        internal static Config Config { get; set; }

        internal virtual void TuneLinks() { }

        internal virtual void AfterTuneLinks() { }
    }
}
