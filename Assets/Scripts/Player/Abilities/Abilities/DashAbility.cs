using System.Collections;
using UnityEngine;

namespace Game.Abilities.Abilities
{
    /// <summary>
    /// Kratak horizontalni burst brzine. Ne troši mana po difoltu, ima cooldown.
    /// Traži Rigidbody2D na owner-u.
    /// </summary>
    public class DashAbility : AbilityBase
    {
        [Header("Dash")]
        public float dashSpeed = 16f;
        public float dashTime  = 0.12f;
        public float dashDrag  = 0f;     // privremeni linear drag
        public bool  cancelYVelocity = true;
        public bool  grantIFrames = false;
        public LayerMask groundMask; // opciono (ako želiš da prekineš dash pri udaru u zid)

        private Rigidbody2D _rb;
        private float _origDrag;

        public override void Initialize(GameObject ownerGO)
        {
            base.Initialize(ownerGO);
            if (owner)
                _rb = owner.GetComponent<Rigidbody2D>();
        }

        protected override void OnCastPerform()
        {
            if (_rb == null) return;
            StopAllCoroutines();
            StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            int dir = FacingSign();
            _origDrag = _rb.linearDamping;

            if (cancelYVelocity) _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);

            if (dashDrag > 0f) _rb.linearDamping = dashDrag;

            if (grantIFrames)
            {
                // Ako imaš PlayerHealth sa iFrame-ovima, ovde pozovi enable
                // owner.GetComponent<PlayerHealth>()?.SetInvulnerable(true);
            }

            _rb.linearVelocity = new Vector2(dir * dashSpeed, _rb.linearVelocity.y);

            float t = dashTime / (scaleWithAttackSpeed && stats ? Mathf.Max(0.1f, stats.AttackSpeed) : 1f);
            float end = Time.time + t;
            while (Time.time < end)
            {
                // održavaj horizontalnu brzinu tokom dash-a
                _rb.linearVelocity = new Vector2(dir * dashSpeed, _rb.linearVelocity.y);
                yield return null;
            }

            _rb.linearDamping = _origDrag;

            if (grantIFrames)
            {
                // owner.GetComponent<PlayerHealth>()?.SetInvulnerable(false);
            }
        }
    }
}
