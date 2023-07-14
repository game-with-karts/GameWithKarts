using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CountdownScreen : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] countdownImages;
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float countingDelay = 1f;
    [SerializeField] private float endingDelay = 1.5f;

    public Action OnCountdownOver;

    public void StartCountdown() {
        StartCoroutine(nameof(Countdown));
    }

    private IEnumerator Countdown() {
        yield return new WaitForSeconds(initialDelay);
        image.enabled = true;
        
        for (int i = 0; i < countdownImages.Length; i++) {
            image.sprite = countdownImages[i];
            if (i == countdownImages.Length - 1) {
                OnCountdownOver?.Invoke();
                yield return new WaitForSeconds(endingDelay);
            }
            else {
                yield return new WaitForSeconds(countingDelay);
            }
        }
        image.enabled = false;
    }

    public void ResetCountdown() {
        StopCoroutine(nameof(Countdown));
        image.enabled = false;
        image.sprite = countdownImages[0];
    }
}