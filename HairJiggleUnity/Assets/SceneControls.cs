using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControls : MonoBehaviour
{
    public Transform ObjTransform;
    public Camera Camera;
    public Transform CameraPivot;

    public float XRotationSpeed;
    public float YRotationSpeed;
    [Range(0, 1)]
    public float Zoom;
    public float MaxCameraDistance;
    public float MinCameraDistance;
    public float FrustumDistance;
    private Quaternion objStartRotation;
    public float MousewheelZoomSpeed;

    private Vector2 startObjMouse;
    private Vector3 startObjRotation;

    private Vector2 startCameraMouse;
    private Vector3 startCameraRotation;


    private void Start()
    {
        objStartRotation = ObjTransform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoReset();
        }

        UpdateZoom();
        HandleObjRotation();
        HandleCameraRotation();
        UpdateCameraForZoom();
    }

    private void DoReset()
    {
        ObjTransform.rotation = objStartRotation;
        CameraPivot.rotation = Quaternion.identity;
        Zoom = 1f;
    }

    private void UpdateZoom()
    {
        float delta = Input.mouseScrollDelta.y * MousewheelZoomSpeed;
        Zoom = Mathf.Clamp01(Zoom + delta);
    }
    private void UpdateCameraForZoom()
    {
        float cameraZ = Mathf.Lerp(MinCameraDistance, MaxCameraDistance, Zoom);
        Camera.transform.localPosition = new Vector3(0, 0, cameraZ);
        Camera.nearClipPlane = Mathf.Max(0.1f, cameraZ - FrustumDistance / 2);
        Camera.farClipPlane = cameraZ + FrustumDistance / 2;
    }

    private void HandleObjRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startObjMouse = Input.mousePosition;
            startObjRotation = ObjTransform.eulerAngles;
        }
        if (Input.GetMouseButton(0))
        {
            float xDelta = startObjMouse.x - Input.mousePosition.x;
            float xRot = xDelta * XRotationSpeed;
            float yDelta = startObjMouse.y - Input.mousePosition.y;
            float yRot = yDelta * YRotationSpeed;
            ObjTransform.rotation = Quaternion.Euler(startObjRotation.x + yRot, startObjRotation.y + xRot, 0);
        }
    }

    private void HandleCameraRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startCameraMouse = Input.mousePosition;
            startCameraRotation = CameraPivot.eulerAngles;
        }
        if (Input.GetMouseButton(1))
        {
            float xDelta = startCameraMouse.x - Input.mousePosition.x;
            float xRot = xDelta * XRotationSpeed;
            float yDelta = startCameraMouse.y - Input.mousePosition.y;
            float yRot = yDelta * YRotationSpeed;
            CameraPivot.rotation = Quaternion.Euler(startCameraRotation.x + yRot, startCameraRotation.y + xRot, 0);
        }
    }
}
