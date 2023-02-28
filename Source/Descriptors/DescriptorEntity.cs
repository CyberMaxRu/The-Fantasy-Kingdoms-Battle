using System;
using System.Diagnostics;
using System.Xml;
using static Fantasy_Kingdoms_Battle.XmlUtils;

namespace Fantasy_Kingdoms_Battle
{
    // Базовый описатель для всех сущностей - сооружений, существ, умений, предметов и т.д.
    internal abstract class DescriptorEntity : DescriptorVisual
    {
        public DescriptorEntity(XmlNode n) : base(n)
        {
            string soundSelect = GetString(n, "SoundSelect");
            if (soundSelect.Length > 0)
                UriSoundSelect = new Uri(Program.FolderResources + @"Sound\Interface\ConstructionSelect\" + soundSelect);

            Descriptors.AddEntity(this);
        }

        public DescriptorEntity(string id, string name, string description, int imageIndex) : base(id, name, description, imageIndex)
        {
            Descriptors.AddEntity(this);
        }

        internal Uri UriSoundSelect { get; }// Проигрываемый звук при выборе сущности
        internal abstract string GetTypeEntity();
    }
}
