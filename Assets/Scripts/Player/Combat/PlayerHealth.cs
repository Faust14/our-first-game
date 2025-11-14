using System;
using Game.Player.ClassSystem;
using Player.Core;
using Player.Combat;
using UnityEngine;

namespace Game.Player.Combat
{
    public sealed class PlayerHealth : IDamageable, IDisposable
    {
        private readonly PlayerConfigSO _cfg;
        private PlayerStats _stats;
        private PlayerEvents _events;

        public float Current { get; private set; }
        public float Max { get; private set; }
        public bool IsAlive { get; private set; } = true;

        public event Action<float, float> OnHealthChanged;

        private float _invulnTimer;
        private const float INVULN_TIME = 0.15f;

        public PlayerHealth(PlayerConfigSO cfg)
        {
            _cfg = cfg;
            Max = (_cfg != null) ? _cfg.BaseHealth : 100f;
            Current = Max;
            RaiseHealthChanged();
        }

        public void Bind(PlayerStats stats, PlayerEvents eventsBus)
        {
            _stats = stats;
            _events = eventsBus;

            // 📌 umesto stats.Health koristi:
            Max = (_stats != null) ? _stats.MaxHealth : _cfg.BaseHealth;
            Current = (_stats != null) ? _stats.CurrentHealth : (int)Max;
            IsAlive = Current > 0f;
            RaiseHealthChanged();

            // sync sa PlayerStats eventom (int → float)
            if (_stats != null)
                _stats.OnHealthChanged += OnStatsHealthChanged;
        }

        private void OnStatsHealthChanged(int current, int max)
        {
            Max = max;
            Current = current;
            IsAlive = Current > 0f;
            RaiseHealthChanged();
        }

        public void Tick(float dt)
        {
            if (_invulnTimer > 0f) _invulnTimer -= dt;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            if (_invulnTimer > 0f) return;

            if (_stats != null)
            {
                // delegiraj na PlayerStats da on emituje event
                int dmg = Math.Clamp((int)MathF.Round(amount), 0, int.MaxValue);
                _stats.TakeDamage(dmg, default, default);
            }
            else
            {
                Current = MathF.Max(0f, Current - amount);
                RaiseHealthChanged();
                if (Current <= 0f)
                {
                    IsAlive = false;
                    _events?.RaiseDied();
                }
            }

            _events?.RaiseTookDamage(amount);
            _invulnTimer = INVULN_TIME;
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;

            if (_stats != null)
            {
                int heal = Math.Clamp((int)MathF.Round(amount), 0, int.MaxValue);
                _stats.Heal(heal); // OnHealthChanged će se sam pozvati iz PlayerStats
            }
            else
            {
                Current = MathF.Min(Current + amount, Max);
                RaiseHealthChanged();
            }
        }

        private void RaiseHealthChanged() => OnHealthChanged?.Invoke(Current, Max);

        public void Dispose()
        {
            if (_stats != null)
                _stats.OnHealthChanged -= OnStatsHealthChanged;
        }

        public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitNormal)
        {
            throw new NotImplementedException();
        }
    }
}
