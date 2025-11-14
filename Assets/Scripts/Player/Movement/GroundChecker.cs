using UnityEngine;

namespace Player.Movement
{
    /// <summary>
    /// Tanki MonoBehaviour koji proverava da li je igrač na tlu (OverlapCircle).
    /// </summary>
    public sealed class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Transform _checkPoint;
        [SerializeField] private float _radius = 0.15f;
        [SerializeField] private LayerMask _groundMask = ~0;

        public bool IsGrounded { get; private set; }

        public void FixedTick(float fdt)
        {
            if (_checkPoint == null) { IsGrounded = false; return; }
            IsGrounded = Physics2D.OverlapCircle(_checkPoint.position, _radius, _groundMask) != null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_checkPoint == null) return;
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_checkPoint.position, _radius);
        }
#endif
    }
}
