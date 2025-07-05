using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralBackgroundFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private float parallaxFactor = 0.01f; // 0.01이면 느리게 따라옴

    private Vector3 initialOffset;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        initialOffset = transform.position - cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 targetPos = cameraTransform.position + initialOffset * parallaxFactor;
        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
    }
}
