using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarCamera))]
[RequireComponent(typeof(CarDrifting))]
[RequireComponent(typeof(CarInput))]
public class BaseCar : MonoBehaviour
{
    private Vector3 startingPosition;
    private Quaternion startingRotation;
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
    public CarMovement Movement => movement;
    public Rigidbody RB => rb;
    public CarCamera Camera => camera;
    public CarDrifting Drifting => drifting;
    public CarInput Input => input;
    public CarPathFollower Path => path;
    public CarUI UI => ui;
    public CarBotController BotController => botController;
    [SerializeField] private bool isBot;
    public bool IsBot => isBot;
    public bool playerControlled => !startingIsBot;
    public bool isEleminated { get; private set; }
    public Action<BaseCar> OnEliminated;
    private List<CarComponent> components;

    void Awake() {}

    public void ResetCar() {
        isEleminated = false;
        isBot = startingIsBot;
        movement.SetAntigrav(startOnAntigrav);
        if (!isBot) {
            path.OnRaceEnd += TurnIntoBot;
        }
        InitComponents();
    }

    public void Init(bool isBot, bool startOnAntigrav) {
        components = new();
        movement.startingPosition = transform.position;
        movement.startingRotation = transform.rotation;
        startingIsBot = isBot;
        this.startOnAntigrav = startOnAntigrav;

        Component[] comps = GetComponents<Component>();
        foreach (var comp in comps) {
            if (comp is CarComponent) {
                components.Add(comp as CarComponent);
            }
        }

        rb.centerOfMass = new Vector3(0, -0.4f, 0);
        ResetCar();
    }

    private void InitComponents() {
        components.ForEach(x => x.Init());
    }

    private void TurnIntoBot(BaseCar _) {
        isBot = true;
        path.OnRaceEnd -= TurnIntoBot;
    }

    public void StartRace() {
        components.ForEach(x => x.StartRace());
    }

    public void Eliminate() {
        isEleminated = true;
        OnEliminated?.Invoke(this);
    }

}
