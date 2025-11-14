using UnityEngine;

namespace Player.Combat
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(float amount);
        void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitNormal);
    }
}
