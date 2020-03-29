using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI360 : MonoBehaviour
{
    [Header("Components")]
    public RectTransform rect;
    private GameObject targetGo;
    public Transform arrow;

    [Header("Settings")]
    [Range(-1f, 1f)] public float dotThreshold = 0.2f;
    public float height = 400f;
    public float width = 800f;
    public float maxDiffY = 5f;
    public float maxDiffX = 5f;

    private Player player;
    private Character Chara => player.Character;
    private Camera Cam => Chara.playerCamera.cam;

    private Vector3 objPos => targetGo.transform.position + Vector3.up *5f;
    private Vector3 camPos => Cam.transform.position;

    private Vector3[] compare;

    public void FeedTarget(GameObject target)
    {
        targetGo = target;
    }

    private void Start()
    {
        player = UIManager.Instance.Player;
    }

    private void Pos()
    {
        // temp fix
        if (Cam == null)
        {
            player = UIManager.Instance.Player;
        }

        float dot = targetGo != null ? Vector3.Dot(Cam.transform.forward, (objPos - camPos).normalized) : 0;
        
        Vector3 target = UIManager.Instance.canvas.WorldToCanvas(objPos, Cam) ;
        if (dot < 0)
        {
            target = -target;
            bool a = Mathf.Abs(target.x) > width - 5;

            float x = a ? width * Mathf.Sign(target.x) : target.x;
            float y = a ? target.y : height * Mathf.Sign(target.y);
            target = new Vector3(x, y, 0);
        }
        rect.anchoredPosition = new Vector2(Mathf.Clamp(target.x , -width, width), Mathf.Clamp(target.y, -height, height));        
    }

    private Vector3 WorldScreenSide(Vector2 v)
    {
        return camPos + Cam.transform.rotation * new Vector3(v.x, v.y, 0f);
    }

    private int Closest(Vector3[] vectors)
    {
        int vec = 0;
        for (int i = 0; i < vectors.Length; i++)
        {
            if (Vector3.Distance(vectors[i], objPos) < Vector3.Distance(vectors[vec], objPos))
            {
                vec = i;
            }
        }

        return vec;
    }

    public virtual void Update()
    {
        Pos();
    }
}
