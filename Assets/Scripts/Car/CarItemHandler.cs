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
                car.UI?.SetItemImage(value);
            }
        }
        private static System.Random random = new();
        private bool isRolling = false;

        public override void Init(bool restarting) {
            StopAllCoroutines();
            isRolling = false;
            currentItem = null;

            if (!eventSubscribed) {
                car.Collider.TriggerEnter += OnTriggerEnter;
                eventSubscribed = true;
            }
            car.UI.SetItemImage(null);

            
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
            if (isRolling) {
                return;
            }
            isRolling = true;
            StartCoroutine(RollItemCoroutine(duration));
        }

        public void ForceRollItem(float duration = 3) {
            StopAllCoroutines();
            isRolling = true;
            StartCoroutine(RollItemCoroutine(duration));
        }

        private IEnumerator RollItemCoroutine(float duration) {
            float time = 0;
            int i = 0;
            float delta = .075f;
            while (time < duration) {
                ItemEntry itemEntry = entries[i];
                i = (i + 1) % entries.Count;
                car.UI.SetItemImage(itemEntry);
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
            isRolling = false;
            if (car.IsBot) {
                car.BotController.SetupItem();
            }
        }

        void Update() {
            if (car.Input.AxisItem == 0) {
                return;
            }
            if (currentItem is null) {
                return;
            }
            IItem item = currentItem?.type switch {
                ItemType.BoostTank => new BoostTankItem(),
                ItemType.LaserDisc => new LaserDiscItem(),
                ItemType.SpikeTrap => new ItemTrapItem(),
                _ => null,
            };
            item?.Use(car, currentItem?.prefab);
            currentItem = null;
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