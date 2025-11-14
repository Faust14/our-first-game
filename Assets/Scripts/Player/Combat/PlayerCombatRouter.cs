using Player.Core;
using Player.Input;
using Game.Player.ClassSystem;

namespace Player.Combat
{
    /// <summary>
    /// Konsoliduje borbene ulaze i cooldown-e.
    /// Za sada: troši Attack input i diže OnPrimaryAttack event.
    /// </summary>
    public sealed class PlayerCombatRouter : System.IDisposable
    {
        private readonly PlayerConfigSO _cfg;

        private PlayerInputFacade _input;
        private PlayerStats _stats;
        private PlayerEvents _events;

        private float _attackCooldown;

        public PlayerCombatRouter(PlayerConfigSO cfg)
        {
            _cfg = cfg;
        }

        public void Bind(PlayerInputFacade input, object /*AbilityController*/ abilities, PlayerStats stats, PlayerEvents eventsBus)
        {
            _input  = input;
            _stats  = stats;
            _events = eventsBus;
            // abilities zasad ne koristimo direktno; slušaće eventove ili ćeš kasnije zvati metode
        }

        public void Tick(float dt)
        {
            if (_attackCooldown > 0f) _attackCooldown -= dt;

            // primarni napad
            if (_input != null && _input.AttackPressed && _attackCooldown <= 0f)
            {
                _input.ConsumeAttack();

                // cooldown = 1 / AttackSpeed
                var atkSpd = (_stats != null && _stats.AttackSpeed > 0f) ? _stats.AttackSpeed : _cfg.AttackSpeed;
                _attackCooldown = atkSpd > 0f ? (1f / atkSpd) : 0.4f;

                // obavesti sve (animator, SFX, abilities…)
                _events?.RaisePrimaryAttack();
            }

            // primer: Dash kasnije možeš prebaciti u Abilities; ovde bi samo digao _events.RaiseDash();
            if (_input != null && _input.DashPressed)
            {
                _input.ConsumeDash();
                _events?.RaiseDash();
            }
        }

        public void Dispose() { }
    }
}
