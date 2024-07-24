using UnityEngine;
using System;
using System.Collections.Generic;

namespace GWK.Kart {
    [RequireComponent(typeof(CarMovement))]
    [RequireComponent(typeof(CarCamera))]
    [RequireComponent(typeof(CarDrifting))]
    [RequireComponent(typeof(CarInput))]
    public class BaseCar : MonoBehaviour, ITargetable {
        private bool startingIsBot;
        private bool startOnAntigrav;

        [SerializeField] private CarMovement movement;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private CarCamera camera;
        [SerializeField] private CarDrifting drifting;
        [SerializeField] private CarInput input;
        [SerializeField] private CarPathFollower path;
        [SerializeField] private CarUI ui;
        [SerializeField] private CarBotController botController;
        [SerializeField] private CarLapTimer timer;
        [SerializeField] private CarAudio audio;
        [SerializeField] private CarCollider collider;
        [SerializeField] private CarItemHandler item;
        [SerializeField] private CarAppearance appearance;
        [SerializeField] private Renderer renderer;
        public CarMovement Movement => movement;
        public Rigidbody RB => rb;
        public CarCamera Camera => camera;
        public CarDrifting Drifting => drifting;
        public CarInput Input => input;
        public CarPathFollower Path => path;
        public CarUI UI => ui;
        public CarBotController BotController => botController;
        public CarLapTimer Timer => timer;
        public CarAudio Audio => audio;
        public CarCollider Collider => collider;
        public CarItemHandler Item => item;
        public CarAppearance Appearance => appearance;
        [SerializeField] private bool isBot;
        public CarDrivingState state = CarDrivingState.Idle;
        public bool IsBot => isBot;
        public bool playerControlled => !startingIsBot;
        public bool isEleminated { get; private set; }
        public bool Finished { get; private set; }
        public ItemProjectile currentProjectile { get; private set; }
        public Action<BaseCar> OnEliminated;
        private List<CarComponent> components;

        public void ResetCar(bool onInit) {
            currentProjectile = null;
            state = CarDrivingState.Idle;
            Finished = false;
            isEleminated = false;
            isBot = startingIsBot;
            movement.SetAntigrav(startOnAntigrav);
            if (!isBot) {
                path.OnRaceEnd += TurnIntoBot;
            }
            InitComponents(onInit);
        }

        public void Init(bool isBot, bool startOnAntigrav) {
            rb.transform.parent = null;
            components = new();
            movement.startingPosition = transform.position;
            movement.startingRotation = transform.rotation;
            startingIsBot = isBot;
            this.startOnAntigrav = startOnAntigrav;
            Finished = false;

            CarComponent[] comps = GetComponents<CarComponent>();
            collider?.SetBaseCar(this);
            foreach (var comp in comps) {
                components.Add(comp);
            }
            ResetCar(true);
        }

        private void InitComponents(bool onInit) {
            foreach (CarComponent c in components) {
                c.Init(!onInit);
                c.InputProvider = isBot ? BotController : Input;
                if (!onInit) {
                    if (c is CarCamera) {
                        (c as CarCamera).ActivateCamera();
                    }
                    if (c is CarUI) {
                        (c as CarUI).ActivateCanvas();
                    }
                }
            }
        }

        private void TurnIntoBot(BaseCar _) {
            isBot = true;
            Finished = true;
            path.OnRaceEnd -= TurnIntoBot;
            components.ForEach(c => c.InputProvider = BotController);
        }

        public void StartRace() {
            components.ForEach(x => x.StartRace());
        }

        void OnDisable() {
            components.ForEach(c => c.InputProvider = null);
        }

        public void Eliminate() {
            isEleminated = true;
            OnEliminated?.Invoke(this);
        }

        public void EndRace() => path.EndRace();

        // ITargetable stuff
        public void MarkAsTarget(ItemProjectile projectile) {
            currentProjectile = projectile;
        }
        
        public void ClearTarget() {
            currentProjectile = null;
        }

        public Vector3 Position => transform.position;
    }

}