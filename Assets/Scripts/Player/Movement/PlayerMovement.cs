using UnityEngine;
using Player.Input;
using Player.Core;
using Game.Player.ClassSystem;
using Player.Movement;

namespace Game.Player.Movement
{
    /// <summary>
    /// Čista C# logika kretanja. Koristi Rigidbody2D za fiziku i JumpForce iz PlayerConfigSO.
    /// </summary>
    public sealed class PlayerMovement : System.IDisposable
    {
        private readonly PlayerConfigSO _cfg;
        private readonly Rigidbody2D _rb;

        private PlayerInputFacade _input;
        private GroundChecker _ground;
        private PlayerStats _stats;
        private PlayerEvents _events;

        // Jump QoL tajmeri
        private bool _wantJump;
        private float _coyoteTimer;
        private float _jumpBufferTimer;

        private const float COYOTE_TIME = 0.10f;
        private const float JUMP_BUFFER = 0.10f;

        public bool IsGrounded => _ground != null && _ground.IsGrounded;

        public PlayerMovement(PlayerConfigSO cfg, Rigidbody2D rb)
        {
            _cfg = cfg;
            _rb = rb;
        }

        public void Bind(PlayerInputFacade input, GroundChecker ground, PlayerStats stats, PlayerEvents eventsBus)
        {
            _input = input;
            _ground = ground;
            _stats = stats;
            _events = eventsBus;
        }

        public void Tick(float dt)
        {
            if (_rb == null) return;

            // horizontalno kretanje
            var dir = _input?.MoveDirection ?? Vector2.zero;
            var vel = _rb.linearVelocity;

            float moveSpeed = (_stats != null && _stats.MoveSpeed > 0f)
                ?_cfg.MoveSpeed
                :  _stats.MoveSpeed;

            vel.x = dir.x * moveSpeed;
            _rb.linearVelocity = vel;

            // jump tajmeri
            if (IsGrounded)
                _coyoteTimer = COYOTE_TIME;
            else
                _coyoteTimer -= dt;

            _jumpBufferTimer -= dt;

            // input skoka
            if (_input?.JumpPressed == true)
            {
                _jumpBufferTimer = JUMP_BUFFER;
                _input.ConsumeJump();
            }

            // trigger skoka
            if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
            {
                _wantJump = true;
                _jumpBufferTimer = 0f;
            }
        }

        public void FixedTick(float fdt)
        {
             if (_rb == null)
    {
        Debug.LogError("PlayerMovement: _rb je NULL u Tick!");
        return;
    }

            if (_wantJump)
            {
                float jumpForce = (_stats != null && _stats.JumpForce > 0f)
                    ? _cfg.JumpForce
                    : _stats.JumpForce;

                // Resetuj vertikalnu brzinu pa dodaj impuls
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
                _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                _wantJump = false;
                _coyoteTimer = 0f;

                _events?.RaiseJump(); // npr. da okine animaciju/sfx
            }
        }

        public void Dispose()
        {
            _input = null;
            _ground = null;
            _stats = null;
            _events = null;
        }

        public void Bind(PlayerEvents events) { }
    }
}
