using UnityEngine;
using UnityEngine.InputSystem;
using Player.Core;

namespace Player.Input
{
    /// <summary>
    /// Fasada između Unity Input System-a i player subsistema.
    /// </summary>
    public sealed class PlayerInputFacade : System.IDisposable
    {
        private PlayerInputActions _actions; // generisana klasa iz .inputactions asseta

        // Exposed vrednosti koje drugi sistemi koriste
        public Vector2 MoveDirection { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool DashPressed { get; private set; }

        public PlayerInputFacade(PlayerConfigSO config)
        {
            _actions = new PlayerInputActions(); // pretpostavka: imaš InputActions asset nazvan PlayerInputActions
            _actions.Enable();

            // Registruj callbackove
            _actions.Player.Move.performed += ctx => MoveDirection = ctx.ReadValue<Vector2>();
            _actions.Player.Move.canceled += _ => MoveDirection = Vector2.zero;

            _actions.Player.Jump.performed += _ => JumpPressed = true;
            _actions.Player.Jump.canceled += _ => JumpPressed = false;

            _actions.Player.Attack.performed += _ => AttackPressed = true;
            _actions.Player.Attack.canceled += _ => AttackPressed = false;

            _actions.Player.Dash.performed += _ => DashPressed = true;
            _actions.Player.Dash.canceled += _ => DashPressed = false;
        }

        public void Tick(float dt)
        {
            // Možeš ovde resetovati one-shot akcije ako treba (npr. JumpPressed = false nakon što Movement obradi skok)
        }

        public void ConsumeJump() => JumpPressed = false;
        public void ConsumeAttack() => AttackPressed = false;
        public void ConsumeDash() => DashPressed = false;

        public void Dispose()
        {
            _actions.Disable();
        }
    }
}
