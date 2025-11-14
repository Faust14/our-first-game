using UnityEngine;

namespace Game.Abilities.Abilities
{
    /// <summary>
    /// Puca jedan projektil (Fireball) u smeru facing-a.
    /// Projektil prefab treba da ima Rigidbody2D + skript koji prima SetOwner/SetDamage ili koristi SimpleProjectile (bonus ispod).
    /// </summary>
    public class FireballAbility : AbilityBase
    {
        [Header("Projectile")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 12f;
        public float projectileLife  = 3f;
        public float damageMultiplier = 1.0f;
        public Vector2 spawnOffset = new Vector2(0.6f, 0.25f);

        protected override void OnCastPerform()
        {
            if (!projectilePrefab || owner == null) return;

            int dir = FacingSign();
            Vector3 off = new Vector3(spawnOffset.x * dir, spawnOffset.y, 0f);
            var go = Object.Instantiate(projectilePrefab, owner.transform.position + off, Quaternion.identity);

            float dmg = (stats ? stats.BaseDamage : 10f) * damageMultiplier;

            // Ako koristiš SimpleProjectile:
            var sp = go.GetComponent<SimpleProjectile>();
            if (sp != null)
            {
                sp.Launch(owner, new Vector2(dir, 0f), projectileSpeed, Mathf.RoundToInt(dmg), projectileLife);
                return;
            }

            // U suprotnom, pokušaj generičan API
            go.SendMessage("SetOwner", owner, SendMessageOptions.DontRequireReceiver);
            go.SendMessage("SetDamage", Mathf.RoundToInt(dmg), SendMessageOptions.DontRequireReceiver);
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = new Vector2(dir * projectileSpeed, 0f);
            Object.Destroy(go, projectileLife);
        }
    }
}
