using System;
using UnityEngine;
using Game.Player.ClassSystem;
using Player.Combat;

namespace Game.Player.ClassSystem
{
    /// <summary>
    /// Drži trenutne i maksimalne statistike igrača + evente.
    /// Implementira IDamageable.
    /// </summary>
    [DisallowMultipleComponent]
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        [Header("Runtime (read-only u Inspectoru)")]
        [SerializeField] private int   _maxHealth    = 100;
        [SerializeField] private int   _maxMana      = 50;
        [SerializeField] private float _baseDamage   = 10f;
        [SerializeField] private float _attackSpeed  = 1.0f;
        [SerializeField] private float _moveSpeed    = 6f;
        [SerializeField] private float _jumpForce    = 12f;

        [Space(6)]
        [SerializeField] private int   _currentHealth;
        [SerializeField] private int   _currentMana;

        public int   MaxHealth   => _maxHealth;
        public int   MaxMana     => _maxMana;
        public float BaseDamage  => _baseDamage;
        public float AttackSpeed => _attackSpeed;
        public float MoveSpeed   => _moveSpeed;
        public float JumpForce   => _jumpForce;

        public int CurrentHealth => _currentHealth;
        public int CurrentMana   => _currentMana;

        public event Action<int, int> OnHealthChanged; // (current, max)
        public event Action<int, int> OnManaChanged;   // (current, max)
        public event Action           OnDied;

        public bool IsDead => _currentHealth <= 0;

        public bool IsAlive => throw new NotImplementedException();

        /// <summary>Primeni ClassDefinition na stats-e i refill-uj barove.</summary>
        public void ApplyClassDefinition(ClassDefinitionSO def)
        {
            if (!def) return;

            _maxHealth   = Mathf.Max(1, def.maxHealth);
            _maxMana     = Mathf.Max(0, def.maxMana);
            _baseDamage  = Mathf.Max(0f, def.baseDamage) * Mathf.Max(0.1f, def.damageMultiplier);
            _attackSpeed = Mathf.Max(0.05f, def.attackSpeed) * Mathf.Max(0.1f, def.attackSpeedMultiplier);
            _moveSpeed   = Mathf.Max(0f, def.moveSpeed) * Mathf.Max(0.1f, def.moveSpeedMultiplier);
            _jumpForce   = Mathf.Max(0f, def.jumpForce);

            _currentHealth = _maxHealth;
            _currentMana   = _maxMana;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            OnManaChanged?.Invoke(_currentMana, _maxMana);
        }

        #region Health / Mana API
        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead) return;
            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void RestoreMana(int amount)
        {
            if (amount <= 0) return;
            _currentMana = Mathf.Min(_currentMana + amount, _maxMana);
            OnManaChanged?.Invoke(_currentMana, _maxMana);
        }

        public bool ConsumeMana(int amount)
        {
            if (amount < 0) return true; // no-op
            if (_currentMana < amount) return false;
            _currentMana -= amount;
            OnManaChanged?.Invoke(_currentMana, _maxMana);
            return true;
        }
        #endregion

        #region IDamageable
        public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitNormal)
        {
            if (IsDead || amount <= 0) return;
            _currentHealth -= amount;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                OnDied?.Invoke();
            }
        }

        public void TakeDamage(float amount)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
