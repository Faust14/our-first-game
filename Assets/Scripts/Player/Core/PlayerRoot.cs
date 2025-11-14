
using System;
using Game.Player;
using Game.Player.Animation;
using Game.Player.ClassSystem;
using Game.Player.Combat;
using Game.Player.Movement;
using Game.Player.UI;
using Player.Combat;
using Player.FX;
using Player.Input;
using Player.Movement;
using UnityEngine;

namespace Player.Core
{
    /// <summary>
    /// Čisti C# agregat koji sklapa sve podsisteme i vozi njihov lifecycle.
    /// Nema MonoBehaviour zavisnosti.
    /// </summary>
    public sealed class PlayerRoot : IDisposable
    {
        public PlayerConfigSO Config { get; }
        public PlayerEvents Events { get; }
        public PlayerStats Stats { get; }
        public PlayerClassManager ClassManager { get; }

        // Podsistemi (expose po potrebi kroz readonly properties):
        public PlayerInputFacade Input { get; }
        public PlayerMovement Movement { get; }
        public PlayerFlip Flipper { get; }
        public GroundChecker GroundChecker { get; }
        public PlayerHealth Health { get; }
        public PlayerCombatRouter Combat { get; }
        public AbilityController Abilities { get; }
        public PlayerAnimatorBridge AnimatorBridge { get; }
        public PlayerSfx Sfx { get; }
        public PlayerVfx Vfx { get; }
        public PlayerHudBridge Hud { get; }
        private readonly Transform _modelTransform;
        private bool _initialized;

        public PlayerRoot(
            PlayerConfigSO config,
            // sve zavisnosti dolaze spolja (iz PlayerLifecycle), ili ih ovde možeš kreirati ako imaju čiste C# konstruktore:
            PlayerEvents events,
            PlayerInputFacade input,
            GroundChecker groundChecker,
            PlayerMovement movement,
            PlayerFlip flipper,
            PlayerHealth health,
            PlayerCombatRouter combat,
            AbilityController abilities,
            PlayerAnimatorBridge animatorBridge,
            PlayerSfx sfx,
            PlayerVfx vfx,
            PlayerHudBridge hud,
            PlayerStats stats,
            PlayerClassManager classManager,
            Transform modelTransform
        )
        {
            Config = config;
            Events = events ?? new PlayerEvents();
            Input = input;
            GroundChecker = groundChecker;
            Movement = movement;
            Flipper = flipper;
            Health = health;
            Combat = combat;
            Abilities = abilities;
            AnimatorBridge = animatorBridge;
            Sfx = sfx;
            Vfx = vfx;
            Hud = hud;
            Stats = stats;
            ClassManager = classManager;
            _modelTransform = modelTransform;
        }

        /// <summary>Ručno zovi iz PlayerLifecycle.Awake/Start</summary>
       public void Init()
{
    if (_initialized) return;

    if (Config == null)
    {
        Debug.LogError("PlayerRoot.Init: Config is NULL – proveri PlayerLifecycle.Config u inspectoru.");
        return;
    }

    // osnovni sistemi
    Movement?.Bind(Input, GroundChecker, Stats, Events);
    Flipper?.Bind(Movement, Input, _modelTransform);
    Health?.Bind(Stats, Events);

    // combat / abilities
    Combat?.Bind(Input, Abilities, Stats, Events);
    Abilities?.Bind(Input, Stats, Events, ClassManager);

    // animator
    AnimatorBridge?.Bind(Movement, Combat, Abilities, Health, GroundChecker, Events);

    // HUD – samo ako postoji
    if (Hud != null)
        Hud.Bind(Health, Abilities, Stats, ClassManager, Events);

    // FX – samo ako postoje
    Sfx?.Bind(Events);
    Vfx?.Bind(Events);

    // klase / statsi
    ClassManager?.InitializeByType(Stats, Config.DefaultClass, Config.ClassCatalog, Events);

    _initialized = true;
}


        /// <summary>Poziva se iz MonoBehaviour.Update</summary>
        public void Tick(float dt)
        {
            if (!_initialized) return;

            Input?.Tick(dt);
            Abilities?.Tick(dt);
            Combat?.Tick(dt);
            Movement?.Tick(dt);
            Flipper?.Tick(dt);
            AnimatorBridge?.Tick(dt);
            Hud?.Tick(dt);
            // FX sistemi obično reaguju na evente pa ne moraju tick, ali ostavljam hook:
            Sfx?.Tick(dt);
            Vfx?.Tick(dt);
        }

        /// <summary>Poziva se iz MonoBehaviour.FixedUpdate</summary>
        public void FixedTick(float fdt)
        {
            if (!_initialized) return;
            Movement?.FixedTick(fdt);
            GroundChecker?.FixedTick(fdt);
        }

        /// <summary>Poziva se iz MonoBehaviour.LateUpdate (opciono)</summary>
        public void LateTick(float dt)
        {
            if (!_initialized) return;
            AnimatorBridge?.LateTick(dt);
        }

        public void Dispose()
        {
            // Odveži event-ove, oslobodi resurse:
            Abilities?.Dispose();
            Combat?.Dispose();
            Movement?.Dispose();
            Health?.Dispose();
            Hud?.Dispose();
            Sfx?.Dispose();
            Vfx?.Dispose();
            AnimatorBridge?.Dispose();
            Input?.Dispose();
            Events?.Dispose();
        }
    }
}
