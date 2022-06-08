using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Описатель игрока
    internal abstract class DescriptorPlayer : DescriptorEntity
    {
        public DescriptorPlayer(XmlNode n, TypePlayer typePlayer) : base(n)
        {
            TypePlayer = typePlayer;
        }

        public DescriptorPlayer(XmlNode n, int index) : base(n)
        {
            Index = index;
            Title = GetStringNotNull(n, "Title");
            ImageIndex = GetIntegerNotNull(n, "ImageIndex");
            TypePlayer = (TypePlayer)Enum.Parse(typeof(TypePlayer), GetStringNotNull(n, "TypePlayer"));
        }

        public DescriptorPlayer(string id, string name, string description, int imageIndex, TypePlayer typePlayer) : base(id, name, description, imageIndex)
        {
            TypePlayer = typePlayer;
        }

        internal int Index { get; }// Индекс
        internal string Title { get; }// Титул
        internal TypePlayer TypePlayer { get; }

        protected override int ShiftImageIndex() => Config.ImageIndexFirstAvatar;
        internal override string GetTypeEntity() => "Игрок";
    }
}