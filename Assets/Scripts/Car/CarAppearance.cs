using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CarAppearance : CarComponent
{
    private Volume volume;
    private ChromaticAberration ca;
    private LensDistortion lens;
    [SerializeField] private Transform model;
    [SerializeField] private float jumpAmount;
    [SerializeField] private float landAmount;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float defaultFOV = 60;
    [SerializeField] private float boostFOV = 80;
    [SerializeField] private ParticleSystem speedLines;
    [Space]
    [SerializeField] private AnimationCurve chromaticAberrationCurve;
    private float caAmount;
    private float caTime;
    
    private readonly Vector3 defaultScale = new(1, 1, 1);
    private readonly Vector3 rotationCorrect = new(0, 360, 0);
    private Quaternion currentRot = Quaternion.Euler(0, 90, 0);
    public override void Init()
    {
        car.Drifting.OnJump += JumpAnimation;
        car.Drifting.OnLand += LandAnimation;
        car.Drifting.OnDriftBoost += DriftEffect;

        volume = GameObject.FindGameObjectWithTag("Global Volume").GetComponent<Volume>();
        volume.profile.TryGet(out ca);
        volume.profile.TryGet(out lens);
        speedLines.Stop();
    }

    void Update() 
    {
        model.localScale += (defaultScale - model.localScale) * animationSpeed * Time.deltaTime;

        Vector3 rotDelta = new Vector3(0, 30 * car.Drifting.DriftDirection + 90, 0) - currentRot.eulerAngles;
        currentRot *= Quaternion.Euler(rotDelta * animationSpeed * Time.deltaTime);
        model.localRotation = currentRot * Quaternion.Euler(0, -90, 0);

        float targetFOV = car.Drifting.isBoosting ? boostFOV : defaultFOV;
        car.Camera.FrontFacingCamera.fieldOfView = Mathf.Lerp(car.Camera.FrontFacingCamera.fieldOfView, targetFOV, animationSpeed * Time.deltaTime);
        ca.intensity.value = chromaticAberrationCurve.Evaluate(caTime) * caAmount;
        lens.intensity.value = chromaticAberrationCurve.Evaluate(caTime) * caAmount * -.6f;
        caTime += Time.deltaTime;

        if (car.Drifting.isBoosting)
        {
            if (speedLines.isStopped) speedLines.Play();
        }
        else speedLines.Stop();
    }

    void JumpAnimation()
    {
        model.localScale = new Vector3(.5f, 2, .5f) * jumpAmount + defaultScale;
    }

    void LandAnimation() 
    {
        model.localScale = new Vector3(2, .5f, 2) * landAmount + defaultScale;
    }

    void DriftEffect(float relTime) 
    {
        caAmount = Mathf.Clamp01(1 - relTime);
        caTime = 0;
    }

    
    
}