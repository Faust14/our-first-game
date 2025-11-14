using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.ClassSystem
{
    /// <summary>
    /// Definicija jedne klase – bazne vrednosti i set startnih skilova (prefabi).
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Player/Class Definition", fileName = "ClassDefinition")]
    public class ClassDefinitionSO : ScriptableObject
    {
        [Header("Meta")]
        public ClassType classType;
        public string displayName = "New Class";
        public Sprite icon;

        [Header("Base Stats")]
        [Min(1)] public int   maxHealth    = 100;
        [Min(0)] public int   maxMana      = 50;
        [Min(0)] public float baseDamage   = 10f;
        [Min(0.05f)] public float attackSpeed = 1.0f;   // napada u sekundi (ili multiplikator)
        [Min(0f)] public float moveSpeed   = 6f;       // units/s
        [Min(0f)] public float jumpForce   = 12f;

        [Header("Secondary (opciono)")]
        [Tooltip("Primarni atribut koji klasa forsira – nije obavezan za core loop, ali koristan za skaliranje.")]
        [Min(0)] public int strength;
        [Min(0)] public int dexterity;
        [Min(0)] public int intelligence;

        [Header("Scaling / Modifiers")]
        [Tooltip("Multiplikatori koji se primenjuju povrh baznih vrednosti (1 = bez promene).")]
        [Min(0.1f)] public float damageMultiplier      = 1f;
        [Min(0.1f)] public float attackSpeedMultiplier = 1f;
        [Min(0.1f)] public float moveSpeedMultiplier   = 1f;

        [Header("Starting Abilities")]
        [Tooltip("MonoBehaviour prefab-i (nasleđuju AbilityBase). Biće instancirani i parentovani na igrača.")]
        public List<GameObject> abilityPrefabs = new();

        private void OnValidate()
        {
            displayName = string.IsNullOrWhiteSpace(displayName) ? classType.ToString() : displayName;
        }
    }
}
