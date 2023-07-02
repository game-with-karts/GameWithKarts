using UnityEngine;

public class MenuScreenSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject nextScreen;

    public void Next() {
        MenuScreenStack.instance.SetScreen(nextScreen);
    }

    public void Back() {
        MenuScreenStack.instance.Back();
    }
}