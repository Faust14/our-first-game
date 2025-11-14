using System;

namespace Player.Core
{
    /// <summary>Jednostavan event bus za komunikaciju među podsistemima.</summary>
    public sealed class PlayerEvents : IDisposable
    {
        // Primeri događaja:
        public event Action<float> OnTookDamage;
        public event Action OnDied;
        public event Action OnJump;
        public event Action OnDash;
        public event Action OnPrimaryAttack;
        public event Action OnCastSpell;

        public void RaiseTookDamage(float amount) => OnTookDamage?.Invoke(amount);
        public void RaiseDied() => OnDied?.Invoke();
        public void RaiseJump() => OnJump?.Invoke();
        public void RaiseDash() => OnDash?.Invoke();
        public void RaisePrimaryAttack() => OnPrimaryAttack?.Invoke();
        public void RaiseCastSpell() => OnCastSpell?.Invoke();

        public void Dispose()
        {
            // Clear delegates
            OnTookDamage = null;
            OnDied = null;
            OnJump = null;
            OnDash = null;
            OnPrimaryAttack = null;
            OnCastSpell = null;
        }
    }
}
