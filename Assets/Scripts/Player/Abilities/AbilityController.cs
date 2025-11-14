using UnityEngine;
using Game.Player.ClassSystem;
using Player.Input;
using Player.Core;

namespace Game.Player
{
    public enum AbilitySlot { Primary = 0, Secondary = 1, Ability1 = 2, Ability2 = 3 }

    [DisallowMultipleComponent]
    public class AbilityController : MonoBehaviour
    {
        [Header("Slots (drag-n-drop)")]
        public Game.Abilities.AbilityBase primary;
        public Game.Abilities.AbilityBase secondary;
        public Game.Abilities.AbilityBase ability1;
        public Game.Abilities.AbilityBase ability2;

        // optional: referencе iz Bind-a (ako ti zatrebaju kasnije)
        private PlayerInputFacade _input;
        private PlayerStats _stats;
        private PlayerEvents _events;
        private PlayerClassManager _classMgr;

        private void Awake()
        {
            // Auto-init svih AbilityBase na ovom obj + childovima
            var abilities = GetComponentsInChildren<Game.Abilities.AbilityBase>(true);
            foreach (var ab in abilities)
                ab.Initialize(gameObject);
        }

        // 🔌 PlayerRoot očekuje ovo
        public void Bind(PlayerInputFacade input, PlayerStats stats, PlayerEvents eventsBus, PlayerClassManager classMgr)
        {
            _input = input;
            _stats = stats;
            _events = eventsBus;
            _classMgr = classMgr;
        }

        // 🔁 PlayerRoot zove Abilities?.Tick(dt)
        public void Tick(float dt)
        {
            // ostavi prazno ili mapiraj input -> Activate(slot)
            // npr: ako koristiš svoj input, možeš ovde:
            // if (_input.PrimaryPressed) Activate(AbilitySlot.Primary);
        }

        // 🧹 PlayerRoot zove Abilities?.Dispose()
        public void Dispose()
        {
            // odjavi evente ako ih budeš dodavao
        }

        public bool Activate(AbilitySlot slot)
        {
            var ab = GetBySlot(slot);
            return ab != null && ab.TryCast();
        }

        public Game.Abilities.AbilityBase GetBySlot(AbilitySlot slot)
        {
            return slot switch
            {
                AbilitySlot.Primary   => primary,
                AbilitySlot.Secondary => secondary,
                AbilitySlot.Ability1  => ability1,
                AbilitySlot.Ability2  => ability2,
                _ => null
            };
        }

        // test tastatura (po želji ostavi, ili prebaci u _input u Tick)
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J)) Activate(AbilitySlot.Primary);
            if (Input.GetKeyDown(KeyCode.K)) Activate(AbilitySlot.Secondary);
            if (Input.GetKeyDown(KeyCode.U)) Activate(AbilitySlot.Ability1);
            if (Input.GetKeyDown(KeyCode.I)) Activate(AbilitySlot.Ability2);
        }
    }
}
