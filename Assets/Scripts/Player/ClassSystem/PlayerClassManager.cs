using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Abilities;
using Player.Core; // IAbility / AbilityBase

namespace Game.Player.ClassSystem
{
    /// <summary>
    /// Drži trenutno aktivnu klasu, instancira klase/skillove i primenjuje statse.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerClassManager : MonoBehaviour
    {
        [Header("Active Class")]
        [SerializeField] private ClassDefinitionSO _activeClass;

        [Header("Ability Parent (opciono)")]
        [Tooltip("Gde će se instancirati ability prefabi (ako je null, koristi transform igrača).")]
        [SerializeField] private Transform _abilityParent;

        private readonly List<GameObject> _spawnedAbilities = new();
        private PlayerStats _stats;
        private bool _initializedViaCode; // da izbegnemo dupli Apply (Initialize vs Start)

        public ClassDefinitionSO ActiveClass => _activeClass;

        /// <summary>Event koji puca kad se primeni nova klasa.</summary>
        public event Action<ClassDefinitionSO> OnClassChanged;

        /// <summary>Helperi za HUD/telemetriju.</summary>
        public ClassType CurrentType => _activeClass != null ? _activeClass.classType : default;
        public string CurrentDisplayName => _activeClass != null ? _activeClass.displayName : string.Empty;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
            if (_abilityParent == null) _abilityParent = transform;
        }

        private void Start()
        {
            // Ako te je već bootstrapovao PlayerRoot.Initialize(...), preskoči Start auto-apply
            if (_initializedViaCode) return;

            if (_activeClass != null)
            {
                ApplyClass(_activeClass);
            }
        }

        /// <summary>
        /// Poziva je PlayerRoot: postavlja stats referencu (ako je došla spolja) i primenjuje default klasu iz Config-a.
        /// </summary>
        public void Initialize(PlayerStats stats, ClassDefinitionSO defaultClass, PlayerEvents events)
        {
            _initializedViaCode = true;

            // Prioritetno koristi prosleđene stats (ako su null, zadrži one iz GetComponent u Awake)
            if (stats != null) _stats = stats;

            if (defaultClass != null)
            {
                ApplyClass(defaultClass);
                Debug.Log($"[PlayerClassManager] Initialize(): postavljena početna klasa '{defaultClass.displayName}'.");
            }
            else if (_activeClass != null)
            {
                // fallback: ako Config.DefaultClass nije prosleđen, ali je već setovan _activeClass u inspectoru
                ApplyClass(_activeClass);
                Debug.Log("[PlayerClassManager] Initialize(): Config.DefaultClass je null, koristim _activeClass iz Inspectora.");
            }
            else
            {
                Debug.LogWarning("[PlayerClassManager] Initialize(): Nije prosleđena klasa (defaultClass) niti je _activeClass setovan u Inspectoru.");
            }

            // events se trenutno ne koristi — možeš ovde da se pretplatiš na promene ako ih bude
            // primer: events.OnLevelUp += ...
        }

        public void SetClass(ClassDefinitionSO newClass)
        {
            if (!newClass || newClass == _activeClass) return;
            ApplyClass(newClass);
        }

        public void SetClassByType(ClassType type, IEnumerable<ClassDefinitionSO> catalog)
        {
            foreach (var def in catalog)
            {
                if (def && def.classType == type)
                {
                    ApplyClass(def);
                    return;
                }
            }
            Debug.LogWarning($"[PlayerClassManager] Class '{type}' nije pronađena u zadatom katalogu.");
        }

        private void ApplyClass(ClassDefinitionSO classDef)
        {
            if (classDef == null)
            {
                Debug.LogWarning("[PlayerClassManager] ApplyClass: classDef je null.");
                return;
            }

            // 1) Stats
            _activeClass = classDef;

            if (_stats == null)
            {
                _stats = GetComponent<PlayerStats>(); // safety net
                if (_stats == null)
                {
                    Debug.LogError("[PlayerClassManager] ApplyClass: PlayerStats komponenta nije pronađena!");
                    return;
                }
            }

            _stats.ApplyClassDefinition(classDef);

            // 2) Abilities – očisti stare, instanciraj nove
            ClearAbilities();

            if (classDef.abilityPrefabs != null)
            {
                foreach (var prefab in classDef.abilityPrefabs)
                {
                    if (!prefab) continue;

                    var go = Instantiate(prefab, _abilityParent ? _abilityParent : transform);
                    _spawnedAbilities.Add(go);

                    // Ako postoji AbilityBase ili IAbility – inicijalizuj
                    var ability = go.GetComponent<AbilityBase>();
                    if (ability != null)
                    {
                        ability.Initialize(gameObject); // prilagodi ako tvoja baza ima drugačiji API
                    }
                    else
                    {
                        var iab = go.GetComponent<IAbility>();
                        iab?.Initialize(gameObject);
                    }
                }
            }

            Debug.Log($"[PlayerClassManager] Primeni klasu: {classDef.displayName}");
            OnClassChanged?.Invoke(classDef);
        }

        private void ClearAbilities()
        {
            for (int i = _spawnedAbilities.Count - 1; i >= 0; i--)
            {
                if (_spawnedAbilities[i])
                    Destroy(_spawnedAbilities[i]);
            }
            _spawnedAbilities.Clear();
        }
        public void InitializeByType(PlayerStats stats, ClassType defaultType, IEnumerable<ClassDefinitionSO> catalog, PlayerEvents events)
        {
            _initializedViaCode = true;

            if (stats != null)
                _stats = stats;

            if (catalog != null)
            {
                SetClassByType(defaultType, catalog);
                Debug.Log($"[PlayerClassManager] InitializeByType(): postavljena početna klasa tipa {defaultType}.");
            }
            else
            {
                Debug.LogWarning("[PlayerClassManager] InitializeByType(): katalog klasa je null, ne mogu da pronađem definiciju klase.");
            }
        }
    }
}
