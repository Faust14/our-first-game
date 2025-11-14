using UnityEngine;
using System.Collections.Generic;

namespace Player.Equipment
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private List<WeaponSO> weapons = new();
        private int currentWeaponIndex = 0;

        public WeaponSO CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;

        public void AddWeapon(WeaponSO weapon)
        {
            if (!weapons.Contains(weapon))
            {
                weapons.Add(weapon);
            }
        }

        public void SwitchWeapon(int index)
        {
            if (index >= 0 && index < weapons.Count)
            {
                currentWeaponIndex = index;
                Debug.Log($"Switched to weapon: {CurrentWeapon.name}");
            }
        }
    }
}
