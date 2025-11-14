using System.Collections;
using UnityEngine;
using Game.Player.ClassSystem; // za PlayerStats

namespace Game.Abilities
{
    /// <summary>
    /// Baza za sve abiliti-je: mana/cooldown/cast pipeline (+ attack speed scaling).
    /// Pozivni niz: TryCast -> (CanCast) -> CastRoutine -> OnCastStart/Perform/End
    /// </summary>
    public abstract class AbilityBase : MonoBehaviour, IAbility
    {
        [Header("Meta")]
        [SerializeField] private string displayName = "Ability";
        [SerializeField] private Sprite icon;

        [Header("Costs & Timings")]
        [Min(0)] public int   manaCost = 0;
        [Min(0f)] public float cooldown = 0.5f;
        [Min(0f)] public float castTime = 0f; // vreme „windup“-a
        [Tooltip("Ako je uključeno, cooldown i/ili castTime se ubrzavaju prema AttackSpeed.")]
        public bool scaleWithAttackSpeed = true;
        [Tooltip("Skalirati cooldown (true) ili samo cast time (false).")]
        public bool scaleCooldownToo = false;

        [Header("Gating")]
        [Tooltip("Ako je true, ability traži da igrač bude okrenut u pravcu targeta/izlaza (koristi scaleX).")]
        public bool useFacing = true;

        protected GameObject owner;
        protected PlayerStats stats;

        private float _lastCastEndTime = -999f;
        private bool _isCasting;

        public string DisplayName => displayName;
        public float CooldownRemaining
        {
            get
            {
                float cd = GetEffectiveCooldown();
                float t  = Time.time - _lastCastEndTime;
                return Mathf.Max(0f, cd - t);
            }
        }

        public virtual void Initialize(GameObject ownerGO)
        {
            owner = ownerGO;
            if (owner)
                stats = owner.GetComponent<PlayerStats>();
        }

        public virtual bool CanCast()
        {
            if (_isCasting) return false;
            if (CooldownRemaining > 0f) return false;
            if (stats && manaCost > 0 && stats.CurrentMana < manaCost) return false;
            return true;
        }

        public bool TryCast()
        {
            if (!CanCast()) return false;
            StartCoroutine(CastRoutine());
            return true;
        }

        private IEnumerator CastRoutine()
        {
            _isCasting = true;

            // mana rezervacija (opciono odmah)
            if (stats && manaCost > 0)
            {
                bool ok = stats.ConsumeMana(manaCost);
                if (!ok) { _isCasting = false; yield break; }
            }

            OnCastStarted();

            float ct = GetEffectiveCastTime();
            if (ct > 0f) yield return new WaitForSeconds(ct);

            OnCastPerform();

            // cooldown tek po završetku
            _lastCastEndTime = Time.time;
            OnCastEnded();

            _isCasting = false;
        }

        protected float GetEffectiveCastTime()
        {
            if (!scaleWithAttackSpeed || stats == null || stats.AttackSpeed <= 0f)
                return castTime;
            // Viši AttackSpeed -> kraći cast time
            return castTime / stats.AttackSpeed;
        }

        protected float GetEffectiveCooldown()
        {
            if (!scaleWithAttackSpeed || !scaleCooldownToo || stats == null || stats.AttackSpeed <= 0f)
                return cooldown;
            return cooldown / stats.AttackSpeed;
        }

        /// <summary>Hook pre izvođenja (anim trigger, VFX warmup...)</summary>
        protected virtual void OnCastStarted() { }

        /// <summary>Glavna akcija – ovde instanciraj hitbox/projektil ili primeni efekat.</summary>
        protected abstract void OnCastPerform();

        /// <summary>Hook posle izvođenja.</summary>
        protected virtual void OnCastEnded() { }

        /// <summary>+1 ili -1, zavisno od lok. skale vlasnika (za smer levo/desno).</summary>
        protected int FacingSign()
        {
            if (!useFacing || owner == null) return 1;
            var sx = owner.transform.localScale.x;
            return sx >= 0f ? 1 : -1;
        }
    }
}
