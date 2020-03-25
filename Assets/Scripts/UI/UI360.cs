using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI360 : MonoBehaviour
{
    [Header("Components")]
    public RectTransform rect;
    public Renderer rend;

    [Header("Settings")]
    [Range(-1f, 1f)] public float dotThreshold = 0.2f;

    private Player player;
    private Character Chara => player.Character;
    private Camera Cam => Chara.playerCamera.cam;

    public string angle;

    private void Start()
    {
        player = UIManager.Instance.player;
    }

    private void Update()
    {
        angle = Angle.ToString("F2");

        float dot = Vector3.Dot(Chara.Forward, (rend.transform.position - Cam.transform.position).normalized);

        if (dot > dotThreshold)
        {
            transform.position = Cam.WorldToScreenPoint(rend.transform.position);
        }

        else
        {
            float normalizedAngle = 1- Mathf.Abs(Angle / 180f);
            float x = Mathf.Sign(Angle) * normalizedAngle * 1100f;
            float y = 0f;
            rect.anchoredPosition = new Vector3(x, y, 0);
        }
    }

    private float Angle => Vector3.SignedAngle(Chara.Forward, (rend.transform.position - Cam.transform.position).normalized, Vector3.up);

    private bool Side => Mathf.Abs(Angle) < 90f;
}
