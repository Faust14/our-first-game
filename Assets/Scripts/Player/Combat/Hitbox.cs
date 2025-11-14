using System.Collections.Generic;
using UnityEngine;

namespace Player.Combat
{
    /// <summary>
    /// MonoBehaviour trigger koji nanosi damage IDamageable meta-pravcima.
    /// Možeš ga držati DISABLED i "armirati" kroz Arm(dur, dmg).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class Hitbox : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField] private LayerMask _targetMask = ~0; // koga gađamo
        [SerializeField] private bool _useOwnerToIgnoreSelf = true;

        [Header("Damage")]
        [SerializeField] private float _defaultDamage = 10f;
        [SerializeField] private float _perTargetCooldown = 0.15f;

        private readonly Dictionary<IDamageable, float> _cooldowns = new();

        private Transform _owner;
        private float _armedTimer;
        private float _damage;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void Update()
        {
            // ističu per-target cooldowni
            if (_cooldowns.Count > 0)
            {
                var toClear = new List<IDamageable>();
                foreach (var kv in _cooldowns)
                {
                    var t = kv.Value - Time.deltaTime;
                    if (t <= 0f) toClear.Add(kv.Key);
                    else _cooldowns[kv.Key] = t;
                }
                foreach (var k in toClear) _cooldowns.Remove(k);
            }

            if (_armedTimer > 0f)
            {
                _armedTimer -= Time.deltaTime;
                if (_armedTimer <= 0f) gameObject.SetActive(false);
            }
        }

        /// <summary>Aktiviraj hitbox na duration sa specificiranim damage-om.</summary>
        public void Arm(float duration, float? damage = null, Transform owner = null)
        {
            _armedTimer = duration;
            _damage = damage ?? _defaultDamage;
            _owner = owner;
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsInMask(other.gameObject.layer)) return;

            if (_useOwnerToIgnoreSelf && _owner != null)
            {
                if (other.transform == _owner || other.transform.IsChildOf(_owner)) return;
            }

            // pokušaj naći IDamageable na targetu ili roditelju
            if (!other.TryGetComponent<IDamageable>(out var dmg))
                dmg = other.GetComponentInParent<IDamageable>();

            if (dmg == null || !dmg.IsAlive) return;

            // per-target lean cooldown
            if (_cooldowns.ContainsKey(dmg)) return;
            _cooldowns[dmg] = _perTargetCooldown;

            dmg.TakeDamage(_damage);
        }

        private bool IsInMask(int layer)
        {
            return (_targetMask.value & (1 << layer)) != 0;
        }
    }
}
