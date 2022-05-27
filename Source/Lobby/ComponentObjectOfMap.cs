using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Fantasy_Kingdoms_Battle
{
    // Компонет - объект карты
    internal sealed class ComponentObjectOfMap
    {
        public ComponentObjectOfMap(Construction owner, bool visible)
        {
            Debug.Assert(owner != null);
            Owner = owner;
            Visible = visible;
        }

        internal Construction Owner { get; }
        internal bool Visible { get; set; }
        internal TypeFlag TypeFlag { get; set; } = TypeFlag.None;// Тип установленного флага
        internal List<Hero> ListHeroesForFlag { get; } = new List<Hero>();// Список существ, выполняющих флаг

        internal void AddHeroForFlag(Hero ph)
        {
            Debug.Assert(ph != null);
            Debug.Assert(ListHeroesForFlag.IndexOf(ph) == -1);
            Debug.Assert(ph.StateCreature.ID == NameStateCreature.Nothing.ToString());
            Debug.Assert(ph.TargetByFlag == null);
            Owner.AssertNotDestroyed();
            Debug.Assert(ListHeroesForFlag.Count < MaxHeroesForFlag());

            ListHeroesForFlag.Add(ph);
            ph.TargetByFlag = Owner;
            ph.SetState(ph.StateForFlag(TypeFlag));
        }

        internal void RemoveAttackingHero(Hero ph)
        {
            Debug.Assert(ListHeroesForFlag.IndexOf(ph) != -1);
            Debug.Assert(ph.TargetByFlag == Owner);
            Owner.AssertNotDestroyed();

            ph.TargetByFlag = null;
            ListHeroesForFlag.Remove(ph);
            ph.SetState(NameStateCreature.Nothing);
        }


        internal int MaxHeroesForFlag()
        {
            switch (Owner.TypeAction())
            {
                case TypeFlag.Scout:
                    return 200;
                case TypeFlag.Attack:
                case TypeFlag.Defense:
                case TypeFlag.Battle:
                    return Owner.Player.Lobby.TypeLobby.MaxHeroesForBattle;
                default:
                    throw new Exception($"Неизвестный тип действия: {Owner.TypeAction()}");
            }
        }

        internal void DropFlag()
        {
            Debug.Assert(TypeFlag != TypeFlag.None);
            Owner.AssertNotDestroyed();

            //Owner.Player.RemoveFlag(this);

            TypeFlag = TypeFlag.None;
            
            while (ListHeroesForFlag.Count > 0)
                RemoveAttackingHero(ListHeroesForFlag[0]);

            Owner.Lobby.Layer.LairsWithFlagChanged();
        }


    }
}
