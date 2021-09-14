using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy_Kingdoms_Battle
{
    // Cпособность у существа
    internal sealed class Ability : SmallEntity
    {
        public Ability(Creature creature, DescriptorAbility typeAbility) : base()
        {
            Creature = creature;
            Descriptor = typeAbility;
            Pos = creature.Abilities.Count + 1;
        }

        internal Creature Creature { get; }
        internal DescriptorAbility Descriptor { get; }
        internal int Pos { get; }// Позиция, под которой умение было добавлено существу. Применяется для сортировки

        internal override int GetImageIndex() => Descriptor.ImageIndex;
        internal override string GetLevel() => Descriptor.MinUnitLevel.ToString();
        internal override bool GetNormalImage() => Creature.Level >= Descriptor.MinUnitLevel;
        internal override string GetText() => Program.formMain.Settings.ShowShortNames ? Descriptor.TypeAbility.ShortName : "";

        internal override void PrepareHint()
        {
            Program.formMain.formHint.AddStep1Header(Descriptor.Name, $"{Descriptor.TypeAbility.Name}{Environment.NewLine}Уровень для обучения: {Descriptor.MinUnitLevel}", Descriptor.Description, Descriptor.ImageIndex, false);
        }
    }
}
