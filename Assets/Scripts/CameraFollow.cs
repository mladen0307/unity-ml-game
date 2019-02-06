using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float maxSpeed = 5f;
    public float smoothTime = 0.2f;
    public NeuralController nController;
    Vector3 currentVelocity = Vector3.zero;
    Vector3 offset;

    void Start()
    {
        offset = transform.position;
    }

    void LateUpdate()
    {
        target = nController.AgentToFollow;
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref currentVelocity, smoothTime, maxSpeed);
    }
}
