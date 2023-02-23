using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class SceneControls : MonoBehaviour
{
    public Transform RotationTransform;
    public Transform PanTransform;
    public Transform HeadTransform;
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
    private Vector3 objStartPosition;
    public float MousewheelZoomSpeed;

    private Vector2 startObjMouse;
    private Vector3 startObjRotation;
    private Vector3 startObjPosition;
    private Vector3 startPanScreen;

    private Vector2 startCameraMouse;
    private Vector3 startCameraRotation;

    private ControlState state;
    private ControlState oldState;

    private enum ControlState
    {
        Idle,
        RotatingCharacter,
        PanningCharacter,
        RotatingLight,
    }

    private void Start()
    {
        objStartRotation = RotationTransform.rotation;
        objStartPosition = PanTransform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoReset();
        }
        state = GetControlState();

        if(state == ControlState.Idle)
        {
            DoHeadFollow();
        }
        UpdateZoom();
        HandleObjRotation();
        HandleObjPan();
        HandleCameraRotation();
        UpdateCameraForZoom();

        oldState = state;
    }

    private void DoHeadFollow()
    {
        Plane plane = new Plane(Camera.main.transform.forward, (HeadTransform.position + Camera.main.transform.position) * .5f);

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        Vector3 target = ray.GetPoint(dist);
        HeadTransform.LookAt(target);
    }

    private Vector3 MouseToObjPlane()
    {
        Plane plane = new Plane(Camera.main.transform.forward, PanTransform.position);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    private void HandleObjPan()
    {
        if (state == ControlState.PanningCharacter && oldState != ControlState.PanningCharacter)
        {
            startPanScreen = MouseToObjPlane();
            startObjPosition = PanTransform.position;
        }

        if (state == ControlState.PanningCharacter)
        {
            Vector3 currentPos = MouseToObjPlane();
            Vector3 delta = currentPos - startPanScreen;
            PanTransform.position = startObjPosition + delta;
        }
    }

    private ControlState GetControlState()
    {
        if(Input.GetMouseButton(0))
        {
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                return ControlState.RotatingLight;
            }
            return ControlState.RotatingCharacter;
        }
        if(Input.GetMouseButton(1))
        {
            return ControlState.PanningCharacter;
        }
        return ControlState.Idle;
    }

    private void DoReset()
    {
        RotationTransform.rotation = objStartRotation;
        PanTransform.position = objStartPosition;
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
        if (state == ControlState.RotatingCharacter && oldState != ControlState.RotatingCharacter)
        {
            startObjMouse = Input.mousePosition;
            startObjRotation = RotationTransform.eulerAngles;
        }
        if (state == ControlState.RotatingCharacter)
        {
            float xDelta = startObjMouse.x - Input.mousePosition.x;
            float xRot = xDelta * XRotationSpeed;
            float yDelta = startObjMouse.y - Input.mousePosition.y;
            float yRot = yDelta * YRotationSpeed;
            RotationTransform.rotation = Quaternion.Euler(startObjRotation.x + yRot, startObjRotation.y + xRot, 0);
        }
    }

    private void HandleCameraRotation()
    {
        if (state == ControlState.RotatingLight && oldState != ControlState.RotatingLight)
        {
            startCameraMouse = Input.mousePosition;
            startCameraRotation = CameraPivot.eulerAngles;
        }
        if (state == ControlState.RotatingLight)
        {
            float xDelta = startCameraMouse.x - Input.mousePosition.x;
            float xRot = xDelta * XRotationSpeed;
            float yDelta = startCameraMouse.y - Input.mousePosition.y;
            float yRot = yDelta * YRotationSpeed;
            CameraPivot.rotation = Quaternion.Euler(startCameraRotation.x + yRot, startCameraRotation.y + xRot, 0);
        }
    }
}
