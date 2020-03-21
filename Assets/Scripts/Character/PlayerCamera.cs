using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float smoothnessCamZ = 0.1f;
    private float targetCamZ;
    private float currentVelCamZ;

    public Quaternion Forward => Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
    private Vector2 mouse;

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
        float target = Utility.Interpolate(minFov, maxFov, 0f, character.m.runSpeed, character.Velocity.magnitude * character.FacingVelocity);
        float currentFov = Mathf.SmoothDamp(cam.fieldOfView, target, ref currentVelFov, fovSmooth);
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
    }

    private void UpdateTPS()
    {
        float newZ = Mathf.SmoothDamp(cam.transform.localPosition.z, targetCamZ, ref currentVelCamZ, smoothnessCamZ);
        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, newZ);
    }

    public void InputMouse(Vector2 mouse)
    {
        this.mouse = mouse;
    }
}
