using UnityEngine;
using Game.Player.ClassSystem;
using System.Collections.Generic;

namespace Player.Core
{
    [CreateAssetMenu(menuName = "Player/PlayerConfig", fileName = "PlayerConfig")]
    public sealed class PlayerConfigSO : ScriptableObject
    {
        // ─────────────── CLASSES / CATALOG ───────────────
        [Header("Class Catalog")]
        [Tooltip("Sve definicije klasa koje player može da koristi/otključa.")]
        public List<ClassDefinitionSO> ClassCatalog;

        [Header("Classes")]
        [Tooltip("Klasa koju player dobija na početku.")]
        public ClassType DefaultClass = ClassType.Dexterity;

        // ─────────────── BASE STATS ───────────────
        [Header("Stats (Base Values)")]
        [Min(0f)] public float BaseHealth = 100f;
        [Min(0f)] public float BaseMana   = 50f;
        [Min(0f)] public float BaseDamage = 10f;

        // ─────────────── MOVEMENT ───────────────
        [Header("Movement")]
        [Tooltip("Horizontalna brzina kretanja u jedinicama po sekundi.")]
        [Min(0f)] public float MoveSpeed = 1f;

        [Tooltip("Impuls skoka. Koristi se u AddForce(..., Impulse).")]
        public float JumpForce = 10f;

        [Tooltip("Koliko brzo ubrzava/usporava pri promeni brzine (opcionalno, koristi ako radiš smoothing).")]
        [Min(0f)] public float Acceleration = 12f;

        [Tooltip("Brzina napada (može da utiče na cooldown/animator speed).")]
        [Min(0f)] public float AttackSpeed = 1.0f;

        // ─────────────── PHYSICS ───────────────
        [Header("Physics")]
        [Tooltip("Slojevi koji se smatraju 'zemljom' za GroundChecker.")]
        public LayerMask GroundLayer;

        [Tooltip("Koliko daleko ispod igrača proveravamo tlo (ray/shape cast).")]
        [Min(0f)] public float GroundCheckDistance = 0.2f;

        [Tooltip("Maksimalna brzina pada (clamp vertikalne brzine). Negativna vrednost!")]
        public float MaxFallSpeed = -25f;

        [Tooltip("Ako želiš da kontrolišeš gravitaciju kroz skriptu (Rigidbody2D.GravityScale može ostati na 1).")]
        [Min(0f)] public float GravityScale = 3.5f;

        [Tooltip("Brzina 'flipovanja' sprite-a levo/desno (ako radiš lerp rotacije/scale).")]
        [Min(0f)] public float FlipSpeed = 12f;

        // ─────────────── INPUT MAP (labels/keys) ───────────────
        [Header("Input (Action Names)")]
        [Tooltip("Ime akcije za kretanje u Input sistemu.")]
        public string MoveAction   = "Move";
        [Tooltip("Ime akcije za skok.")]
        public string JumpAction   = "Jump";
        [Tooltip("Ime akcije za napad/glavni udarac.")]
        public string AttackAction = "Attack";
        [Tooltip("Ime akcije za dash.")]
        public string DashAction   = "Dash";
        [Tooltip("Ime akcije za Ability #1.")]
        public string Ability1Action = "Ability1";
        [Tooltip("Ime akcije za Ability #2.")]
        public string Ability2Action = "Ability2";
    }
}
