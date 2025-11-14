using Player.Core;
using UnityEngine;

namespace Player.FX
{
    public class PlayerVfx : MonoBehaviour
    {
        public ParticleSystem attackEffect;
        public ParticleSystem hitEffect;
        public ParticleSystem deathEffect;
        public void Bind(PlayerEvents events)
        {
            // Ovde kasnije možeš da se pretplatiš na evente za vizuelne efekte (npr. udarci, eksplozije)
        }

        public void Tick(float dt) { }

        public void Dispose() { }
        public void PlayAttack()
        {
            if (attackEffect != null) attackEffect.Play();
        }

        public void PlayHit()
        {
            if (hitEffect != null) hitEffect.Play();
        }

        public void PlayDeath()
        {
            if (deathEffect != null) deathEffect.Play();
        }
    }
}
