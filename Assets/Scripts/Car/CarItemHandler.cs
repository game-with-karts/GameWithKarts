using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GWK.Kart {
    public class CarItemHandler : CarComponent {
        private bool eventSubscribed = false;
        [SerializeField] private List<ItemEntry> entries;
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

        public override void Init(bool restarting) {
            StopAllCoroutines();
            IsRolling = false;
            currentItem = null;

            if (!eventSubscribed) {
                car.Collider.TriggerEnter += OnTriggerEnter;
                eventSubscribed = true;
            }
        }

        void UseItem() {
            if (currentItem is null) {
                return;
            }
            IItem item = currentItem?.type switch {
                ItemType.BoostTank => new BoostTankItem(),
                ItemType.LaserDisc => new LaserDiscItem(),
                ItemType.SpikeTrap => new ItemTrapItem(),
                ItemType.Missile => new MissileItem(),
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
            int totalWeight = entries.Sum(i => i.weight);
            int selectedWeight = random.Next(totalWeight);
            i = 0;
            while (selectedWeight >= 0) {
                selectedWeight -= entries[i].weight;
                if (selectedWeight >= 0) {
                    i += 1;
                }
            }
            currentItem = entries[i];
            IsRolling = false;
            if (car.IsBot) {
                car.BotController.SetupItem();
            }
        }

        void Update() {
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
    public struct ItemEntry {
        public ItemType type;
        public Sprite image;
        public GameObject prefab;
        public int weight;
    }
}