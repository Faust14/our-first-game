using UnityEngine;
using Player.Input;
using Game.Player.Movement;

namespace Player.Movement
{
    /// <summary>
    /// Čista klasa koja okreće model po smeru kretanja.
    /// </summary>
    public sealed class PlayerFlip : System.IDisposable
    {
        private Transform _modelTransform;
        private PlayerMovement _movement;
        private PlayerInputFacade _input;

        public void Bind(PlayerMovement movement, PlayerInputFacade input, Transform modelTransform)
        {
            _movement = movement;
            _input = input;
            _modelTransform = modelTransform;
        }

        public void Tick(float dt)
        {
            if (_modelTransform == null || _input == null) return;
            float x = _input.MoveDirection.x;
            if (x > 0.01f) SetFacing(1f);
            else if (x < -0.01f) SetFacing(-1f);
        }

        private void SetFacing(float dir)
        {
            var s = _modelTransform.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(dir);
            _modelTransform.localScale = s;
        }

        public void Dispose() { }
    }
}
