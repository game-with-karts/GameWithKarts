using System.Collections;
using UnityEngine;

public class CarCamera : CarComponent
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float smoothingAmount;

    [SerializeField] private Camera frontFacingCamera;
    [Header("Post-race settigns")]
    [Min(0)]
    [SerializeField] private float cameraAnimationDelay;
    [SerializeField] private Vector3 cameraAnimationEulerAngles;
    public Camera FrontFacingCamera => frontFacingCamera;

    private Quaternion offset = Quaternion.identity;

    private void Update() {
        Vector3 targetEuler = transform.eulerAngles;
        if (!car.Movement.IsAntigrav) targetEuler.z = 0;
        Quaternion targetRotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(targetEuler) * offset, Time.deltaTime * smoothingAmount );
        cameraTransform.SetPositionAndRotation(cameraTarget.position, targetRotation);
    }

    public override void Init() {
        cameraTransform.gameObject.SetActive(!car.IsBot);
        cameraTransform.parent = null;
        car.Path.OnRaceEnd += RaceEnd;
        offset = Quaternion.identity;
    }

    private void RaceEnd(BaseCar _) {
        StartCoroutine(nameof(RaceEndAnimation));
    }

    private IEnumerator RaceEndAnimation() {
        yield return new WaitForSeconds(cameraAnimationDelay);
        offset = Quaternion.Euler(cameraAnimationEulerAngles);
    }
}
