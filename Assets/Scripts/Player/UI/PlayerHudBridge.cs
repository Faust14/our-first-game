using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Player.Combat;
using Game.Player.ClassSystem;
using Player.Equipment;
using Game.Player.Movement;
using Player.Combat;
using Player.Movement;
using Player.Core;

namespace Game.Player.UI
{
    public class PlayerHudBridge : MonoBehaviour
    {
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider manaBar;
        [SerializeField] private TMP_Text classLabel;
        [SerializeField] private TMP_Text weaponLabel;

        private PlayerHealth playerHealth;
        private PlayerStats playerStats;
        private PlayerInventory inventory;
        private PlayerClassManager classManager;
        public void Init(PlayerHealth health, PlayerStats stats, PlayerInventory inv)
        {
            playerHealth = health;
            playerStats  = stats;
            inventory    = inv;

            if (playerHealth != null)
                playerHealth.OnHealthChanged += UpdateHealth;

            UpdateUI();

            // odmah osveži bar da ne čeka prvi damage/heal
            if (playerHealth != null)
                UpdateHealth(playerHealth.Current, playerHealth.Max);
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
                playerHealth.OnHealthChanged -= UpdateHealth;
        }

        private void UpdateHealth(float current, float max)
        {
            if (healthBar != null && max > 0f)
                healthBar.value = current / max;
        }

        private void UpdateUI()
        {
             if (classLabel != null)
    {
        // Prikaži lep naziv ako postoji, inače enum ime
        if (classManager != null && classManager.ActiveClass != null)
            classLabel.text = !string.IsNullOrEmpty(classManager.ActiveClass.displayName)
                ? classManager.ActiveClass.displayName
                : classManager.ActiveClass.classType.ToString();
        else
            classLabel.text = "-";
    }

    if (weaponLabel != null)
        weaponLabel.text = "None";
        }
        public void Bind(PlayerHealth health,
                         AbilityController abilities,
                         PlayerStats stats,
                         PlayerClassManager classManager,
                         PlayerEvents events)
        {
            // TODO: poveži health bar, mana bar, skill slotove, itd.
        }

        public void Tick(float dt) { }

        public void LateTick(float dt) { }

        public void Dispose() { }
    }
}
