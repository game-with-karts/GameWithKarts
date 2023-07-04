using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : CarComponent
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float smoothingAmount;

    [SerializeField] private Camera backCamera;
    public Camera BackCamera => backCamera;

    protected override void Awake()
    {
        base.Awake();
        cameraTransform.parent = null;
    }

    private void LateUpdate()
    {
        Vector3 targetEuler = transform.eulerAngles;
        if (!car.Movement.IsAntigrav) targetEuler.z = 0;
        float blend = Mathf.Pow(.5f, smoothingAmount * Time.deltaTime);
        Quaternion targetRotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(targetEuler), blend);
        cameraTransform.SetPositionAndRotation(cameraTarget.position, targetRotation);
    }

    public override void Init()
    {
        cameraTransform.gameObject.SetActive(!car.IsBot);
    }
}
