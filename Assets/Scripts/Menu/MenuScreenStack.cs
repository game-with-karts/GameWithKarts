using UnityEngine;
using System.Collections.Generic;

public class MenuScreenStack : MonoBehaviour
{
    private Stack<GameObject> stack;
    public static MenuScreenStack instance;
    [SerializeField] private GameObject startScreen;

    void Awake() {
        if (instance is null) instance = this;
        else Destroy(this);
        Init();
    }

    public void Init() {
        stack = new();
        stack.Push(startScreen);
    }

    /// <summary>
    /// You can put some custom animations in this place
    /// </summary>
    public void SetScreen(GameObject screen) {
        stack.Peek().SetActive(false);
        screen.SetActive(true);
        stack.Push(screen);
    }

    /// <summary>
    /// You can put some custom animations in this place
    /// </summary>
    public void Back() {
        stack.Pop().SetActive(false);
        stack.Peek().SetActive(true);
    }
}