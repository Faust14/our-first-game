using UnityEngine;

namespace Player.Equipment
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Player/Weapon")]
    public class WeaponSO : ScriptableObject
    {
        public string weaponName;
        public Sprite icon;
        public float damage;
        public float attackSpeed;
        public float range;
        public AudioClip attackSound;
        public GameObject projectilePrefab; // optional, for ranged
    }
}
