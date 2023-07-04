using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarCamera))]
[RequireComponent(typeof(CarDrifting))]
[RequireComponent(typeof(CarInput))]
public class BaseCar : MonoBehaviour
{
    [SerializeField] private CarMovement movement;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CarCamera camera;
    [SerializeField] private CarDrifting drifting;
    [SerializeField] private CarInput input;
    [SerializeField] private CarPathFollower path;
    [SerializeField] private CarUI ui;
    public CarMovement Movement => movement;
    public Rigidbody RB => rb;
    public CarCamera Camera => camera;
    public CarDrifting Drifting => drifting;
    public CarInput Input => input;
    public CarPathFollower Path => path;
    public CarUI UI => ui;
    [SerializeField] private bool isBot;
    public bool IsBot => isBot;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void Init(bool isBot, bool startsOnAntigrav) {
        rb.centerOfMass = new Vector3(0, -0.4f, 0);
        if (!isBot) path.OnRaceEnd += TurnIntoBot;
        this.isBot = isBot;
        movement.SetAntigrav(startsOnAntigrav);

        Component[] comps = GetComponents<Component>();
        foreach (var comp in comps) {
            if (comp is CarComponent) {
                (comp as CarComponent).Init();
            }
        }
    }

    private void TurnIntoBot(CarPathFollower _) {
        isBot = true;
        path.OnRaceEnd -= TurnIntoBot;
    }

}
