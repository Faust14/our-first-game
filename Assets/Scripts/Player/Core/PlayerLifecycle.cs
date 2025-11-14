using UnityEngine;
using Player.Input;
using Player.Movement;
using Player.Combat;
using Player.FX;
using Game.Player.Animation;
using Game.Player.UI;
using Game.Player.ClassSystem;
using Game.Player.Combat;
using Game.Player;
using Game.Player.Movement;

namespace Player.Core
{
    /// <summary>
    /// TANKI MonoBehaviour koji je jedini "most" ka Unity životnom ciklusu.
    /// On kreira PlayerRoot i prosleđuje Update/FixedUpdate/LateUpdate.
    /// </summary>
    public sealed class PlayerLifecycle : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private PlayerConfigSO _config;
        [Header("Scene References")]
        [SerializeField] private Rigidbody2D _rb;                  // ✅ dodaj ovo
        [SerializeField] private GroundChecker _groundCheckerMB; 
        // Reference na scene/child komponente koje su MonoBehaviour (ako ih ima) ili na plain C# inicijalizatore:
        [Header("Bridges/Scene Components")]
        [SerializeField] private PlayerAnimatorBridge _animatorBridge;
        [SerializeField] private PlayerHudBridge _hudBridge;
        [SerializeField] private AbilityController _abilityController;
        [SerializeField] private Transform modelTransform;
        private PlayerRoot _root;

        private void Awake()
        {
            
            _groundCheckerMB ??= GetComponentInChildren<GroundChecker>(true);
            _abilityController ??= GetComponentInChildren<AbilityController>(true);
            // Instantiate čistih C# modula:
            var events = new PlayerEvents();
            var stats  = GetComponent<PlayerStats>() ?? gameObject.AddComponent<PlayerStats>();
            var classMgr = GetComponent<PlayerClassManager>() ?? gameObject.AddComponent<PlayerClassManager>();

            var input = new PlayerInputFacade(_config);        // npr. prosledi mapu akcija iz configa
            var movement = new PlayerMovement(_config, _rb);
            var flipper = new PlayerFlip();
            var health = new PlayerHealth(_config);
            var combat = new PlayerCombatRouter(_config);
            var sfx = GetComponent<PlayerSfx>() ?? gameObject.AddComponent<PlayerSfx>();
            var vfx = GetComponent<PlayerVfx>() ?? gameObject.AddComponent<PlayerVfx>();

            // AnimatorBridge/HudBridge/ GroundChecker mogu biti ili MB ili čisti C#;
            // ovde pretpostavljamo da su MB i već su dodeljeni kroz Inspector.
            var animatorBridge = _animatorBridge;
            var hud = _hudBridge;
            var groundChecker = _groundCheckerMB ?? gameObject.AddComponent<GroundChecker>(); // fallback na čistu klasu ako nema MB
if (_rb == null)
        Debug.LogError("PlayerLifecycle: _rb je NULL! Proveri da li si podesio Rb u inspectoru.");
            _root = new PlayerRoot(
                _config,
                events,
                input,
                groundChecker,
                movement,
                flipper,
                health,
                combat,
                _abilityController,
                animatorBridge,
                sfx,
                vfx,
                hud,
                stats,
                classMgr,
                modelTransform  
            );

            _root.Init();
        }

        private void Update()
        {
            _root?.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _root?.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            _root?.LateTick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _root?.Dispose();
            _root = null;
        }
    }
}
