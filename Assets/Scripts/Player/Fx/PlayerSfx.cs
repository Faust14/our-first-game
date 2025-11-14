using Player.Core;
using UnityEngine;

namespace Player.FX
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerSfx : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        public void Bind(PlayerEvents events)
        {
            // Ovde možeš kasnije da se pretplatiš na evente za zvuk (npr. OnAttack, OnJump)
        }

        public void Tick(float dt) { }

        public void Dispose() { }
        public void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        // Example event hooks
        public void OnAttack(AudioClip clip) => PlaySound(clip);
        public void OnHurt(AudioClip clip) => PlaySound(clip);
        public void OnDeath(AudioClip clip) => PlaySound(clip);
    }
}
