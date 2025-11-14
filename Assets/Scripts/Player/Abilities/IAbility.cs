using UnityEngine;

namespace Game.Abilities
{
    public interface IAbility
    {
        /// <summary>Jednokratna inicijalizacija kada se ability „zakači” na vlasnika.</summary>
        void Initialize(GameObject owner);

        /// <summary>Da li ability može trenutno da se aktivira (mana, cooldown, stanje)?</summary>
        bool CanCast();

        /// <summary>Pokušaj aktivacije. Vraća true ako je aktivirano.</summary>
        bool TryCast();

        /// <summary>Preostalo vreme cooldown-a u sekundama.</summary>
        float CooldownRemaining { get; }

        /// <summary>Opisno ime (za UI).</summary>
        string DisplayName { get; }
    }
}
