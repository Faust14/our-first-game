using Game.Player.Combat;
using Game.Player.Movement;
using Player.Combat;
using Player.Core;
using Player.Movement;
using UnityEngine;

namespace Game.Player.Animation
{
    /// <summary>
    /// Bridge između gameplay logike (movement/combat/health) i Animator-a.
    /// Postavlja Animator parametre (hash-irane), brine o triggerima i basic crossfade helperima.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimatorBridge : MonoBehaviour
    {
        [Header("Source Components (optional but recommended)")]
        [Tooltip("Ako je zadato, iz ovoga čitamo brzinu i Y velocity za animacije.")]
        [SerializeField] private Rigidbody2D rb2d;

        [Tooltip("Ground checker koji koristiš i za movement (za IsGrounded).")]
        [SerializeField] private GroundChecker groundChecker;

        [Header("Tuning")]
        [Tooltip("Koliko brzo lerpujemo vizuelnu brzinu za BlendTree (0 = odmah).")]
        [Range(0f, 20f)] public float speedLerp = 12f;

        [Tooltip("World brzina pri kojoj će Speed parametar biti 1.0 u Animatoru.")]
        [SerializeField] private float maxWorldSpeedForAnim = 5f;

        private Animator _anim;
        private float _displayedSpeed;
        private bool _isGrounded;
        private bool _isDashing;
        private bool _isAttacking;
        private int  _attackIndex;

        // Animator parametri
        private static readonly int SpeedHash        = Animator.StringToHash("Speed");
        private static readonly int GroundedHash     = Animator.StringToHash("IsGrounded");
        private static readonly int YVelHash         = Animator.StringToHash("YVel");
        private static readonly int DashingHash      = Animator.StringToHash("IsDashing");
        private static readonly int AttackingHash    = Animator.StringToHash("IsAttacking");
        private static readonly int AttackIndexHash  = Animator.StringToHash("AttackIndex");
        private static readonly int AttackTrigHash   = Animator.StringToHash("AttackTrig");
        private static readonly int CastTrigHash     = Animator.StringToHash("CastTrig");
        private static readonly int HitTrigHash      = Animator.StringToHash("HitTrig");
        private static readonly int DieTrigHash      = Animator.StringToHash("DieTrig");
        private static readonly int LandTrigHash     = Animator.StringToHash("LandTrig");
        private static readonly int JumpTrigHash     = Animator.StringToHash("JumpTrig");

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_anim == null) return;

            // 1) Čitanje brzine iz Rigidbody2D
            float worldSpeed = 0f;
            if (rb2d != null)
            {
                worldSpeed = rb2d.linearVelocity.x;
                _anim.SetFloat(YVelHash, rb2d.linearVelocity.y);
            }

            // 2) Grounded info
            bool grounded = groundChecker != null && groundChecker.IsGrounded;

            // 3) Izračunaj normalizovanu brzinu (0–1) i apdejtuj state
            SetMove(worldSpeed, grounded);

            // 4) Primeni vrednosti na animator
            _anim.SetFloat(SpeedHash, _displayedSpeed);   // sada je uvek 0–1
            _anim.SetBool(GroundedHash, _isGrounded);
            _anim.SetBool(DashingHash, _isDashing);
            _anim.SetBool(AttackingHash, _isAttacking);
            _anim.SetInteger(AttackIndexHash, _attackIndex);
        }

        #region Public API

        /// <summary>
        /// worldSpeed = brzina iz fizike (m/s). Ovde je normalizujemo u [0,1].
        /// </summary>
        public void SetMove(float worldSpeed, bool grounded)
        {
            float absSpeed = Mathf.Abs(worldSpeed);

            // Normalizacija na 0–1
            float target01 = 0f;
            if (maxWorldSpeedForAnim > 0f)
                target01 = Mathf.Clamp01(absSpeed / maxWorldSpeedForAnim);

            _displayedSpeed = Mathf.Lerp(
                _displayedSpeed,
                target01,
                1f - Mathf.Exp(-speedLerp * Time.deltaTime)
            );

            if (_displayedSpeed < 0.001f && target01 == 0f)
                _displayedSpeed = 0f;

            if (_isGrounded != grounded)
            {
                if (grounded && !_isGrounded)
                    _anim.SetTrigger(LandTrigHash);

                _isGrounded = grounded;
            }
        }

        public void SetDashing(bool value) => _isDashing = value;

        public void TriggerAttack(int comboIndex = 0)
        {
            _attackIndex = comboIndex;
            _isAttacking = true;
            _anim.ResetTrigger(AttackTrigHash);
            _anim.SetTrigger(AttackTrigHash);
        }

        public void EndAttack() => _isAttacking = false;

        public void TriggerCast()
        {
            _anim.ResetTrigger(CastTrigHash);
            _anim.SetTrigger(CastTrigHash);
        }

        public void TriggerHit()
        {
            _anim.ResetTrigger(HitTrigHash);
            _anim.SetTrigger(HitTrigHash);
        }

        public void TriggerDie()
        {
            _anim.ResetTrigger(DieTrigHash);
            _anim.SetTrigger(DieTrigHash);
        }

        public void TriggerJump()
        {
            _anim.ResetTrigger(JumpTrigHash);
            _anim.SetTrigger(JumpTrigHash);
        }

        public void Crossfade(string stateName, float normalizedTransitionDuration = 0.1f, int layer = 0, float normalizedTimeOffset = 0f)
        {
            _anim.CrossFadeInFixedTime(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset);
        }

        // Root.Init() ovo zove i prosleđuje pravi GroundChecker + RB
        public void Bind(
            PlayerMovement movement,
            PlayerCombatRouter combat,
            AbilityController abilities,
            PlayerHealth health,
            GroundChecker groundChecker,
            PlayerEvents events)
        {
            this.groundChecker = groundChecker;

            if (rb2d == null)
                rb2d = GetComponentInParent<Rigidbody2D>();
        }

        public void Tick(float dt) { }
        public void LateTick(float dt) { }
        public void Dispose() { }

        #endregion
    }
}
