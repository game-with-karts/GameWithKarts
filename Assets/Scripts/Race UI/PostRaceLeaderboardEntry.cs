using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GWK.Util;

public class PostRaceLeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text positionDisplay;
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private TMP_Text raceTimeDisplay;
    [SerializeField] private Image bg;
    [SerializeField] private Color colorFirst;
    [SerializeField] private Color colorSecond;
    [SerializeField] private Color colorThird;
    [SerializeField] private Color colorDefault;
    public int Position { get; private set; }
    private float colorMult = .5f;

    public void Display(string name, int position, int raceTime) {
        positionDisplay.text = StringsUtil.FormatPlace(position);
        nameDisplay.text = name;
        raceTimeDisplay.text = StringsUtil.GetFormattedTime(raceTime);
        Position = position;
        switch (position) {
            case 1:
                positionDisplay.color = colorFirst;
                nameDisplay.color = colorFirst;
                raceTimeDisplay.color = colorFirst;
                bg.color = colorFirst * colorMult;
                break;
            case 2:
                positionDisplay.color = colorSecond;
                nameDisplay.color = colorSecond;
                raceTimeDisplay.color = colorSecond;
                bg.color = colorSecond * colorMult;
                break;
            case 3:
                positionDisplay.color = colorThird;
                nameDisplay.color = colorThird;
                raceTimeDisplay.color = colorThird;
                bg.color = colorThird * colorMult;
                break;
            default:
                positionDisplay.color = colorDefault;
                nameDisplay.color = colorDefault;
                raceTimeDisplay.color = colorDefault;
                bg.color = colorDefault * colorMult;
                break;
        }
    }
}