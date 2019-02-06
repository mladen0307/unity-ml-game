using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToggle : MonoBehaviour {

    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _camera.orthographic = !_camera.orthographic;
        }
    }
}
