// Animation/AnimationEventsRelay.cs
using System;
using UnityEngine;

namespace Game.Player.Animation
{
    /// <summary>
    /// Jednostavan relayer za Unity Animation Events.
    /// U klipovima dodaj event-e koji pozivaju javne metode ispod (OnFootstep, OnEnableHitbox, ...).
    /// Subscribe-ujte se iz Combat/Audio/P FX sistema na ove C# event-e.
    /// </summary>
    [DisallowMultipleComponent]
    public class AnimationEventsRelay : MonoBehaviour
    {
        // General
        public event Action Footstep;
        public event Action Landed;
        public event Action Jumped;
        public event Action<string> Marker;                // generički event sa string markerom iz klipa

        // Combat
        public event Action AttackStarted;
        public event Action AttackWindowOpened;            // npr. vreme za input za naredni combo
        public event Action AttackWindowClosed;
        public event Action AttackHit;                     // frame kada treba da registrujemo dmg
        public event Action AttackEnded;

        public event Action<string> EnableHitbox;          // prosledi ID hitbox-a koji pališ
        public event Action<string> DisableHitbox;

        // Abilities
        public event Action DashImpulse;                   // impuls/pojačanje sile tačno na keyframe-u
        public event Action ShootProjectile;               // za archera / mage-a, tačan frame ispucavanja
        public event Action CastStarted;
        public event Action CastEnded;

        // === Methods to be called from Animation Events (clip) ===

        // General
        public void OnFootstep()                 => Footstep?.Invoke();
        public void OnLanded()                   => Landed?.Invoke();
        public void OnJumped()                   => Jumped?.Invoke();
        public void OnMarker(string id)          => Marker?.Invoke(id);

        // Combat
        public void OnAttackStarted()            => AttackStarted?.Invoke();
        public void OnAttackWindowOpen()         => AttackWindowOpened?.Invoke();
        public void OnAttackWindowClose()        => AttackWindowClosed?.Invoke();
        public void OnAttackHit()                => AttackHit?.Invoke();
        public void OnAttackEnded()              => AttackEnded?.Invoke();

        public void OnEnableHitbox(string id)    => EnableHitbox?.Invoke(id);
        public void OnDisableHitbox(string id)   => DisableHitbox?.Invoke(id);

        // Abilities
        public void OnDashImpulse()              => DashImpulse?.Invoke();
        public void OnShootProjectile()          => ShootProjectile?.Invoke();
        public void OnCastStarted()              => CastStarted?.Invoke();
        public void OnCastEnded()                => CastEnded?.Invoke();
    }
}
