using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fantasy_Kingdoms_Battle.Utils;

namespace Fantasy_Kingdoms_Battle
{
    // Cпособность у существа
    internal sealed class Ability : EntityForCreature
    {
        public Ability(Creature creature, DescriptorAbility typeAbility) : base(creature, typeAbility)
        {
            Assert(creature != null);
            Assert(typeAbility != null);

            DescriptorAbility = typeAbility;
            Pos = creature.Abilities.Count + 1;
        }

        internal DescriptorAbility DescriptorAbility { get; }
        internal int Pos { get; }// Позиция, под которой умение было добавлено существу. Применяется для сортировки

        internal override string GetTypeEntity() => DescriptorAbility.TypeAbility.Name;

        internal override int GetImageIndex() => DescriptorAbility.ImageIndex;
        internal override string GetLevel() => DescriptorAbility.MinUnitLevel.ToString();
        internal override bool GetNormalImage() => Creature.Level >= DescriptorAbility.MinUnitLevel;
        internal override string GetText() => Program.formMain.Settings.ShowShortNames ? DescriptorAbility.TypeAbility.ShortName : "";

        internal override void PrepareHint(PanelHint panelHint)
        {
            panelHint.AddStep2Entity(this);
            panelHint.AddStep4Level($"Уровень для обучения: {DescriptorAbility.MinUnitLevel}");
            panelHint.AddStep5Description(DescriptorAbility.Description);
        }
    }
}
