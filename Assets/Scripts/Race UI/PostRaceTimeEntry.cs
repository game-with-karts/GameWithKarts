using UnityEngine;
using TMPro;
using GWK.Util;

public class PostRaceTimeEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text lapDisplay;
    [SerializeField] private TMP_Text timeDisplay;

    public void Display(string lapTitle, int lapTime) {
        lapDisplay.text = lapTitle;
        timeDisplay.text = StringsUtil.GetFormattedTime(lapTime);
    }
}