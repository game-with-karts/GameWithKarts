using UnityEngine;
using System;

public class PreRaceSequence : MonoBehaviour
{
    [SerializeField] private CameraSequence[] sequences = new CameraSequence[3];
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private AudioClip preRaceMusic;
    public Action OnSequenceEnd;
    private int currentSequence = -1;

    public void StartSequence() {
        SoundManager.SetMusic(preRaceMusic);
        SoundManager.PlayMusic();
        NextSequence();
    }

    void Update() {
        if (!gameObject.activeSelf) return;
        if (currentSequence < 0 || currentSequence >= sequences.Length) return;
        (Vector3 pos, Quaternion rot) = sequences[currentSequence].Evaluate(Time.deltaTime);
        cameraTransform.SetPositionAndRotation(pos, rot);
    }

    private void NextSequence() {
        currentSequence++;
        if (currentSequence > 0)
            sequences[currentSequence - 1].OnSequenceEnd -= NextSequence;
        if (currentSequence >= sequences.Length) {
            StopSequence();
            return;
        }
        sequences[currentSequence].OnSequenceEnd += NextSequence;
        sequences[currentSequence].Init();
    }

    public void StopSequence() {
        SoundManager.StopMusic();
        cameraTransform.gameObject.SetActive(false);
        gameObject.SetActive(false);
        OnSequenceEnd?.Invoke();
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        foreach (var s in sequences) {
            s.DrawGizmo();
        }
    }
    #endif

}