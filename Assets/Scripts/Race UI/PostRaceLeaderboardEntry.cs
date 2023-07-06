using UnityEngine;
using TMPro;

public class PostRaceLeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text positionDisplay;
    [SerializeField] private TMP_Text nameDisplay;

    public void Display(string name, int position) {
        positionDisplay.text = position.ToString();
        nameDisplay.text = name;
    }
}