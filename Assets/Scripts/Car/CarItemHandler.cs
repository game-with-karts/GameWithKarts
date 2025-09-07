using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GWK.Kart {
    public class CarItemHandler : CarComponent {
        private bool eventSubscribed = false;
        [SerializeField] private Transform itemSpawnpoint;
        [SerializeField] private ItemWeights itemWeights;
        [SerializeField] private List<ItemEntry> entries;
        public Transform ItemSpawnpoint => itemSpawnpoint;
        private ItemEntry? _currentItem = null;
        private ItemEntry? currentItem {
            get => _currentItem;
            set {
                _currentItem = value;
            }
        }
        public ItemEntry? CurrentItem => currentItem;
        private static System.Random random = new();
        public bool IsRolling { get; private set; }
        public BaseCar target { get; private set; }

        private List<ItemEntry> entriesActual;

        private Timer shieldTimer = new();
        public bool IsShieldActive {get; private set;}
        public event Action<bool> OnShieldEnd;
        public event Action OnShieldStart;

        private Timer invincTimer = new();
        public bool IsInvincible {get; private set;}
        public event Action<bool> OnInvincibility;

        private bool lookingBackwards;
        public bool LookingBackwards => lookingBackwards;
        private void SetBackwards(bool v) => lookingBackwards = v;
        protected override void SubscribeProviderEvents() {
            InputProvider.Item += UseItem;
            InputProvider.BackCamera += SetBackwards;
        }

        protected override void UnsubscribeProviderEvents() {
            InputProvider.Item -= UseItem;
            InputProvider.BackCamera -= SetBackwards;
        }

        
        public void EnableShield() {
            shieldTimer.Reset();
            if (!IsShieldActive) {
                OnShieldStart?.Invoke();
                shieldTimer.Start();
            }
            IsShieldActive = true;
        }

        public void DisableShield(bool broken) {
            if (IsShieldActive) {
                shieldTimer.Stop();
                shieldTimer.Reset();
            }
            OnShieldEnd?.Invoke(broken);
            IsShieldActive = false;
        }

        public void EnableInvincibility() {
            invincTimer.Reset();
            if (!IsInvincible) {
                OnInvincibility?.Invoke(true);
                invincTimer.Start();
            }
            IsInvincible = true;
        }

        public void DisableInvincibility() {
            if (IsInvincible) {
                invincTimer.Stop();
                invincTimer.Reset();            
            }
            OnInvincibility?.Invoke(false);
            IsInvincible = false;
        }

        public override void Init(bool restarting) {
            StopAllCoroutines();
            IsRolling = false;
            currentItem = null;

            if (!eventSubscribed) {
                car.Collider.TriggerEnter += OnTriggerEnter;
                eventSubscribed = true;
            }

            DisableShield(false);
            DisableInvincibility();
        }

        void UseItem() {
            if (currentItem is null) {
                return;
            }
            IItem item = currentItem?.type switch {
                ItemType.BoostTank => new BoostTankItem(),
                ItemType.LaserDisc => new LaserDiscItem(),
                ItemType.SpikeTrap => new ItemTrapItem(),
                ItemType.Freezer => new ItemTrapItem(),
                ItemType.Missile => new MissileItem(),
                ItemType.DynamiteCrate => new ItemTrapItem(),
                ItemType.Shield => new ShieldItem(),
                ItemType.Fireball => new FireballItem(),
                ItemType.Invincibility => new InvincibilityItem(),
                _ => null,
            };
            item?.Use(car, currentItem?.prefab);
            currentItem = null;
            target = null;
        }

        void OnTriggerEnter(Collider other) {
            if (!other.gameObject.CompareTag("Item Box")) {
                return;
            }
            // testing
            if (car.IsBot) {
                return;
            }
            ItemBox itemBox = other.GetComponent<ItemBox>();
            if (!itemBox.IsActive) {
                return;
            }
            RollItem();
        }

        public void RollItem(float duration = 3f) {
            if (currentItem is not null) {
                return;
            }
            if (IsRolling) {
                return;
            }
            IsRolling = true;
            StartCoroutine(RollItemCoroutine(duration));
        }

        public void ForceRollItem(float duration = 3) {
            StopAllCoroutines();
            IsRolling = true;
            StartCoroutine(RollItemCoroutine(duration));
        }

        private IEnumerator RollItemCoroutine(float duration) {
            float time = 0;
            int i = 0;
            float delta = .075f;
            while (time < duration) {
                ItemEntry itemEntry = entries[i];
                i = (i + 1) % entries.Count;
                if (!car.IsBot) {
                    car.Audio.PlayOneShot(car.Audio.ItemRollingSource);
                }
                yield return new WaitForSeconds(delta);
                time += delta;
            }

            int position = car.Path.currentPlacement;

            entriesActual = new(entries.Count);

            foreach (ItemEntry temp in entries) {
                if (GameRulesManager.instance.currentTrack.settings.itemsEnabled[temp.type]) {
                temp.weight = itemWeights.records
                                          .Where(r => r.itemType == temp.type)
                                          .Single()
                                          .GetPlacementWeight(position < 1 ? 1 : position);
                entriesActual.Add(temp);
                }
            }

            int totalWeight = entriesActual.Sum(i => i.weight);
            if (totalWeight == 0) {
                foreach (ItemEntry ie in entriesActual) {
                    ie.weight = 1;
                }
                totalWeight = entriesActual.Sum(i => i.weight);
            }

            int selectedWeight = random.Next(totalWeight);
            i = 0;
            while (selectedWeight >= 0) {
                selectedWeight -= entriesActual[i].weight;
                if (selectedWeight >= 0) {
                    i += 1;
                }
            }
            currentItem = entriesActual[i];
            IsRolling = false;
            if (car.IsBot) {
                car.BotController.SetupItem();
            }
        }

        void Update() {
            shieldTimer.Tick(Time.deltaTime);
            if (shieldTimer.Time >= 15) {
                DisableShield(false);
            }
            invincTimer.Tick(Time.deltaTime);
            if (invincTimer.Time >= 10) {
                DisableInvincibility();
            }
            if (currentItem?.type == ItemType.Missile) {
                IEnumerable<BaseCar> targetables = RaceManager.instance.GetTargetables()
                    .Where(c => Vector3.Dot(transform.forward, c.transform.position - transform.position) > 0)
                    .Where(c => (c.transform.position - transform.position).magnitude < 75f)
                    .Where(c => c != car);
                if (!targetables.Any()) {
                    target = null;
                }
                else {
                    target = targetables.OrderBy(c => (c.transform.position - transform.position).magnitude).First();
                }
            }
        }

        public IEnumerable<Sprite> GetItemSprites() {
            return entries.Select(e => e.image);
        }
    }

    [Serializable]
    public class ItemEntry {
        public ItemType type;
        public Sprite image;
        public GameObject prefab;
        [HideInInspector] public int weight = 1;
    }
}
