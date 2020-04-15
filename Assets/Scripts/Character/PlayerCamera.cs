using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pataya.QuikFeedback;

public class PlayerCamera : MonoBehaviour
{
    [Header("Components")]
    public Character character;
    public Camera cam;
    public Transform roll;
    public Animator animator;

    [Header("Roll")]
    public float maxRoll = 10f;
    public AnimationCurve rollCurve;
    public float smoothnessRoll = 1f;
    private float currentVelRoll;
    private float t;

    [Header("FOV")]
    public float minFov = 80;
    public float maxFov = 100;
    public float fovSmooth = 0.5f;
    private float currentVelFov;

    [Header("TPS")]
    public float tpsDistance = 10f;
    public float tpsSide = 10f;
    public float smoothnessCamZ = 0.1f;
    private float targetCamZ;
    private float targetCamX;
    private float currentVelCamZ;
    private float currentVelCamX;

    public Quaternion Forward => Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
    private Vector2 mouse;
    public Vector2 Mouse => mouse;

    private float maxSpeedFov => character.m.runSpeed * character.m.flowSpeedMul;

    private void Update()
    {
        UpdateRoll();
        UpdateFOV();
        UpdateRotation();
        UpdateTPS();
    }

    private void UpdateRoll()
    {
        t = Mathf.SmoothDamp(t, character.CameraRoll, ref currentVelRoll, smoothnessRoll);

        float current = rollCurve.Evaluate(Mathf.Abs(t)) * maxRoll * Mathf.Sign(t);
        roll.transform.localEulerAngles = new Vector3(0, 0, current);
    }

    private void UpdateFOV()
    {
        float target = Utility.Interpolate(minFov, maxFov, 0f, maxSpeedFov, character.Velocity.magnitude * character.DotFacingVelocity);
        float currentFov = Mathf.SmoothDamp(cam.fieldOfView, target, ref currentVelFov, fovSmooth);

        if(!QuikFeedbackManager.instance.IsZooming)
            cam.fieldOfView = currentFov;
    }

    private void UpdateRotation()
    {
        Quaternion x = Quaternion.AngleAxis(mouse.x, Vector3.up);
        Quaternion y = Quaternion.AngleAxis(-mouse.y, Vector3.right);
        transform.rotation = x * y;
    }
    
    private bool isTPS = false;
    public void ToggleTPS(bool on)
    {
        isTPS = on;
        targetCamZ = isTPS ? -tpsDistance:0f;
        targetCamX = isTPS ? -tpsSide:0f;
    }

    private void UpdateTPS()
    {
        float newZ = Mathf.SmoothDamp(cam.transform.localPosition.z, targetCamZ, ref currentVelCamZ, smoothnessCamZ);
        float newX = Mathf.SmoothDamp(cam.transform.localPosition.x, targetCamX, ref currentVelCamX, smoothnessCamZ);
        cam.transform.localPosition = new Vector3(newX, cam.transform.localPosition.y, newZ);
    }

    public void InputMouse(Vector2 mouse)
    {
        this.mouse = mouse;
    }
}
